using Game.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eActionID = InputManager.eActionID;

//만약 프레임이 중요한 스킬이라면 Logic에서 해당 프레임까지 기다리는 로직 생성
public class SkillRunner : MonoBehaviour
{
    //SO에서 설정한 속성값 셋팅 후 런타임에서 관리
    [Serializable]
    public class SkillContext
    {
        public SkillRunner skill;                                          //SO를 들고있는 skill Comonent
        public Animator animator;                                             
        public SkillObject runSkillObject = null;                    //소환된 오브젝트

        public Vector3 designSpawnPostion = Vector3.zero;                  //스킬 소환 위치

        public bool complete = false;                                      //스킬 완료 여부
        public bool pressed = false;                                       //스킬에 해당하는 버튼 누름 여부
        public float chargeTime =0.0f;                                     //누적 시간

        public List<IChargeEvent> chargeEvents = new List<IChargeEvent>(); //차지 이벤트 (스킬 오브젝트의 리스트 참조기 때문에 Clear X

        public bool hitEvent = false;                                      //히트 이벤트 발생 여부
        public int hitIndex = -1;                                          //히트 이벤트 프레임
        public int animHashName;                                           //현재 애니메이션 해시 이름  
    }
    [Serializable]
    public enum eSkillType
    {
        None,
        Default,
        SubSkill,
        MainSkill,
    }

    public enum eSkillState
    {
        Success,
        Failed,
        Waiting
    }

    private CoolDownView[] _cooldowns = null;
    private CoolDownView _runCollDown = null;

    private SOSKill[] _skills;
    private SOSKill _runSkill = null;                                       // 현재 실행중인 스킬
    public SOSKill RunSkill => _runSkill;
    //기본 스킬 틀에서 차지 스킬, 즉발, 캐스팅 

    private Player _player = null;
    public Player OwnerPlayer => _player;

    [SerializeField] private SkillContext _skillContext = new SkillContext();
    public SkillContext Context => _skillContext;

    private int _iCurrentSkillIdx = 0;

    public void Awake()
    {
        _player = GetComponent<Player>();
        _skillContext.animator = _player.Animator;
        _skillContext.skill = this;

        _skills = new SOSKill[PlayerInterfaceSlot.SLOT_SIZE];
        _cooldowns = new CoolDownView[PlayerInterfaceSlot.SLOT_SIZE];
    }

   
    public void Update()
    {
        for(int i = 0; i<_cooldowns.Length; ++i)
        {
            if (_cooldowns[i] == null || _cooldowns[i].IsDone == true)
                continue;

            _cooldowns[i].UpdateCoolTime(Time.deltaTime);
        }
    }

    public void SetSkillDefinition(int slotIdx, SOSKill skill, CoolDownView cooldown)
    {
        _skills[slotIdx] = skill;
        if(cooldown != null)
            _cooldowns[slotIdx] = cooldown;
    }

    // 몬스터는 생성시 스킬 우선 등록, 플레이어는 수시로 변경 가능하도록.

    //Input Manager 콜백 함수
    public void UseSkill(int _slotIdx , CoolDownView _cooldown)
    {
        //스킬과 쿨타임 관리 객체가 없다면
        if (_skills == null || _cooldowns == null)
            return;

        //스킬 슬롯 인덱스를 넘어간다면
        if (_slotIdx < 0 || _slotIdx >= _skills.Length || _slotIdx >= _cooldowns.Length)
            return;
         
        // 이미 스킬 실행중, 이미 누르고 있는 스킬이 아니라면
        if (_runSkill == _skills[_slotIdx] || _skills[_slotIdx] == null)
            return;

        // 쿨타임 체크 , 쿨타임이 없다면 계속 누를 수 있는 스킬
        if (_cooldowns[_slotIdx] != null && _cooldowns[_slotIdx].IsDone == false)
            return;

        //마나 체크
        if (_player.PlayerInfo.MP < _skills[_slotIdx].Option.manacost)
            return;

        //현재 스킬로 지정 후 스킬 시작
        _runSkill = _skills[_slotIdx];
        _runCollDown = _cooldown;
        _iCurrentSkillIdx = 0;

        StartSkill();
    }

