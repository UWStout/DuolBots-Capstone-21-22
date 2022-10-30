using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DuolBots
{
    public class BackgroundVideoManager : MonoBehaviour
    {
        private Material m_material;

        private void Awake()
        {
            RawImage rawImage = GetComponent<RawImage>();
            m_material = new Material(rawImage.material);
            rawImage.material = m_material;
        }

        // DO a courtine to gradually buildup
        public void MainMenuMaterialStats()
        {
            //Blur Value
            StartCoroutine("UnBlur");
            //m_material.SetColor("_Tint", new Color(0f, 0f, 0f, 0f));
        }

        public void SubMenuMaterialStats()
        {
            // Blur Value
            StartCoroutine("Blur");
            //m_material.SetColor("_Tint", new Color(0f, 0f, 0f, 150f));
        }

        IEnumerator Blur()
        {
            for (float blur = 0f; blur <= 0.004; blur += 0.001f)
            {
                m_material.SetFloat("_BlurValue", blur);
                yield return new WaitForSeconds(.1f);
            }
        }
        IEnumerator UnBlur()
        {
            for (float blur = 0.004f; blur >= 0; blur -= 0.001f)
            {
                m_material.SetFloat("_BlurValue", blur);
                yield return new WaitForSeconds(.1f);
            }
        }

    }
}
