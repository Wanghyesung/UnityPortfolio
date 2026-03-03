using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IRaycastEvent
{
    public void Execute()
    {
        //테두리
    }
}


public class NPC : MonoBehaviour
{
    [SerializeField] private LayerMask m_tPlayerMask;

    [SerializeField] private List<SONPCSpeech> m_listSpeech = new();
  
    private void OnCollisionEnter(Collision collision)
    {
        if (m_listSpeech.Count == 0)
            return;

        if ((m_tPlayerMask.value & (1 << collision.gameObject.layer)) != 0)
        {
            collision.gameObject.GetComponent<Player>().RESETAGENT();

            speech();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if ((m_tPlayerMask.value & (1 << collision.gameObject.layer)) != 0)
        {
            SpeechManager.m_Instance.CloseSpeechdUI();
        }
    }

    private void speech()
    {
        Vector3 vQuestPos = transform.position + transform.up * 2.0f;
        if (m_listSpeech.Count == 1)
            SpeechManager.m_Instance.ShowSpeechUI(vQuestPos,m_listSpeech[0]);
        
        else
            SpeechManager.m_Instance.ShowQuestUI(vQuestPos, m_listSpeech);
    }

}
