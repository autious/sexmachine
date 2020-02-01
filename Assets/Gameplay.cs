using System;
using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TalkingElement : ScriptableObject
{
    public TalkingElement rightAnswerNode;
    public TalkingElement wrongAnswerNode;
    public bool correctlyAnswered;
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
        Debug.Log("Size: "+ talkingPoints.Count);
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

    public AndroidState androidState;
    // Start is called before the first frame update
    void Start()
    {
        fullLine = "";
        readLine = "";
        //Input.
        androidState = new AndroidUpset(this);
        androidState.Enter();
    }

    // Update is called once per frame
    void Update()
    {
        androidState.Update();

        outputDia.text = readLine;

        if (!currentTalkingElement || currentTalkingElement.GoNext())
        {
            Debug.Log("HEREH: ");
            if (currentTalkingElement is Question)
            {
                Debug.Log("Ah question answer: "+ currentTalkingElement.correctlyAnswered);
                TalkingElement nextElement = null;

                if(currentTalkingElement.correctlyAnswered)
                {
                    nextElement = currentTalkingElement.rightAnswerNode;
                }
                else
                {
                    nextElement = currentTalkingElement.wrongAnswerNode;
                }

                if(nextElement != null)
                {
                    currentTalkingElement = nextElement;
                    Debug.Log("Not nUll");
                }
                else
                {
                    currentTalkingElement = AndroidStatus.GetTalkingElement();
                    Debug.Log("Next Element");
                }

            }
            else
                currentTalkingElement = AndroidStatus.GetTalkingElement();
        }

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
    protected Gameplay gameRef;
    public AndroidState(Gameplay inRef)
    {
        gameRef = inRef;
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

public class AndroidUpset : AndroidState
{
    public int questionTraverser;
    
    public AndroidUpset(Gameplay inRef):base(inRef)
    {

    }

    public override void Enter()
    {
        questionTraverser = 0;

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
        wrongText.Add("God damn you...");
        List<string> righttext = new List<string>();
        tempQuestion = new Question(questionBreadText, PlayerAnswer.Yes,wrongText,righttext);
        tempQuestion.wrongAnswerNode = tempQuestion;

        AndroidStatus.AddTalkingElement(tempQuestion);
    }

    public override void Update()
    {
      if(AndroidStatus.happniess < 10)
        {
            if (AndroidStatus.talkingPoints.Count == 0)
            {
                AndroidStatus.AddTalkingElement(gameRef.questions[questionTraverser]);

                Debug.Log("Quesitons Length" +gameRef.questions.Length);

                questionTraverser++;
                Debug.Log("Quesitons Length" + questionTraverser);

                if (gameRef.questions.Length > questionTraverser)
                    questionTraverser = 0;
            }
        }
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
        if (Math.Abs(InputManager.GetX()) <= 0.0f && Math.Abs(InputManager.GetY()) <= 0.0f )
        {
            stayStileTime += Time.deltaTime;
            if (stayStileTime > 1)
            {
                shakeCount = 0;
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

                if (shakeCount >= 4)
                {
                    shakeCount = 0;

                    return PlayerAnswer.No;
                }
            }
            //NO
            else
            {
                if (isHorizontal)
                    shakeCount = 0;
                isHorizontal = false;
                direction = InputManager.GetY();

                if (shakeCount >= 4)
                {
                    shakeCount = 0;
                    return PlayerAnswer.Yes;
                }
            }

            if ((direction + lastDirection) == 0)
                shakeCount++;

            lastDirection = direction;

        }

        return PlayerAnswer.Nothing;
    }
}