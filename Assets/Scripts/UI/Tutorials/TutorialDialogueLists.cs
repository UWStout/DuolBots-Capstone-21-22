using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using DuolBots;

public class TutorialDialogueLists : MonoBehaviour
{
    public List<string> stage1Dialogue = new List<string>(), stage2Dialogue = new List<string>();

    void Awake()
    {
        #region stage 1
        stage1Dialogue.Add("Welcome to the battle arena! This is where you'll face other teams in fights to the inevitable destruction of your bot!");
        stage1Dialogue.Add("First you need to learn to drive. Player one will turn right while player two will turn left, both players will press forward to turn. Together you can move straight.");
        stage1Dialogue.Add("Try driving through all of the highlighted locations");
        //count 3 : last index 2
        #endregion

        #region stage 2
        stage2Dialogue.Add("Now you know how to drive, next is learning a weapon.");
        stage2Dialogue.Add("players can cycle through parts with the bumpers. The controls for a specific part will be displayed at the bottom of the screen in the center.");
        stage2Dialogue.Add("Now I placed a chassis in the arena track it down and destroy it.");
        #endregion
    }

}