    //스킬 시작
    public void StartSkill(){
        //애니메이션 시작
        _player.PlayerInfo.AddMP((int)_runSkill.Option.manacost * -1);
        HealthManager.m_Instance.UpdateMP();


        //스킬 업데이트 시작, 해당 스킬 눌렸다는 걸 저장
        _runSkill.Loggic.startSkillLogic?.UpdateSkill(_skillContext);
        _skillContext.pressed = true;

        //애니메이션 실행
        AnimationSetting();
    }
    public void EndSkill()    //스킬이 끝나면
    {
        _runSkill.Loggic.endSkillLogic?.UpdateSkill(_skillContext);

        OffSkill();
    }

    public void OffSkill()          //초기화
    {
        if(_runCollDown != null)
            _runCollDown.ResetCoolTime();

        _iCurrentSkillIdx = 0; 
        _runSkill = null;
        _runCollDown = null;

        _skillContext.chargeTime = 0.0f;
        _skillContext.pressed = false;
        _skillContext.runSkillObject = null;

        _skillContext.hitEvent = false;
        _skillContext.hitIndex = -1;

        
        //스킬이 다 안끝났는데 끊겼다면
    }

    public void CancelSkill()
    {
        if (_runSkill != null)
        {
            if (_runSkill.Option.destroyOnCancel == true)
            {
                SkillObject pSkillObj = _skillContext.runSkillObject;

                //공격이 아직 생성되지 않았다면
                if (pSkillObj != null && pSkillObj.IsSkillActive == false)
                    _skillContext.runSkillObject.PushObjectPool();
            }
        }

        OffSkill();
    }

    public void SetSpawnPosition(in Vector3 _vPosition)
    {
        _skillContext.designSpawnPostion = _vPosition;
    }

    public void EndPressedSkill()       
    {
        _skillContext.pressed = false;
    }

    public void OnSkillHitEvent(int hitEvent)
    {
        _skillContext.hitEvent = true;
    }


    //스킬이 진행되는 동안
    public void UpdateSkill() 
    {
        //스킬 로직들 (행동트리의 sequence노드를 바탕으로 제작)
        List<SOSkillLogic> listSkill = _runSkill.Loggic.skillLogic;
        eSkillState eResult = eSkillState.Success;

        while(eResult == eSkillState.Success)
        {
            eResult = listSkill[_iCurrentSkillIdx].UpdateSkill(_skillContext);
            if (eResult != eSkillState.Success)
                break;

            //현재 스킬 단계가 끝나면 다음 단계 스킬
            ++_iCurrentSkillIdx;

            //스킬이 끝나면
            if (_iCurrentSkillIdx > listSkill.Count - 1)
            {
                //계속 실행할 수 있는 스킬인지 확인
                if (_runSkill.Option.loopskill == true)
                {
                    StartSkill();
                    _iCurrentSkillIdx = 0;
                }

                else
                    EndSkill();

                break;
            }
        }
       
        if (eResult == eSkillState.Failed)
            EndSkill();
    }

    private void AnimationSetting()
    {
        AnimatorStateInfo animState =
                       _player.Animator.GetCurrentAnimatorStateInfo(_runSkill.Animation.layerIndex);

        switch (_runSkill.Animation.playMode)
        {
            case SkillAnimPlayMode.Trigger:
                {
                    _player.Animator.SetTrigger(_runSkill.Animation.triggerName);
                }
                break;

            case SkillAnimPlayMode.Bool:
                {
                    _player.Animator.SetBool(_runSkill.Animation.boolName, true);
                }
                break;
            case SkillAnimPlayMode.CrossFadeState:
                { 
                    // 같은 상태인데 다시 재생 원치 않으면 skip
                    if (animState.IsName(_runSkill.Animation.stateName) == true)
                        return;

                    _player.Animator.CrossFade(_runSkill.Animation.stateName,
                        _runSkill.Animation.crossFadeTime, _runSkill.Animation.layerIndex, 0f);
                }
                break;
        }

        _skillContext.animHashName = Animator.StringToHash(_runSkill.Animation.stateName);
    }

    private void OffAnimation()
    {

    }
}
