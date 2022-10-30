using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DuolBots;

public class TutorialAnimationsManager : MonoBehaviour
{
    private Animator m_arrow1Anim, m_arrow2Anim, m_textAnims;
    [SerializeField] private GameObject m_arrow1, m_arrow2, m_textBox;
    private int m_stage;

    void Awake()
    {
        m_arrow1Anim = m_arrow1.GetComponent<Animator>();
        m_arrow2Anim = m_arrow2.GetComponent<Animator>();
        m_textAnims = m_textBox.GetComponent<Animator>();

        m_arrow1.SetActive(false);
        m_arrow2.SetActive(false);
    }

    public void SetStageInt(int stage)
    {
        m_stage = stage;
        m_textAnims.SetInteger("stage", stage);

        if(stage == 1)
        {
            ActivateAnims(0);
        }
    }


    public void ActivateAnims(int index)
    {
        if(m_stage == 0)
        {
            if(index == 2)
            {
                m_arrow1.SetActive(true);
                m_arrow1Anim.SetInteger("arrow", 1);
            } else if(m_arrow1.activeSelf)
            {
                m_arrow1.SetActive(false);
                m_arrow1Anim.SetInteger("arrow", 0);
            }
        } else if(m_stage == 1)
        {
            if(index == 0)
            {
                m_arrow1.SetActive(true);
                m_arrow1Anim.SetInteger("arrow", 2);

                m_arrow2.SetActive(true);
                m_arrow2Anim.SetInteger("arrow", 3);
            }
            else if (m_arrow1.activeSelf && m_arrow2.activeSelf)
            {
                m_arrow1.SetActive(false);
                m_arrow1Anim.SetInteger("arrow", 0);

                m_arrow2.SetActive(false);
                m_arrow2Anim.SetInteger("arrow", 0);
            }
        }

        m_textAnims.SetInteger("index", index);
    }

}
