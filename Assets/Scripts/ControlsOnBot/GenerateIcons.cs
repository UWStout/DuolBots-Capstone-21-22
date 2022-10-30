using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Icons;
using UnityEngine.InputSystem;


namespace DuolBots
{
    public class GenerateIcons : MonoBehaviour
    {
        /// <summary>
        /// 0 - Button South
        /// 1 - Button East
        /// 2 - Button West
        /// 3 - Button North
        /// 4 - Dpad Up
        /// 5 - Dpad Down
        /// 6 - Dpad Right
        /// 7 - Dpad Left
        /// 8 - Left Bumper
        /// 9 - Right Bumper
        /// 10 - Left Trigger
        /// 11 - Right Trigger
        /// 12 - Stick Up
        /// 13 - Stick Down
        /// 14 - Stick Right
        /// 15 - Stick Left
        /// </summary>
        [SerializeField]
        private List<IconImages> Icons;


        [SerializeField]
        private Sprite DPad;

        /// <summary>
        /// 0 - Button Fire
        /// 1 - Trigger Fire
        /// 2 - Bumper Fire
        /// 3 - Joystick Fire
        /// 4 - Rotate Right
        /// 4.5 - must manually flip for rotate left
        /// 5 -  joystick Rotate Right
        /// 5.5 - joystick Rotate left
        /// 6 - Up
        /// 6.5 - Down
        /// </summary>
        [SerializeField]
        private List<BackImages> Backs;

        [SerializeField]
        private List<Vec1ToAnalog> BreakControl;

        [SerializeField]
        private GameObject DpadOBJ;

        [SerializeField]
        private GameObject RegularOBJ;

        [SerializeField]
        private List<GameObject> m_Controllers;


        private void Start()
        {
            
        }

        /// <summary>
        /// gets list of lists of icons by reading through BuildSceneInputData with a sspecified team index
        /// </summary>
        /// <param name="_TeamIndex"></param>
        /// <returns>List<List<Gameobject>></returns>
        public List<List<GameObject>> GenerateAllIcons(byte _TeamIndex)
        {
            eSpriteRenderingTypes space = eSpriteRenderingTypes.Canvas;
            List<List<GameObject>> LLGO = new List<List<GameObject>>();
            IReadOnlyList<CustomInputBinding> AllInputs =
                BuildSceneInputData.GetInputBindingsForPlayer(_TeamIndex);
            int partIndex=-1;
            foreach (CustomInputBinding CBI in AllInputs)
            {
                //how to know if its different part
                //TODO: update this when CBI operates of slot index
                if (CBI.partSlotID != partIndex)
                {
                    LLGO.Add(new List<GameObject>());
                    partIndex = CBI.partSlotID;
                }
                List<GameObject> tempList = CreateIcons(CBI.inputType,
                    PartDatabase.instance.GetPartScriptableObject(CBI.partUniqueID).
                    actionList[CBI.actionIndex].iconType,CBI.playerIndex, space, CBI.partSlotID,
                    _TeamIndex, CBI.partUniqueID, CBI.actionIndex);

                //put objects on correct layer
                foreach (GameObject GO in tempList)
                {
                    GO.layer = 13 + CBI.playerIndex;
                    foreach (Transform child in GO.transform)
                    {
                        child.gameObject.layer = 13 + CBI.playerIndex;
                    }
                }
                //add objects to list
                LLGO[LLGO.Count - 1].AddRange(tempList);
            }
            return LLGO;
        }

        /// <summary>
        /// gets the correct icon type by going into the part database
        /// </summary>
        /// <param name="partUniqueID"></param>
        /// <param name="actionIndex"></param>
        /// <returns>eIconType</returns>
        public eIconType GetIconType(string partUniqueID, byte actionIndex)
        {
            return PartDatabase.instance.GetPartScriptableObject(partUniqueID).actionList[actionIndex].iconType;
        }

