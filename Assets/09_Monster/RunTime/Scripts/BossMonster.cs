using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

using Color = UnityEngine.Color;

public class BossMonster : Monster
{
    /*
     renderer.material.color = Color.white;
    머티리얼이 인스턴스 복제 다른 오브젝트에 영향 주면 안 되니까 이 Renderer 전용 Material 인스턴스
    몬스터 많아지면 메모리/드로우콜 증가
    모바일에서 특히 위험

    렌더링 시점에 GPU로 보낼 때: 
    MaterialPropertyBlock은: 머티리얼 자체는 안 건드림 Renderer 1개에만 임시로 덮어씀(머티리얼의 원래 값 위에 “이번 렌더링 때만 덮어쓰는 임시 값”)
     */
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    [SerializeField] private Renderer[] m_arrRenderer = null;
    private Color[][] m_arrOriginColor;
    private MaterialPropertyBlock m_pMaterialBlock = null;
    private Coroutine m_pFlashCoroutine = null;

    [SerializeField] private float m_fFlashTime = 0.4f;
    private float m_fCurFlashTime = 0.0f;
    override protected void Awake()
    {
        base.Awake();

        if(m_arrRenderer.Length <= 0)
            m_arrRenderer = GetComponentsInChildren<MeshRenderer>();
        m_pMaterialBlock = new MaterialPropertyBlock();

        m_arrOriginColor = new Color[m_arrRenderer.Length][];

        for (int i = 0; i < m_arrRenderer.Length; ++i)
        {
            Material[] arrMat = m_arrRenderer[i].sharedMaterials;

            m_arrOriginColor[i] = new Color[arrMat.Length];

            for (int m = 0; m < arrMat.Length; ++m)
            {
                Material pMat = arrMat[m];

                if (pMat == null)
                {
                    m_arrOriginColor[i][m] = Color.white;
                    continue;
                }

                if (pMat.HasProperty(BaseColorId))
                    m_arrOriginColor[i][m] = pMat.GetColor(BaseColorId);
                else if (pMat.HasProperty(ColorId))
                    m_arrOriginColor[i][m] = pMat.GetColor(ColorId);
                else
                    m_arrOriginColor[i][m] = Color.white;
            }
        }
    }

    override protected void Start()
    {
        base.Start();
    }

    override protected void OnDisable()
    {
        base.OnDisable();
        m_pNavMeshAgent.isStopped = false;
    }
   
    override public void MonsterUpdate()
    {
        base.MonsterUpdate();
    }

    override public void MonsterLateUpdate()
    {
        base.MonsterLateUpdate();
    }

    public override void TakeDamage(AttackInfo _pAttackInfo)
    {
        DamageManager.m_Instance.Damaged(m_pMonsterInfo, _pAttackInfo);
        m_pBlackbard.HpRatio = (float)m_pMonsterInfo.HP / m_pMonsterInfo.MaxHp;

        if (m_pFlashCoroutine != null)
            StopCoroutine(m_pFlashCoroutine);

        StartCoroutine(FlashRoutine());

        if (m_pMonsterInfo.HP <= 0.0f)
        {
            ClearAttackObject();
            m_pNavMeshAgent.isStopped = true;
            Dead();
        }

        m_pMonsterHPBar?.UpdateHPBar(m_pMonsterInfo.HP, m_pMonsterInfo.MaxHp);

        if (_pAttackInfo.HitSound != null)
            SoundManager.m_Instance.PlaySfx(_pAttackInfo.HitSound, transform);
    }

   private IEnumerator FlashRoutine()
    {
        m_fCurFlashTime = 0.0f;
        SetColorAll(Color.white * 2);
       
        while(m_fCurFlashTime < m_fFlashTime)
        {
            m_fCurFlashTime += Time.deltaTime;
            yield return null;
        }
        ResetColorAll();
        m_pFlashCoroutine = null;
    }

    private void SetColorAll(in Color _tColor)
    {
        for (int i = 0; i < m_arrRenderer.Length; ++i)
        {
            Renderer pRenderer = m_arrRenderer[i];

            for (int m = 0; m < pRenderer.sharedMaterials.Length; ++m)
            {
                pRenderer.GetPropertyBlock(m_pMaterialBlock, m);

                m_pMaterialBlock.SetColor(BaseColorId, _tColor);
                m_pMaterialBlock.SetColor(ColorId, _tColor);

                pRenderer.SetPropertyBlock(m_pMaterialBlock, m);
            }
        }
    }

    private void ResetColorAll()
    {
        for (int i = 0; i < m_arrRenderer.Length; ++i)
        {
            Renderer pRenderer = m_arrRenderer[i];

            for (int m = 0; m < pRenderer.sharedMaterials.Length; ++m)
            {
                pRenderer.GetPropertyBlock(m_pMaterialBlock, m);

                m_pMaterialBlock.SetColor(BaseColorId, m_arrOriginColor[i][m]);
                m_pMaterialBlock.SetColor(ColorId, m_arrOriginColor[i][m]);

                pRenderer.SetPropertyBlock(m_pMaterialBlock, m);
            }
        }
    }


}
