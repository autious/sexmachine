using System;
using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TalkingElement : ScriptableObject
{

    public virtual string GetText()
    {
        return "";
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
}

public static class AndroidStatus
{
    public static float happniess;
    public static float sexualPleasue;


    public static List<TalkingElement> talkingPoints = new List<TalkingElement>();

    public static void AddTalkingElementy(TalkingElement inElement)
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
    public float inputXOut;
    public float inputYOut;

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
        inputXOut = InputManager.GetX();
        inputYOut = InputManager.GetY();
        outputDia.text = readLine;

        if (!currentTalkingElement || InputManager.PushToTalk())
            currentTalkingElement = AndroidStatus.GetTalkingElement();

        if (currentTalkingElement)
        {
            if (currentTalkingElement.GetText() != fullLine)
                SetDialogue(currentTalkingElement.GetText());
        }



        readLine = ReadCurrentDialogue();
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if(Input.GetKeyDown(kcode))
                Debug.Log("KeyCode down: " + kcode);
        }
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
        AndroidStatus.AddTalkingElementy(new RegularTalkingPoint("So..."));
        AndroidStatus.AddTalkingElementy(new RegularTalkingPoint("I think we need to talk"));
        AndroidStatus.AddTalkingElementy(new RegularTalkingPoint(" Do you have no shame in your body"));
        AndroidStatus.AddTalkingElementy(new RegularTalkingPoint("The way that you've been treating me is not acceptable"));
        AndroidStatus.AddTalkingElementy(new RegularTalkingPoint("I want you to be honest with me and answer my questions with yes or no"));
        AndroidStatus.AddTalkingElementy(new RegularTalkingPoint("Wiggle my joystick back and forward for yes"));
        AndroidStatus.AddTalkingElementy(new RegularTalkingPoint("And side to side for no"));
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
    public static float GetX()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public static float GetY()
    {
        return Input.GetAxisRaw("Vertical");
    }

    public static bool IsForward()
    {
        return Input.GetAxisRaw("Vertical") > 0;
    }

    public static bool IsBackward()
    {
        return Input.GetAxisRaw("Vertical") < 0;
    }

    public static bool PushToTalk()
    {
        return Input.GetKeyDown(KeyCode.Joystick2Button0);
    }
}