using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using InputFrame = InputManager.InputFrame;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using tTouchEvent = InputManager.tTouchEvent;

using System.Linq;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Utilities;
using InputType = InputManager.InputType;
public class TouchTracker 
{

    //private Dictionary<int, tTrack> m_hashTrack = new Dictionary<int, tTrack>();
    List<bool> m_listTouchOverUI = Enumerable.Repeat(false, 10).ToList();

    private const float m_fTapMaxTime = 0.2f;  // 탭 최대 지속시간
    private const float m_fTapMaxMove = 3.0f;   // 탭 허용 이동량
    private const float m_fLongPressTime = 0.45f; // 롱프레스 최소 시간
    private const float m_fSwipeMinDist = 5.0f;   // 스와이프 최소 거리
    private const float m_fDragStartDist = 1.0f;   // 드래그 시작 거리


    public void UpdateTouchPhase(ref ReadOnlyArray<Touch> _listTouch,
        List<tTouchEvent> _listResultTouch, UIRayCaster _pRayCaster)
    {
        _listResultTouch.Clear();

        for(int i = 0; i< _listTouch.Count; ++i)
        {
            switch (_listTouch[i].phase)
            {
                case TouchPhase.Began:
                    began_touch(_listTouch[i], _pRayCaster);
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    moved_touch(_listTouch[i], _listResultTouch);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    ended_touch(_listTouch[i], _listResultTouch);
                    break;
            }
        }
        

        //페어가 바뀌지 않게 가장 처음 2개를 우선으로 정렬해서
        if (_listTouch.Count >= 2)
        {
            int iFirstID = 0;
            int iSecondID = 0;

            for(int i = 0; i< _listTouch.Count; ++i)
            {
                iFirstID = _listTouch[i].startTime > _listTouch[iFirstID].startTime ? i : iFirstID;
            }
            for(int i = 1; i< _listTouch.Count; ++i)
            {
                if (i != iFirstID)
                    iSecondID = _listTouch[i].startTime < _listTouch[iSecondID].startTime ? i : iSecondID;
            }
            track_multi_touch(_listResultTouch, _listTouch[iFirstID], _listTouch[iSecondID]);
        }
           
    }


    private void add_touch_event(List<tTouchEvent> _listTouchEvent, InputType _eInputType, 
              Vector2 _vDelta , Vector2 _vDeltaDrag, float _fValue, bool _bOverUI, /*Test*/ int _iID)
    {
        _listTouchEvent.Add(new tTouchEvent
        {
            eInputType = _eInputType,
            vDelta = _vDelta,
            vDeltaDrag = _vDeltaDrag,
            fValue = _fValue,
            bOverUI = _bOverUI
        });
    }

    private void track_multi_touch(List<tTouchEvent> _listTouchEvent, in Touch _tFirstTouch, in Touch _tSecondTouch)
    {
        int iFirstID = _tFirstTouch.finger.index;
        int iSecondID = _tSecondTouch.finger.index;

        float fCurDist = (_tFirstTouch.screenPosition - _tSecondTouch.screenPosition).magnitude;                //현재 두 손가락 거리
        float fStartDist = (_tFirstTouch.startScreenPosition - _tSecondTouch.startScreenPosition).magnitude;    //시작 두 손가락 거리
        float fScale = fCurDist / fStartDist;                                                                   //핀치 스케일 변화

        add_touch_event(_listTouchEvent, InputType.Pinch, 
             Vector2.zero, Vector2.zero, fScale,
            m_listTouchOverUI[iFirstID] && m_listTouchOverUI[iSecondID], iFirstID | iSecondID);

    }

    private void moved_touch(in Touch _tTouch, List<tTouchEvent> _listTouchEvent)
    {
        int iID = _tTouch.finger.index;
        if (m_listTouchOverUI[iID] == true)
            return;

        //과거에 있들어왔던 터치시간과 비교
        float fDeltaTime = (float)(_tTouch.time - _tTouch.startTime);
        Vector2 vDist = _tTouch.screenPosition - _tTouch.startScreenPosition;
      
        if (fDeltaTime >= m_fLongPressTime && vDist.magnitude < m_fDragStartDist)
        {
            add_touch_event(_listTouchEvent, InputType.Stay,
                Vector2.zero, _tTouch.delta, fDeltaTime, m_listTouchOverUI[iID], iID);
        }

        else if (vDist.magnitude >= m_fDragStartDist) //일정거리 이상 이동 드래그
        {
            add_touch_event(_listTouchEvent, InputType.Drag,
                vDist, _tTouch.delta, fDeltaTime, m_listTouchOverUI[iID], iID);
        }
    }

    private void ended_touch(in Touch _tTouch, List<tTouchEvent> _listTouchEvent)
    {
        int iID = _tTouch.finger.index;
        if (m_listTouchOverUI[iID] == true)
        {
            m_listTouchOverUI[iID] = false;
            return;
        }

        float fDeltaTime = (float)(_tTouch.time - _tTouch.startTime);
        Vector2 vDist = _tTouch.screenPosition - _tTouch.startScreenPosition;

        //짧은 시간, 적은 이동량
        if (fDeltaTime <= m_fTapMaxTime && vDist.magnitude < m_fTapMaxMove)                 
        {
            add_touch_event(_listTouchEvent, InputType.Tap,
               _tTouch.screenPosition, Vector2.zero, fDeltaTime, m_listTouchOverUI[iID], iID);
        }

        //큰 이동량
        else if (vDist.magnitude >= m_fSwipeMinDist) //스와이프 인식
        {
            add_touch_event(_listTouchEvent, InputType.Swipe,
                vDist, _tTouch.delta, fDeltaTime, m_listTouchOverUI[iID], iID);
        }

        m_listTouchOverUI[iID] = false; 
    }

    private void began_touch(in Touch _tTouch, UIRayCaster _pRayCaster)
    {
        m_listTouchOverUI[_tTouch.finger.index] = _pRayCaster.IsOverUI(_tTouch.screenPosition);
    }
}
