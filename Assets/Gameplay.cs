using System;
using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TalkingElement : ScriptableObject
{
    public TalkingElement rightAnswerNode;
    public TalkingElement wrongAnswerNode;

    public virtual string GetText()
    {
        return "";
    }
    public virtual bool GoNext()
    {
        return false;
    }
}

public class RegularTalkingPoint: TalkingElement
{
    string Text;

    public RegularTalkingPoint(string textIn)
    {
        Text = textIn;
    }

    public override string GetText()
    {
        return Text;
    }

    public override bool GoNext()
    {
        return InputManager.PushToTalk();
    }
}

public static class AndroidStatus
{
    public static float happniess;
    public static float sexualPleasue;


    public static List<TalkingElement> talkingPoints = new List<TalkingElement>();

    public static void AddTalkingElement(TalkingElement inElement)
    {
        talkingPoints.Add(inElement);
    }

    public static TalkingElement GetTalkingElement()
    {
        TalkingElement returnelement = null;

        if (talkingPoints.Count != 0)
        {
            returnelement = talkingPoints[0];
            talkingPoints.RemoveAt(0);
        }
        return returnelement;
    }
}

public class Gameplay : MonoBehaviour
{
    public Text outputDia;

    public Question[] questions;

    TalkingElement currentTalkingElement;

    #region ReadLine Variables
    public float dialogueTimeInterval;

    string fullLine, readLine;

    float dialogueTimeUp;
    #endregion

    AndroidState androidState;
    // Start is called before the first frame update
    void Start()
    {
        fullLine = "";
        readLine = "";
        //Input.
        androidState = new AndroidUpset();
        androidState.Enter();
    }

    // Update is called once per frame
    void Update()
    {
        androidState.Update();

        outputDia.text = readLine;

        if (!currentTalkingElement || currentTalkingElement.GoNext())
            currentTalkingElement = AndroidStatus.GetTalkingElement();

        if (currentTalkingElement)
        {
            if (currentTalkingElement.GetText() != fullLine)
                SetDialogue(currentTalkingElement.GetText());
        }

        readLine = ReadCurrentDialogue();

    }

    public void ChangeState(AndroidState changeToState)
    {
        androidState.Exit();
        androidState = changeToState;
        androidState.Enter();
    }

    public string ReadCurrentDialogue()
    {
        if(fullLine.Length <= readLine.Length)
        {
            return fullLine;
        }
        dialogueTimeUp += Time.deltaTime;


        if (dialogueTimeUp > dialogueTimeInterval)
        {
            dialogueTimeUp = 0;
            readLine += fullLine[readLine.Length];
        }
        return readLine;
    }

    public void SetDialogue(string inDialogue)
    {
        fullLine = inDialogue;
        readLine = "";
    }
}

#region Emotionalstates

public class AndroidState
{

    public AndroidState()
    {

    }

    public virtual void Enter()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void Exit()
    {

    }
}

public class AndroidUpset: AndroidState
{
    public override void Enter()
    {
        AndroidStatus.AddTalkingElement(new RegularTalkingPoint("So..."));
        AndroidStatus.AddTalkingElement(new RegularTalkingPoint("I think we need to talk"));
        AndroidStatus.AddTalkingElement(new RegularTalkingPoint(" Do you have no shame in your body"));
        AndroidStatus.AddTalkingElement(new RegularTalkingPoint("The way that you've been treating me is not acceptable"));
        AndroidStatus.AddTalkingElement(new RegularTalkingPoint("I want you to be honest with me and answer my questions with yes or no"));

        Question tempQuestion;

 

        List<string> questionBreadText = new List<string>();

        questionBreadText.Add("Wiggle my joystick back and forward for yes");
        questionBreadText.Add("And side to side for no");
        questionBreadText.Add("Do you understand?");

        List<string> wrongText = new List<string>();
        wrongText.Add("Butthole Idiot");
        List<string> righttext = new List<string>();
        righttext.Add("THAT'S GOOD");
        tempQuestion = new Question(questionBreadText, PlayerAnswer.Yes,wrongText,righttext);
        tempQuestion.wrongAnswerNode = tempQuestion;

        AndroidStatus.AddTalkingElement(tempQuestion);
    }

    public override void Update()
    {
       // base.Update();
    }

    public override void Exit()
    {
       // base.Exit();
    }


}

#endregion

public static class InputManager
{
    public static YesAndNo yesAndNo = new YesAndNo();

    public static float GetX()
    {
        return Input.GetAxis("Vertical");
    }

    public static float GetY()
    {
        return Input.GetAxis("Horizontal");
    }

    public static bool IsForward()
    {
        return Input.GetAxis("Vertical") > 0;
    }

    public static bool IsBackward()
    {
        return Input.GetAxis("Vertical") < 0;
    }

    public static PlayerAnswer answer()
    {
        return yesAndNo.GetAnswer();// Input.GetAxis("Vertical") < 0;
    }

    public static bool PushToTalk()
    {
        return Input.GetKeyDown(KeyCode.Joystick2Button0);
    }
}

public class YesAndNo
{
    int shakeCount;
    bool isHorizontal;
    float direction,lastDirection;

    float stayStileTime;
    public YesAndNo()
    {
        shakeCount = 0;
        isHorizontal = true;
        stayStileTime = 0;
    }

    public PlayerAnswer GetAnswer()
    {

        Debug.Log(shakeCount);
        if (Math.Abs(InputManager.GetX()) <= 0.0f && Math.Abs(InputManager.GetY()) <= 0.0f )
        {
            stayStileTime += Time.deltaTime;
            if (stayStileTime > 1)
            {
                shakeCount = 0;
                Debug.Log("WABABABA");
            }
        }
        else
        {
            stayStileTime = 0;
            // Debug.Log("IN HERE");
            //YES
            if (Math.Abs(InputManager.GetX()) > Math.Abs(InputManager.GetY()))
            {
                if (!isHorizontal)
                    shakeCount = 0;

                isHorizontal = true;
                direction = InputManager.GetX();

                if (shakeCount >= 8)
                    return PlayerAnswer.No;
            }
            //NO
            else
            {
                if (isHorizontal)
                    shakeCount = 0;
                isHorizontal = false;
                direction = InputManager.GetY();

                if (shakeCount >= 8)
                    return PlayerAnswer.Yes;
            }

            if ((direction + lastDirection) == 0)
                shakeCount++;

            lastDirection = direction;

        }

        return PlayerAnswer.Nothing;
    }
}