        /// <summary>
        /// creates a new icon(s) and returns them as a list
        /// </summary>
        /// <param name="_input"></param>
        /// <param name="_iconType"></param>
        /// <returns>List<GameObject></returns>
        public List<GameObject> CreateIcons(eInputType _input, eIconType _iconType, byte _playerIndex, eSpriteRenderingTypes space, byte _slotID = 0, byte _TeamIndex=0, string _ID =null, int _index = 0)
        {
            m_Controllers.AddRange(GameObject.FindGameObjectsWithTag("PlayerInput"));

            GameObject controller = m_Controllers.Find(X => X.GetComponent<PlayerIndex>().playerIndex == _playerIndex);

            List<GameObject> temp = new List<GameObject>();
            foreach (Vec1ToAnalog vA in BreakControl)
            {
                if (vA.Vec1 == _input)
                {
                    temp.Add(CreateIcon(vA.Positive, _iconType,controller, space, _playerIndex, _slotID, _TeamIndex,_ID,_index));
                    temp.Add(CreateIcon(vA.Negative, _iconType,controller, space, _playerIndex, _slotID, _TeamIndex,_ID, _index));
                    return temp;
                }
            }

            temp.Add(CreateIcon(_input, _iconType, controller, space, _playerIndex, _slotID, _TeamIndex, _ID, _index));
            return temp;
        }


        /// <summary>
        /// Overloads CreatIcons for only canvas space
        /// </summary>
        /// <param name="_input"></param>
        /// <param name="_iconType"></param>
        /// /// <param name="_playerIndex"></param>
        /// <returns>List<GameObject></returns>
        public List<GameObject> CreateIcons(eInputType _input, eIconType _iconType, byte _playerIndex)
        {
            return CreateIcons(_input, _iconType, _playerIndex, eSpriteRenderingTypes.Canvas);
        }

        /// <summary>
        /// create a singular icon gameObject
        /// </summary>
        /// <param name="_input"></param>
        /// <param name="_iconType"></param>
        /// <param name="space"></param>
        /// <returns>GameObject</returns>
        private GameObject CreateIcon(eInputType _input, eIconType _iconType,GameObject _controller, eSpriteRenderingTypes space, byte _playerIndex, byte _slotID, byte _TeamIndex,string _ID, int _index)
        {
            int index1 = 0;
            float index2 = 0f;
            foreach (IconImages II in Icons)
            {
                if (II.Input == _input) { break; }
                index1++;
            }

            switch (_iconType)
            {
                case eIconType.FireCharge:
                    if (BreakControl[4].Positive == _input || BreakControl[4].Negative == _input)
                    {
                        index2 = 1;
                    }
                    else if (BreakControl[5].Positive == _input || BreakControl[5].Negative == _input)
                    {
                        index2 = 2;
                    }
                    else if (BreakControl[0].Positive == _input || BreakControl[0].Negative == _input || BreakControl[3].Positive == _input || BreakControl[3].Negative == _input)
                    {
                        index2 = 3;
                    }
                    else
                    {
                        index2 = 0;
                    }
                    break;
                case eIconType.RaiseLower:
                    index2 = 6.5f;
                    foreach (Vec1ToAnalog vA in BreakControl)
                    {
                        if (vA.Positive == _input)
                        {
                            index2 = 6;
                            break;
                        }
                    }
                    break;
                case eIconType.Rotate:
                    index2 = 4.5f;
                    foreach (Vec1ToAnalog vA in BreakControl)
                    {
                        if (vA.Positive == _input  && !(BreakControl[0].Positive == _input || BreakControl[0].Negative == _input || BreakControl[3].Positive == _input || BreakControl[3].Negative == _input))
                        {
                            index2 = 4;
                            break;
                        }
                        else if (vA.Positive == _input && !(BreakControl[4].Positive == _input || BreakControl[4].Negative == _input))
                        {
                            index2 = 5;
                            break;
                        }
                        else if (vA.Negative == _input && !(BreakControl[4].Positive == _input || BreakControl[4].Negative == _input))
                        {
                            index2 = 5.5f;
                            break;
                        }
                    }
                    break;
                case eIconType.Turn:
                    break;
                default:
                    break;
            }

            return CombineIconWithBack(index1, index2, _controller, space, _playerIndex,_input, _slotID, _TeamIndex, _ID, _index);
        }


