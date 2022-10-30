using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Icons;
using UnityEngine.UI;
// Orignal Authors - Ben

namespace DuolBots
{
    [RequireComponent(typeof(GenerateIcons))]
    public class Shared_IconManager : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        [SerializeField] private eSpriteRenderingTypes m_space;
        private GenerateIcons m_generateIcons = null;
        private ITeamIndex m_teamIndex = null;
        private float m_canvasScale = .2f;


        private void Awake()
        {
            m_teamIndex = GetComponent<ITeamIndex>();
            Assert.IsNotNull(m_teamIndex, $"{name}'s {GetType().Name} " +
                $"requires {typeof(ITeamIndex)} be attached but none was found.");

            m_generateIcons = GetComponent<GenerateIcons>();
            Assert.IsNotNull(m_generateIcons, $"{name}'s {GetType().Name} " +
                $"requires {typeof(GenerateIcons)} be attached but none was found.");
        }


        public void SetupBotIcons()
        {
            CustomDebug.Log($"{nameof(SetupBotIcons)}", IS_DEBUGGING);

            byte temp_teamIndex = m_teamIndex.teamIndex;

            List<List<GameObject>> n = m_generateIcons.GenerateAllIcons(temp_teamIndex);
            GameObject IconEnabler = new GameObject();
            IconEnabler.name = $"IconEnabler {temp_teamIndex}";
            IconEnabler.AddComponent<IconEnabling>();
            IconEnabler.AddComponent<IconEnabling>();
            foreach (List<GameObject> lgo in n)
            {
                GameObject Icons = new GameObject();
                Icons.name = $"Icons {n.IndexOf(lgo)}";
                Icons.AddComponent<FollowTargetGameObject>();

                GameObject P0Icons = new GameObject();
                P0Icons.name = "P0Icons";
                GameObject P1Icons = new GameObject();
                P1Icons.name = "P1Icons";

                P0Icons.AddComponent<ControlDisplayObject>();
                P0Icons.AddComponent<Canvas>();
                P0Icons.AddComponent<CanvasScaler>();
                P0Icons.AddComponent<GraphicRaycaster>();
                P0Icons.transform.localScale = Vector3.one* m_canvasScale;
                P0Icons.layer = 13;
                P1Icons.AddComponent<ControlDisplayObject>();
                P1Icons.AddComponent<Canvas>();
                P1Icons.AddComponent<CanvasScaler>();
                P1Icons.AddComponent<GraphicRaycaster>();
                P1Icons.layer = 14;
                P1Icons.transform.localScale = Vector3.one * m_canvasScale;
                P0Icons.transform.SetParent(Icons.transform);
                P1Icons.transform.SetParent(Icons.transform);

                P0Icons.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
                P1Icons.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
                foreach (GameObject go in lgo)
                {
                    go.transform.localScale = Vector3.one * .04f;
                    switch (go.layer)
                    {
                        case 13:
                            go.transform.SetParent(P0Icons.transform);
                            break;
                        case 14:
                            go.transform.SetParent(P1Icons.transform);
                            break;
                    }
                }

                CustomDebug.Log($"Creating {nameof(PartsOnBot)}", IS_DEBUGGING);
                PartsOnBot Pob = new PartsOnBot(temp_teamIndex);

                GameObject temp_tagetObj;
                if (n.IndexOf(lgo) != 0 && n.IndexOf(lgo)<=Pob.Slots.Count)
                {
                    temp_tagetObj = Pob.Slots[n.IndexOf(lgo) - 1];                    
                }
                else
                {
                    temp_tagetObj = Pob.Wheels;
                }
                CustomDebug.Log($"Setting target to {temp_tagetObj.name}",
                    IS_DEBUGGING);
                FollowTargetGameObject temp_followTargetScript =
                    Icons.GetComponent<FollowTargetGameObject>();
                temp_followTargetScript.SetTarget(temp_tagetObj);

                Icons.transform.parent = IconEnabler.transform;
            }

            IconEnabling[] IconEnablers = IconEnabler.GetComponents<IconEnabling>();
            IconEnablers[0].UpdateChildrenList(0);
            IconEnablers[1].UpdateChildrenList(1);
        }
    }
}
