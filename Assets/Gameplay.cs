using System;
using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public static class AndroidStatus
{
    public static float happniess;
    public static float sexualPleasue;

    public static string currentDialogue;
}

public class Gameplay : MonoBehaviour
{
    public float inputXOut;
    public float inputYOut;

    public Text outputDia;


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

        if (AndroidStatus.currentDialogue != fullLine)
            SetDialogue(AndroidStatus.currentDialogue);


        readLine = ReadCurrentDialogue();
        //foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        //{
        //    Debug.Log("KeyCode down: " + kcode);
        //}
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
        AndroidStatus.currentDialogue = "FUUUCKU";
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
}