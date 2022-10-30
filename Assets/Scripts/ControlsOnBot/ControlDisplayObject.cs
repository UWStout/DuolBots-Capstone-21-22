using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DuolBots
{
    public class ControlDisplayObject : MonoBehaviour
    {
        private GameObject m_CameraObj = null;

        [SerializeField]
        private GameObject Bot = null;
        [SerializeField]
        private List<IconData> IconInfo;

        [SerializeField]
        public GameObject CameraObj => m_CameraObj;
        [SerializeField]
        float radius = 30;

        BreakControls m_BC;

        // Start is called before the first frame update
        void Start()
        {
            m_BC = new BreakControls();
            if (transform.childCount > 0)
            {

                GameObject[] Cameras = GameObject.FindGameObjectsWithTag("PlayerCamera");
                foreach (GameObject c in Cameras)
                {

                    int temp1 = transform.GetChild(0).gameObject.layer;
                    int temp2 = c.layer;

                    if (temp1 == temp2)
                    {
                        m_CameraObj = c.gameObject;
                    }
                }

                float angleBetweenIcons = (Mathf.PI) / (transform.childCount + 1);

                int indexer = 1;
                foreach (Transform t in gameObject.transform)
                {
                    t.localPosition = radius * new Vector3(Mathf.Cos(angleBetweenIcons * indexer), Mathf.Sin(angleBetweenIcons * indexer), 0);
                    indexer++;
                }
            }
            StartCoroutine(FindPart());
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (CameraObj != null)
            {
                transform.rotation = Quaternion.LookRotation(transform.position - CameraObj.transform.position);
            }
        }

        IEnumerator FindPart()
        {
            while (Bot == null)
            {
                Bot = gameObject.GetComponentInParent<FollowTargetGameObject>().m_Target.GetComponentInParent<Local_RobotInputController>().gameObject;
            }
            if (Bot.GetComponent<Local_RobotInputController>() != null)
            {
                Bot.GetComponent<Local_RobotInputController>().onPartInput += UpdatePart;
                StopCoroutine(FindPart());
            }
            yield return new WaitForEndOfFrame();
        }

        private void OnEnable()
        {
            if (Bot != null)
            {
                Bot.GetComponent<Local_RobotInputController>().onPartInput += UpdatePart;
            }
        }
        private void OnDisable()
        {
            if (Bot != null)
            {
                Bot.GetComponent<Local_RobotInputController>().onPartInput -= UpdatePart;
            }
        }

        private void UpdatePart(IReadOnlyList<CustomInputBinding> _inputs, CustomInputData _value)
        {
            if (IconInfo.Count < 1) { return;  }
            CustomInputBinding CIB = _inputs[0];
            {
                Debug.Log(CIB.playerIndex);
                Debug.Log(IconInfo[0]);
                if (CIB.playerIndex == IconInfo[0].GetPlayerIndex())
                {
                    int searchTarget = m_BC.m_Controls.FindIndex(X => X.Vec1 == CIB.inputType);
                    // not split controls
                    if (searchTarget < 0)
                    {
                        List<IconData> IconsToUpdate = IconInfo.FindAll(X => X.GetInputType() == CIB.inputType);
                        float scaleBy = .2f;
                        if (_value.isPressed == true)
                        {
                            scaleBy = .3f;
                        }
                        UpdateIconDataScale(IconsToUpdate, scaleBy);
                    }
                    else
                    {
                        List<IconData> IconsToUpdateP = IconInfo.FindAll(X => X.GetInputType() == m_BC.FindPositive(CIB.inputType));
                        float scalePBy = .2f;
                        if (_value.Get<float>()>0)
                        {
                            scalePBy = .3f;
                        }
                        UpdateIconDataScale(IconsToUpdateP, scalePBy);


                        List<IconData> IconsToUpdateN = IconInfo.FindAll(X => X.GetInputType() == m_BC.FindNegative(CIB.inputType));
                        float scaleNBy = .2f;
                        if (_value.Get<float>() < 0)
                        {
                            scaleNBy = .3f;
                        }
                        UpdateIconDataScale(IconsToUpdateN, scaleNBy);

                        if (_value.Get<float>() == 0 && _value.isPressed)
                        {
                            UpdateIconDataScale(IconsToUpdateP, .3f);
                            UpdateIconDataScale(IconsToUpdateN, .3f);
                        }
                    }

                }
            }
        }

        private void UpdateIconDataScale(List<IconData> IconsToUpdate, float scaleBy)
        {
            foreach (IconData ID in IconsToUpdate)
            {
                if (ID.transform.localScale.x < scaleBy)
                {
                    ID.gameObject.GetComponent<RechargeAnimationOnButton>().buttonPressed();
                }
                ID.transform.localScale = Vector3.one * scaleBy;
                
            }
        }


        public void UpdateChildren()
        {
            IconInfo = new List<IconData>(GetComponentsInChildren<IconData>());
        }
    }
}
