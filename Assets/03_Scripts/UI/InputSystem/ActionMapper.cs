using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine;

using eActionID = InputManager.eActionID;
using tTouchEvent = InputManager.tTouchEvent;
using static InputManager;
public class ActionMapper 
{
    
    public sealed class PointerInputState
    {
        public Vector2 vScreenPos;  // 마지막으로 손가락을 땐 위치
        public Vector2 vMove;       // 이동 벡터(정규화/클램프)
        public Vector2 vDelta;      // 이전프레임과 비교 이동 벡터
        public Vector2 vSwipeDelta; // 스와이프 벡터(프레임 이동량)

        public float fZoomDelta;   // 핀치로 인한 줌 변화량 누적
        public float fRotateDelta; // 두 손가락 회전 각 변화량 누적(도 단위)

        public bool bFireDown;     // 눌림 시작 트리거(한 프레임)
        public bool bFireHeld;     // (연속)
        public bool bFireUp;       // 해제 트리거(한 프레임)
       
        public void ClearAction() { bFireDown = bFireUp = false; fZoomDelta = 0; fRotateDelta = 0; }
    }
    public sealed class ActionState
    {
        public bool bSkillDefault;
        public bool bSkill1;
        public bool bSkill2;
        public bool bMainSkill;
        public bool bItem;
        public Vector2 vDirection;

        public void CopyFrom(in ActionState other)
        {
            bSkillDefault = other.bSkillDefault;
            bSkill1 = other.bSkill1;
            bSkill2 = other.bSkill2;
            bMainSkill = other.bMainSkill;
            bItem = other.bItem;
            vDirection = other.vDirection;
        }
        public void SetBoolean(eActionID _eActionID, bool _bValue)
        {
            switch(_eActionID)
            {
                case eActionID.SkillDefault:
                    bSkillDefault = _bValue;
                    break;

                case eActionID.Skill1:
                    bSkill1 = _bValue;
                    break;

                case eActionID.Skill2:
                    bSkill2 = _bValue;
                    break;

                case eActionID.MainSkill:
                    bMainSkill = _bValue;
                    break;

                case eActionID.Item:
                    bItem = _bValue;
                    break;
            }
        }
        public void SetVector2D(eActionID _eActionID, in Vector2 _vValue)
        {
            switch (_eActionID)
            {
                case eActionID.Move:
                    vDirection = _vValue;
                    break;
            }
        }

        public bool GetBoolean(eActionID _eActionID)
        {
            return _eActionID switch
            {
                eActionID.SkillDefault => bSkillDefault,
                eActionID.Skill1 => bSkill1,
                eActionID.Skill2 => bSkill2,
                eActionID.MainSkill => bMainSkill,
                eActionID.Item => bItem,
                _ => false,
            };
        }

        public Vector2 GetVector2D(eActionID _eActionID)
        {
            return _eActionID switch
            {
                eActionID.Move => vDirection,
                _ => Vector2.zero,
            };
        }

        public void Clear()
        {
            bSkillDefault = false;
            bSkill1 = false;
            bSkill2 = false;
            bMainSkill = false;
            bItem = false;
            vDirection = Vector2.zero;
        }
    }

    
    //다른 디바이스와 맵핑
    public void MapPointer(List<tTouchEvent> _listTouchEvent, PointerInputState _pState, List<PointerBinding> _listPointerBinding)
    {
        _pState.ClearAction();

        foreach(var tEve in _listTouchEvent)
        {
            //바탕이 아닌 UI를 눌렀다면 취소
            if (tEve.bOverUI == true)
                continue;

            switch(tEve.eInputType)
            {
                case InputType.Drag:
                    _pState.vMove = Vector2.ClampMagnitude(tEve.vDelta, 1.0f); // 이동 벡터 정규화
                    _pState.vDelta = Vector2.ClampMagnitude(tEve.vDeltaDrag, 1.0f);
                    break;

                case InputType.Swipe:
                    _pState.vSwipeDelta += Vector2.ClampMagnitude(tEve.vDelta, 1.0f); // 스와이프 벡터 누적
                    break;

                case InputType.Tap:
                    _pState.vScreenPos = tEve.vDelta;
                    _pState.bFireDown = true;  
                    break;

                case InputType.Stay:
                     _pState.bFireHeld = true;  // 롱프레스 유지
                    break;

                case InputType.Pinch:
                    _pState.fZoomDelta += tEve.fValue;   // 두 손가락 회전 각도 변화량
                    break;

                case InputType.Rotate:
                    _pState.fRotateDelta += tEve.fValue; // 두 손가락 회전 각도 변화량
                    break;
            }

            execute_pointer(tEve.eInputType, _listPointerBinding);
        }
    }
    public void MapDevice(ActionState _pState, List<ActionBinding> _listActionBindg)
    {
       
        for(int i = 0; i<_listActionBindg.Count; ++i)
        {
            var pActionRef = _listActionBindg[i];
            for(int j = 0; j< pActionRef.listAction.Count; ++j)
            {
                //해당 액션 (키보드나 다른 장치들)
                if (check_value(_pState, pActionRef,j) == true)
                {
                    //이전엔 눌리지 않았는데 이번엔 눌렸다면
                    if(pActionRef.IsPressed == false && pActionRef.InputFunction != null)
                    {
                        pActionRef.IsPressed = true;
                        pActionRef.InputFunction?.Invoke();
                    }
                    break;
                }
                //저번 프레임에서 눌렸는데 이번 프레임엔 안 눌렸다면
                else if (pActionRef.IsPressed == true) 
                {
                    pActionRef.IsPressed = false;
                    pActionRef.ReleaseFunction?.Invoke();
                }
            }
        }
    }

   
    private bool check_value(ActionState _pState , ActionBinding _pAction, int _idx)
    {
        bool bResult = false;

        eActionID eID = _pAction.ID;
        var pAction = _pAction.listAction[_idx];

        switch (pAction.action.type)
        {
            case InputActionType.Button:

                //만약 UGUI에서 먼저 받았다면
                if (_pState.GetBoolean(eID) == true)
                    return true;

                bool bPressed = pAction.action.IsPressed();
                _pState.SetBoolean(eID, bPressed);

                bResult = bPressed;
                break;
            case InputActionType.Value:

                if (_pState.GetVector2D(eID) != Vector2.zero)
                    return true;

                var tVector2 = pAction.action.ReadValue<Vector2>();
                if(tVector2 != Vector2.zero)
                    _pState.SetVector2D(eID, tVector2);

                bResult = tVector2 != Vector2.zero;
                break;
        }

        return bResult;
    }
    private void execute_pointer(InputType _eType, List<PointerBinding> _listPointerBinding)
    {
        PointerBinding pBindTarget = _listPointerBinding[(int)_eType];
        if(pBindTarget != null)
        {
           pBindTarget.PointerFunctions?.Invoke();
        }
    }
}

