using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Game.Common;
using System.Text.RegularExpressions;

public class PlayerScript : MonoBehaviour
{

    
    private SkillRunner     _skillrunner;    // 스킬 실행기
    private CharacterController _controller;   // 캐릭터 컨트롤러

    // ================== 실제 구현 변수 ==================

    [SerializeField] private Rigidbody  _Rigidbody;
    [SerializeField] private Collider   _Collider;

    // ================== 임시 조작용 변수 =================
    InputManager action;
    InputAction moveAction;
    InputAction activeAction;

    [SerializeField]    private float Speed = 2f;

    [SerializeField]    private float RotateTime = 0.1f;
    private float rotateVelocity;                       // 회전 속도 보관용
    private Vector3 lastNonzeroDir = Vector3.forward;   // 마지막으로 0이 아닌 방향 벡터 저장

    #region Initialization
    // ============================================================
    //                       Initialization
    // ============================================================


    private void Awake()
    {
        _skillrunner = this.GetOrAdd<SkillRunner>();
        _controller = this.GetOrAdd<CharacterController>();

        if (_controller.skinWidth < 0.02f) _controller.skinWidth = 0.02f;

        // 입력 세팅
        action = new InputManager();

        activeAction.ChangeBinding(0).WithName("DefaultAttack");
        activeAction.ChangeBinding(1).WithName("Skill1");
        activeAction.ChangeBinding(2).WithName("Skill2");
    }

    private void OnEnable()
    {
        moveAction.Enable();
        activeAction.Enable();
        moveAction.performed += OnActivePerformed;
        activeAction.performed += OnActivePerformed;
    }

    private void OnDisable()
    {
        activeAction.performed -= OnActivePerformed;
        moveAction.performed -= OnActivePerformed;
        activeAction.Disable();
        moveAction.Disable();
    }


    // ============================================================
    #endregion

    #region Main Loop
    // ============================================================
    //                         Main Loop
    // ============================================================

    private void Update()
    {
        MOVE();
    }

    private void MOVE()
    {
        // 1) 입력
        Vector2 input = moveAction.ReadValue<Vector2>();

        // 2) 평면 이동 방향
        Vector3 moveDir = new Vector3(input.x, 0f, input.y);
        if (moveDir.sqrMagnitude > 0.0001f)
            lastNonzeroDir = moveDir.normalized;

        // 3) 회전
        float targetY = Mathf.Atan2(lastNonzeroDir.x, lastNonzeroDir.z) * Mathf.Rad2Deg;
        float currentY = transform.eulerAngles.y;
        float smoothY = Mathf.SmoothDampAngle(currentY, targetY, ref rotateVelocity, RotateTime);
        transform.rotation = Quaternion.Euler(0f, smoothY, 0f);

        // 4) 평면 속도
        Vector3 planar = (moveDir.sqrMagnitude > 1f ? moveDir.normalized : moveDir) * Speed;


        // 5) 이동 적용
        Vector3 motion = planar;
        _controller.Move(motion * Time.deltaTime);
    }

    private void OnActivePerformed(InputAction.CallbackContext context)
    {
        // 이번에 입력된 실제 컨트롤
        var control = context.control;

        // 키보드 키라면 KeyControl로 캐스팅
        int idx = context.action.GetBindingIndexForControl(context.control);
        var binding = context.action.bindings[idx];
        
        switch(binding.name)
        {   
            case "DefaultAttack":
                Debug.Log("기본 공격 실행");
                break;
            case "Skill1":
                Debug.Log("스킬1 실행");
                break;
            case "Skill2":
                Debug.Log("스킬2 실행");
                break;
            default:
                Debug.LogWarning($"알 수 없는 액션: {binding.name}");
                break;
        }
    }

    // ============================================================
    #endregion
}