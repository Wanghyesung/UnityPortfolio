using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
#endif

public class CameraManager : MonoBehaviour
{
    public static CameraManager m_Instance = null;

    [Header("BaseMove")]
    //시네머신 기준
    [SerializeField] Camera m_pMainCamera = null;
    [SerializeField] float m_fMoveSpeed = 10.0f;
    [SerializeField] float m_fZoomSpeed = 1.0f;
    //[SerializeField] private CinemachineVirtualCamera m_pMainCamera = null;
    private bool m_bFreeView = false;

    private Vector3 m_vBaseFollowOffset;
 
    private CinemachineTransposer m_pMainTransposer;

    [Header("SmoothMove")]
    [SerializeField] private CinemachineSmoothPath m_pPath = null;
    [SerializeField] private CinemachineVirtualCamera m_pDollyCamera = null;

    private CinemachineTrackedDolly m_pDollyComp = null;
    private CinemachineComposer m_pDollyComposer = null;

    private float m_fBaseScreenY = 0.5f;
    [SerializeField] private float m_fTargetScreenY = 0.65f;
    [SerializeField] private float m_fSmoothPathTime = 1.0f;

    private Coroutine m_pSmoothPathCoroutine = null;

    [Header("postprocess")]
    [SerializeField] DamageScreenFx m_pDamagePostProcess= null;
    public void Awake()
    {
        if(m_Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        m_Instance = this;

        m_pMainTransposer = m_pMainCamera.GetComponentInChildren<CinemachineTransposer>();
        m_pDollyComp = m_pDollyCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        m_pDollyComposer = m_pDollyCamera.GetCinemachineComponent<CinemachineComposer>();

        if (m_pDollyComposer != null)
            m_fBaseScreenY = m_pDollyComposer.m_ScreenY;

        m_pDollyComp.gameObject.SetActive(false);
    }

    public void MoveCameraPosition()
    {
        if (m_bFreeView == false)
            return;

        Vector2 vDelta = InputManager.m_Instance.PointerState.vDelta;
        if (vDelta == Vector2.zero)
            return;

        Vector3 vOffset = m_pMainTransposer.m_FollowOffset;
        vOffset.x -= vDelta.x * m_fMoveSpeed * Time.deltaTime;
        vOffset.z -= vDelta.y * m_fMoveSpeed * Time.deltaTime;

        m_pMainTransposer.m_FollowOffset = vOffset;

    }

    public void ZoomCamera()
    {
        if (m_bFreeView == false)
            return;
        //핀치면
        float fZoom = InputManager.m_Instance.PointerState.fZoomDelta;
        
        if(fZoom > 0.0f)
        {
            Vector3 vOffset = m_pMainTransposer.m_FollowOffset;

            // 현재 거리
            float fDist = vOffset.magnitude;

            // 거리 증가/감소
            fDist -= fZoom * m_fZoomSpeed * Time.deltaTime;

            // 최소 / 최대 거리 제한
            fDist = Mathf.Clamp(fDist, 4f, 30f);
         
            // 방향을 유지한 상태에서 거리 재적용
            vOffset = vOffset.normalized * fDist;

            m_pMainTransposer.m_FollowOffset = vOffset;
        }
    }
    public void FreeViewButton()
    {
        if(m_bFreeView == false)
        {
            m_vBaseFollowOffset = m_pMainTransposer.m_FollowOffset;

            m_bFreeView = true;
        }
        else
        {
            m_bFreeView = false;
            m_pMainTransposer.m_FollowOffset = m_vBaseFollowOffset;
        }
    }

    public void DamagedPostProcess()
    {
        m_pDamagePostProcess?.PlayHitVignette();
    }
    public void MainCameraMove()
    {
       
        m_pDollyComp.m_PathPosition = 0.0f;
        m_pDollyComposer.m_ScreenY = m_fTargetScreenY;

        m_pSmoothPathCoroutine = null;
        m_pDollyCamera.Priority = 50;
    }
   
    public void SmoothMove()
    {
        if (m_pSmoothPathCoroutine != null)
            return;

        m_pDollyCamera.Priority = 70; //메인보다 우선적으로
        m_pSmoothPathCoroutine = StartCoroutine(smooth_move());
    }



    private IEnumerator smooth_move()
    {
        float fCurTime = 0.0f;

        float fStartPathPos = m_pDollyComp.m_PathPosition;
        float fStartScreenY = m_pDollyComposer.m_ScreenY;

        while (fCurTime <= m_fSmoothPathTime)
        {
            fCurTime += Time.deltaTime;
            float t = fCurTime / m_fSmoothPathTime;

            
            m_pDollyComp.m_PathPosition = Mathf.Lerp(fStartPathPos, 1.0f, t);

            // 카메라 시선 위로 (0.65)
            m_pDollyComposer.m_ScreenY = Mathf.Lerp(fStartScreenY, m_fTargetScreenY, t);

            yield return null;
        }

    }

    
}
