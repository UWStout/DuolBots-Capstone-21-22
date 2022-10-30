using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DuolBots;
public class RechargeAnimationOnButton : MonoBehaviour
{
    [SerializeField]

    Color color1 = new Color(26, 26, 26, .8f);
    Color color2 = new Color(255, 255, 255, .8f);

    private List<Image> Images;
    private IconData ID = null;
    private CooldownRemaining CDR = null;
    private PartsOnBot POB = null;
    private void Start()
    {
        Images = new List<Image>(GetComponentsInChildren<Image>());
        Images.Insert(0, GetComponent<Image>());
        ID = GetComponent<IconData>();
        POB = new PartsOnBot(ID.GetTeamIndex());
    }

    private void FixedUpdate()
    {
        if (CDR == null || !ID.GetHasCooldown()) { return; }
        Debug.Log(CDR.coolDown);
        SetImagePercent();
    }

    public void buttonPressed()
    {
        if (CDR == null && ID.GetHasCooldown())
        {
            StartCoroutine(FindCorrectCooldownRemaining());
        }
    }

    private void SetImagePercent()
    {
        foreach (Image i in Images)
        {
            i.fillAmount = CDR.coolDown;
        }
    }

    private IEnumerator FindCorrectCooldownRemaining()
    {
        while (CDR==null) {
            CooldownRemaining _cdr = POB.Slots[ID.GetSlotIndex()].GetComponent<CooldownRemaining>();
            if (_cdr != null && _cdr.inputType == ID.GetInputType())
            {
                CDR = _cdr;
            }
            yield return new WaitForEndOfFrame();
        }
    }

}
