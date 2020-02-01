using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerAnswer
{
    Nothing,
    Yes,
    No
}

[CreateAssetMenu(menuName = "Dialogue")]
public class Question : TalkingElement
{
    bool allDone;
    int questionState = 0;
    int currentElement = 0;
    public List<string> breadText;
    public PlayerAnswer expectedAnswer;
    public List<string> wrongAnswer;
    public List<string> rightAnswer;

    public Question(List<string> inBreadText, PlayerAnswer inExpectedAnswer, List<string> inWrongAnswer, List<string> inRightAnswer)
    {
        breadText = inBreadText;
        this.expectedAnswer = inExpectedAnswer;
        this.wrongAnswer = inWrongAnswer;
        this.rightAnswer = inRightAnswer;
    }

    public override string GetText()
    {
     switch(questionState)
        {
            case 0:
                return breadText[currentElement] + ":" + currentElement;
                break;
            case 1:
                return breadText[currentElement] + ":" + currentElement;
                break;
            case 2:
                return wrongAnswer[currentElement] + ":" + currentElement;
                break;
            case 3:
                return rightAnswer[currentElement] + ":" + currentElement;
                break;
        }
        return "Don't go here";
    }

    public override bool GoNext()
    {
        switch (questionState)
        {
            case 0:
                if (currentElement < breadText.Count)
                {
                    if (InputManager.PushToTalk())
                    {
                        currentElement++;
                    }
                }
                else
                {
                    currentElement = 0;
                    questionState = 1;
                }
                break;
            case 1:

                PlayerAnswer answer = InputManager.yesAndNo.GetAnswer();

                if(answer != PlayerAnswer.Nothing)
                {
                    if (expectedAnswer != answer)
                    {
                        questionState = 2;
                    }
                    else
                    {
                        questionState = 3;
                    }
                }

                break;
            case 2:
                //WRONG ANSWER
                if (currentElement < wrongAnswer.Count)
                {
                    if (InputManager.PushToTalk())
                    {
                        currentElement++;
                    }
                }
                else
                {
                    currentElement = 0;
                    allDone = true;
                }
                break;
            case 3:
                //RIGHT ANSWER
                if (currentElement < rightAnswer.Count)
                {
                    if (InputManager.PushToTalk())
                    {
                        currentElement++;
                    }
                }
                else
                {
                    currentElement = 0;
                    allDone = true;
                }
                break;
        }


           

        return allDone;
    }
}
