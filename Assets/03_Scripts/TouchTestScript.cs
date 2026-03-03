using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;

public class TouchTestScript : MonoBehaviour
{
    // 1) 누름/뗌: <Touchscreen>/press  (Button)
    public InputAction press = new InputAction(
        type: InputActionType.Button, binding: "<Touchscreen>/press");

    // 2) 접촉 여부: <Touchscreen>/touch0/touchContact  (Button)
    public InputAction contact = new InputAction(
        type: InputActionType.Button, binding: "<Touchscreen>/touch0/touchContact");

    // 3) 탭 제스처: <Touchscreen>/tap  (Button) + Interactions=Tap
    public InputAction tap = new InputAction(
        type: InputActionType.Button, binding: "<Touchscreen>/tap",
        interactions: "tap(duration=0.2,pressPoint=0.1)"); // 옵션은 필요에 맞게

    // 포지션/델타(이동 패드/드래그용)
    public InputAction position = new InputAction(
        type: InputActionType.PassThrough, binding: "<Touchscreen>/touch0/position"); // Vector2
    public InputAction delta = new InputAction(
        type: InputActionType.PassThrough, binding: "<Touchscreen>/touch0/delta");     // Vector2


    void OnEnable()
    {
        press.Enable();
        contact.Enable();
        tap.Enable();
        position.Enable();
        delta.Enable();

        press.started += OnPress; press.canceled += OnPress; press.performed += OnPress;
        contact.started += OnContact; contact.canceled += OnContact; contact.performed += OnContact;
        tap.performed += OnTap;       // Tap은 보통 performed만 쓰면 됨

        position.performed += OnPosition;
        delta.performed += OnDelta;
    }

    void OnDisable()
    {
        press.started -= OnPress; press.canceled -= OnPress; press.performed -= OnPress;
        contact.started -= OnContact; contact.canceled -= OnContact; contact.performed -= OnContact;
        tap.performed -= OnTap;

        position.performed -= OnPosition;
        delta.performed -= OnDelta;

        position.Disable(); delta.Disable();
    }

    // --- 콜백들 ---
    void OnPress(InputAction.CallbackContext ctx)
    {
        // 버튼 파이프라인: started(누르기 시작), performed(조건 성립시점), canceled(떼었을 때)
        if (ctx.started) Debug.Log("Press started");
        if (ctx.performed) Debug.Log("Press performed");
        if (ctx.canceled) Debug.Log("Press canceled");
    }

    void OnContact(InputAction.CallbackContext ctx)
    {
        // 손가락이 화면에 닿아있는가 (touchContact)
        // ReadValue<float>() : 1(접촉) / 0(비접촉)
        float v = ctx.ReadValue<float>();
        Debug.Log($"TouchContact = {v}");
    }

    void OnTap(InputAction.CallbackContext ctx)
    {
        // 탭 제스처 감지
        Debug.Log("Tap!");
    }

    void OnPosition(InputAction.CallbackContext ctx)
    {
        Vector2 pos = ctx.ReadValue<Vector2>();
        Debug.Log($"Touch pos = {pos}");
    }

    void OnDelta(InputAction.CallbackContext ctx)
    {
        Vector2 d = ctx.ReadValue<Vector2>();
        Debug.Log($"Touch delta = {d}");
    }
}