        /// <summary>
        /// helper function of creatIcon that puts the correct image in its place
        /// </summary>
        /// <param name="indIcon"></param>
        /// <param name="indBack"></param>
        /// <param name="space"></param>
        /// <returns>GameObject</returns>
        public GameObject CombineIconWithBack(int indIcon, float indBack, GameObject _controller, eSpriteRenderingTypes space, byte _playerIndex, eInputType _input, byte _slotID, byte _TeamIndex, string _ID, int _index)
        {
            GameObject newObj;
            string[] arrow = { "UP", "DOWN", "RIGHT", "LEFT" };

            switch (indIcon, _controller.GetComponent<PlayerInput>().currentControlScheme)
            {
                case (4,"Gamepad"):
                case (5,"Gamepad"):
                case (6,"Gamepad"):
                case (7,"Gamepad"):
                    newObj = Instantiate(DpadOBJ);
                    newObj.transform.GetChild(0).GetComponent<ManageWorldOrUI>().setUp(space);
                    newObj.transform.GetChild(0).GetComponent<ManageWorldOrUI>().setImage(DPad);
                    newObj.transform.GetChild(newObj.transform.childCount - 1).transform.rotation = newObj.transform.GetChild(0).Find(arrow[indIcon % 4]).transform.rotation;
                    newObj.transform.GetChild(newObj.transform.childCount - 1).transform.localScale = new Vector3(1, 1, 1);
                    newObj.transform.GetChild(newObj.transform.childCount - 1).transform.position = newObj.transform.GetChild(0).Find(arrow[indIcon % 4]).transform.position;
                    break;
                default:
                    newObj = Instantiate(RegularOBJ);
                    break;
            }
            foreach (ManageWorldOrUI MWOUI in newObj.GetComponentsInChildren<ManageWorldOrUI>())
            {
                MWOUI.setUp(space);
            }

            newObj.transform.GetChild(newObj.transform.childCount - 1).gameObject.GetComponent<ManageWorldOrUI>().setImage(Icons[indIcon].Image(_controller));
            if (newObj.transform.GetComponentInChildren<SpriteRenderer>())
            {
                newObj.transform.GetChild(newObj.transform.childCount - 1).gameObject.GetComponent<ManageWorldOrUI>().flipImage(direction.X);
            }

            newObj.GetComponent<ManageWorldOrUI>().setImage(Backs[(int)Mathf.Floor(indBack)].Image);
            if (indBack % 1 != 0)
            {
                switch (Backs[(int)indBack].flipDirection)
                {
                    case 1:
                        newObj.GetComponent<ManageWorldOrUI>().flipImage(direction.X);
                        break;
                    case 2:
                        newObj.GetComponent<ManageWorldOrUI>().flipImage(direction.Y);
                        break;
                    default:
                        break;
                }

            }

            IconData newIconData = newObj.GetComponent<IconData>();
            newIconData.SetPlayerIndex(_playerIndex);
            newIconData.SetInputType(_input);
            newIconData.SetSlotIndex(_slotID);
            newIconData.SetTeamIndex(_TeamIndex);
            if (_ID != null)
            {
                newIconData.SetHasCooldown(PartDatabase.instance.GetPartScriptableObject(_ID).actionList[_index].hasCooldown);
            }
            if (newObj.GetComponent<Image>())
            {
                newObj.AddComponent<RechargeAnimationOnButton>();
            }

            return newObj;
        }



        // helper structs
        [System.Serializable]
        private struct IconImages
        {
            [SerializeField]
            public Sprite Image_G;
            [SerializeField]
            public Sprite Image_K;
            [SerializeField]
            public eInputType Input;

            public Sprite Image(GameObject _controller)
            {
                switch (_controller.GetComponent<PlayerInput>().currentControlScheme)
                {
                    case "Keyboard and Mouse":
                        return Image_K;
                    case "Gamepad":
                        return Image_G;
                    default:
                        return Image_G;
                }
            }

        }

        [System.Serializable]
        private struct BackImages
        {
            [SerializeField]
            public Sprite Image;
            [SerializeField]
            public eIconType Icon;
            [SerializeField]
            public eActionType Action;
            [SerializeField]
            public int flipDirection;
        }

        [System.Serializable]
        private struct Vec1ToAnalog
        {
            [SerializeField]
            public eInputType Vec1;
            [SerializeField]
            public eInputType Positive; // Up & Right
            [SerializeField]
            public eInputType Negative; // Down & Left
        }
    }
}
