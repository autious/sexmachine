using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerAnswer
{
    Yes,
    No
}

[CreateAssetMenu(menuName = "Dialogue")]
public class Question : TalkingElement
{
    int currentLement;
    public List<string> breadText;
    public PlayerAnswer inExpectedAnswer;
    public List<string> wrongAnswer;
    public List<string> rightAnswer;


    public override string GetText()
    {
        return "";// breadText.;
    }
}
