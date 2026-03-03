using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public static class InitializeInput
{
    // 씬 로드 전에 가장 먼저 실행
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        // 1) 에디터/PC에서 가상 터치 디바이스가 없으면 생성
        if (Touchscreen.current == null)
            InputSystem.AddDevice<Touchscreen>();

        EnableInput();
    }

    public static void DisableInput()
    {
        EnhancedTouchSupport.Disable();
#if UNITY_EDITOR || UNITY_STANDALONE
        TouchSimulation.Disable();
#endif
    }

    public static void EnableInput()
    {
        // 2) EnhancedTouch 파이프라인 + 마우스→터치 시뮬 ON
        EnhancedTouchSupport.Enable();

#if UNITY_EDITOR || UNITY_STANDALONE
        TouchSimulation.Enable();        // 에디터/PC: 마우스=싱글터치
#endif

    }
}