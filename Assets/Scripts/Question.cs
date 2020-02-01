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
    public List<StringEmotion> breadText;
    public PlayerAnswer expectedAnswer;
    public List<StringEmotion> wrongAnswer;
    public List<StringEmotion> rightAnswer;

    [System.Serializable]
    public class StringEmotion {
        [TextArea] public string breadText;
        public FaceSystem.Emotion emotionState;
    }

    public Question(List<StringEmotion> inBreadText, PlayerAnswer inExpectedAnswer, List<StringEmotion> inWrongAnswer, List<StringEmotion> inRightAnswer)
    {
        breadText = inBreadText;
        this.expectedAnswer = inExpectedAnswer;
        this.wrongAnswer = inWrongAnswer;
        this.rightAnswer = inRightAnswer;
        allDone = false;
        questionState = 0;
        currentElement = 0;
    }


    public override StringEmotion GetText()
    {
        StringEmotion toRead = null;
     switch(questionState)
        {
            case 0:
                if (currentElement < breadText.Count)
                    toRead =  breadText[currentElement];
                break;
            case 1:
                if (currentElement < breadText.Count)
                    toRead =  breadText[currentElement];
                break;
            case 2:
                if (currentElement < wrongAnswer.Count)
                    toRead = wrongAnswer[currentElement];
                break;
            case 3:
                if(currentElement < rightAnswer.Count)
                    toRead = rightAnswer[currentElement];
                break;
        }

        return toRead;
    }

    public override bool GoNext()
    {
        allDone = false;
        switch (questionState)
        {
            case 0:
                if (currentElement == breadText.Count-1)
                {
                    currentElement++;
                }
                else if (currentElement < breadText.Count)
                {
                    if (InputManager.PushToTalk())
                    {
                        currentElement++;
                    }
                }

                if(currentElement >= breadText.Count)
                {
                    currentElement = breadText.Count - 1;
                    questionState = 1;
                }
                break;
            case 1:

                PlayerAnswer answer = InputManager.yesAndNo.GetAnswer();
                if (answer != PlayerAnswer.Nothing)
                {
                    if (expectedAnswer != answer)
                    {
                        questionState = 2;
                        correctlyAnswered = false;
                    }
                    else
                    {
                        questionState = 3;
                        correctlyAnswered = true;
                    }
                    currentElement = 0;
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


        if(allDone)
        {
            currentElement = 0;
            questionState = 0;
        }

        return allDone;
    }
}
