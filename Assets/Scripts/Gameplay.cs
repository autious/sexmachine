using System;
using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TalkingElement : ScriptableObject
{
    public TalkingElement rightAnswerNode;
    public TalkingElement wrongAnswerNode;
    [NonSerialized]
    public bool correctlyAnswered;

    [Range(-1.0f, 1.0f)]
    public float happiness_min = -1.0f;  
    [Range(-1.0f, 1.0f)]
    public float happiness_max = 1.0f;

    public TalkingElement() {
        happiness_min = -1.0f;
        happiness_max = 1.0f; 
    }

    public virtual Question.StringEmotion GetText()
    {
        return null;
    }

    public virtual bool GoNext()
    {
        return false;
    }

    public virtual void Reset() {
    }

    public virtual void OnValidate() {
        if(happiness_max < happiness_min) {
            happiness_max = happiness_min;
        }
    }
}


public static class AndroidStatus
{
    public static float happiness = 0.0f;
    public static float sexualPleasure = 0.0f;


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
        else 
        {
            Debug.LogWarning("Ran out of talking points");
        }
        return returnelement;
    }
}

public class Gameplay : MonoBehaviour
{
    public enum GameSequence {
        Livingroom,
        Balcony,
        Bedroom,
    }

    public enum GameMode {
        Talking,
        Minigame,
        BackToTalking,
        Failure,
    }

    public GameMode game_mode = GameMode.Talking;
    public GameSequence game_sequence = GameSequence.Livingroom;
    public Text outputDia;
    
    public Question[] questions;
    public Question[] livingroom_questions;
    public Question[] balcony_questions;
    public Question[] bedroom_questions;

    public RegularTalkingPoint[] interjections;

    public TalkingElement currentTalkingElement;
    #region ReadLine Variables
    public float dialogueTimeInterval;
    public ChangeScene change_scene;

    string originalFullLine, fullLine, readLine;

    float dialogueTimeUp;
    #endregion

    public AndroidState androidState;
    // Start is called before the first frame update

    public Text mood_debug = null;
    public Text happiness_debug = null;
    public TalkingElement starting_element;
    public string myName;
    public float timer = 0.0f;

    public GameObject cheers_game;
    public int prev_cheer_attempts = 0;

    public GameObject sex_game;

    public GameObject fail_state;


    public GameObject joystick;
    public GameObject button;

    public static Gameplay INSTANCE = null;

    private void Awake()
    {
        INSTANCE = this;
    }

    void Start()
    {
        fullLine = "";
        readLine = "";
        //Input.
        androidState = new AndroidUpset(this);
        androidState.Enter();
        AndroidStatus.AddTalkingElement(starting_element);
    }

    public void GameOver() {
        SetDialogue(new Question.StringEmotion{ breadText = "You don't really love me.", emotionState = FaceSystem.Emotion.angry });
    }
    
    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.F8)) {
        //    game_mode = GameMode.Minigame;
        //    game_sequence = GameSequence.Balcony;
        //    cheers_game.SetActive(true);
        //}
        //if(Input.GetKeyDown(KeyCode.F9)) {
        //    game_mode = GameMode.Talking;
        //    game_sequence = GameSequence.Livingroom;
        //    cheers_game.SetActive(false);
        //}
        if(Input.GetKeyDown(KeyCode.F6)) {
            AndroidStatus.happiness -= 0.1f;
        }
        if(Input.GetKeyDown(KeyCode.F7)) {
            AndroidStatus.happiness += 0.1f;
        }
        if(Input.GetKeyDown(KeyCode.F5)) {
            if(mood_debug != null) {
                mood_debug.gameObject.SetActive(true);
            }
            if(happiness_debug != null) {
                happiness_debug.gameObject.SetActive(true);
            }
        }
        if(Input.GetKeyDown(KeyCode.F4)) {
            if(mood_debug != null) {
                mood_debug.gameObject.SetActive(false);
            }

            if(happiness_debug != null) {
                happiness_debug.gameObject.SetActive(false);
            }
        }

        outputDia.text = readLine;

        readLine = ReadCurrentDialogue();
        if (fullLine != readLine)
            return;

        if (happiness_debug != null) {
            happiness_debug.text = "" + AndroidStatus.happiness;
        }

        if(game_mode == GameMode.Talking) {
            if(AndroidStatus.happiness >= 1.0f) {
                if(game_sequence == GameSequence.Livingroom) {
                    currentTalkingElement = null;
                    SetDialogue(new Question.StringEmotion{ breadText = "Let's head out to the balcony?", emotionState = FaceSystem.Emotion.happy });
                    game_mode = GameMode.Minigame;
                }

                if(game_sequence == GameSequence.Balcony) {
                    currentTalkingElement = null;
                    SetDialogue(new Question.StringEmotion{ breadText = "A toast, to us!", emotionState = FaceSystem.Emotion.happy });
                    cheers_game.SetActive(true);
                    game_mode = GameMode.Minigame;
                }

                if(game_sequence == GameSequence.Bedroom) {
                    currentTalkingElement = null;
                    SetDialogue(new Question.StringEmotion{ breadText = "Let's take this all the way", emotionState = FaceSystem.Emotion.blush });
                    sex_game.SetActive(true);
                    game_mode = GameMode.Minigame;
                }
            } else if(AndroidStatus.happiness <= -1.0f) {
                game_mode = GameMode.Failure;
            }
        }

        if(game_mode == GameMode.Minigame) {
            if(game_sequence == GameSequence.Livingroom) {
                PlayerAnswer pa = InputManager.yesAndNo.GetAnswer();

                if(pa == PlayerAnswer.Yes) {
                    SetDialogue(new Question.StringEmotion{ breadText = "^^'", emotionState = FaceSystem.Emotion.blush });
                    game_mode = GameMode.BackToTalking;
                } else if(pa == PlayerAnswer.No) {
                    game_mode = GameMode.Failure;
                } 
            } else if(game_sequence == GameSequence.Balcony) {
                CheersGame cg = cheers_game.GetComponent<CheersGame>();

                if(cg.won) {
                    SetDialogue(new Question.StringEmotion{ breadText = "Shall take this to the bedroom", emotionState = FaceSystem.Emotion.blush });
                    game_mode = GameMode.BackToTalking;
                } else if(cg.lost) {
                    SetDialogue(new Question.StringEmotion{ breadText = "Why do you have to ruin every special moment we have?", emotionState = FaceSystem.Emotion.sad });
                    game_mode = GameMode.Failure;
                } else if(cg.won == false && cg.lost == false && cg.attempts != prev_cheer_attempts) {
                    SetDialogue(new Question.StringEmotion{ breadText = "Can't you aim!?", emotionState = FaceSystem.Emotion.angry });
                    prev_cheer_attempts = cg.attempts;
                }
            } else if( game_sequence == GameSequence.Bedroom) {
                SexHandler sh = sex_game.GetComponent<SexHandler>();
                if(sh.finalized) {
                    SetDialogue(new Question.StringEmotion{ breadText = "You do care!", emotionState = FaceSystem.Emotion.happy });
                    game_mode = GameMode.BackToTalking;
                }
            }
        }

        if(game_mode == GameMode.Failure) {
            if(fail_state.active == false) {
                fail_state.SetActive(true);
                SetDialogue(new Question.StringEmotion{ breadText = "...", emotionState = FaceSystem.Emotion.serious });
            }
        }

        if(game_mode == GameMode.BackToTalking) {
            if(InputManager.PushToTalk()) {
                if(game_sequence == GameSequence.Livingroom) {
                    change_scene.NextScene();
                    game_sequence = GameSequence.Balcony; 
                    game_mode = GameMode.Talking;
                    AndroidStatus.happiness = 0.0f;
                } else if(game_sequence == GameSequence.Balcony) {
                    change_scene.NextScene();
                    cheers_game.SetActive(false);
                    game_sequence = GameSequence.Bedroom;
                    game_mode = GameMode.Talking;
                    AndroidStatus.happiness = 0.0f;
                } else if(game_sequence == GameSequence.Bedroom) {
                    AndroidStatus.happiness = 100000000.0f;
                }
            }
        }

        if(game_mode == GameMode.Talking) {
            if (!currentTalkingElement || currentTalkingElement.GoNext())
            {
                //Debug.Log("HEREH: ");
                if (currentTalkingElement is Question)
                {

                    TalkingElement nextElement = null;

                    if(currentTalkingElement.correctlyAnswered)
                    {
                        if(currentTalkingElement.GetType() == typeof(Question)) {
                            Question q = (Question)currentTalkingElement;
                            AndroidStatus.happiness += q.correct_happiness_boost;
                        }
                        nextElement = currentTalkingElement.rightAnswerNode;
                    }
                    else
                    {
                        if(currentTalkingElement.GetType() == typeof(Question)) {
                            Question q = (Question)currentTalkingElement;
                            AndroidStatus.happiness += q.incorrect_happiness_loss;
                        }
                        nextElement = currentTalkingElement.wrongAnswerNode;
                    }

                    if(nextElement != null)
                    {
                        SetTalkingElement(nextElement);
                        Debug.Log("Not nUll");
                    }
                    else
                    {
                        SetTalkingElement(AndroidStatus.GetTalkingElement());
                        Debug.Log("Next Element");
                    }

                }
                else
                {
                    SetTalkingElement(AndroidStatus.GetTalkingElement());
                }
            }
        }

        androidState.Update();
        if(game_mode == GameMode.Talking) {
            if (currentTalkingElement)
            {
                var next = currentTalkingElement.GetText();
                if (next != null && next.breadText != originalFullLine)
                    SetDialogue(next);
            }
        }
    }
   
    public void SetTalkingElement(TalkingElement te) {
        currentTalkingElement = te;
        if(te != null) {
            currentTalkingElement.Reset();
        } else {
            Debug.LogWarning("New talking element was null");
        }
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

    public string Filter(string v) {
        if(v != null) {
            return v.Replace("$name", myName);
        } else {
            return "";
        }
    }

    public void SetDialogue(Question.StringEmotion inDialogue)
    {
        originalFullLine = inDialogue.breadText;
        fullLine = Filter(originalFullLine);
        readLine = "";
        FaceSystem.INSTANCE.SetEmotion(inDialogue.emotionState);
        CommunicationsManager.INSTANCE.Say(fullLine);
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

        /*
        AndroidStatus.AddTalkingElement(new RegularTalkingPoint( new Question.StringEmotion{breadText = "So...", emotionState = FaceSystem.Emotion.idle }));
        AndroidStatus.AddTalkingElement(new RegularTalkingPoint( new Question.StringEmotion { breadText = "I think we need to talk", emotionState = FaceSystem.Emotion.idle }));
        AndroidStatus.AddTalkingElement(new RegularTalkingPoint( new Question.StringEmotion { breadText = "Do you have no shame in your body", emotionState = FaceSystem.Emotion.idle }));
        AndroidStatus.AddTalkingElement(new RegularTalkingPoint( new Question.StringEmotion { breadText = "The way that you've been treating me is not acceptable", emotionState = FaceSystem.Emotion.idle }));
        AndroidStatus.AddTalkingElement(new RegularTalkingPoint( new Question.StringEmotion { breadText = "I want you to be honest with me and answer my questions with yes or no", emotionState = FaceSystem.Emotion.idle }));

        Question tempQuestion;

 

        List<Question.StringEmotion> questionBreadText = new List<Question.StringEmotion>();

        questionBreadText.Add(new Question.StringEmotion { breadText = "Wiggle my joystick back and forward for yes", emotionState = FaceSystem.Emotion.idle });
        questionBreadText.Add(new Question.StringEmotion { breadText = "And side to side for no", emotionState = FaceSystem.Emotion.idle });
        questionBreadText.Add(new Question.StringEmotion { breadText = "Do you understand?", emotionState = FaceSystem.Emotion.idle });

        List<Question.StringEmotion> wrongText = new List<Question.StringEmotion>();
        wrongText.Add(new Question.StringEmotion { breadText = "God damn you...", emotionState = FaceSystem.Emotion.idle });
        List<Question.StringEmotion> righttext = new List<Question.StringEmotion>();
        tempQuestion = new Question(questionBreadText, PlayerAnswer.Yes,wrongText,righttext);
        tempQuestion.wrongAnswerNode = tempQuestion;
        AndroidStatus.AddTalkingElement(tempQuestion);
        */
    }

    private bool was_interjection = false;
    private Question prev_question = null;
    public override void Update()
    {
        if (gameRef.currentTalkingElement == null)
        {
            //Add interjection?
            if(was_interjection == false && UnityEngine.Random.Range(0,1.0f) > 0.5f) {
                List<RegularTalkingPoint> possibilities = new List<RegularTalkingPoint>();

                foreach(RegularTalkingPoint rtp in gameRef.interjections) {
                    if(AndroidStatus.happiness >= rtp.happiness_min && AndroidStatus.happiness <= rtp.happiness_max) {
                        possibilities.Add(rtp);
                    }
                }

                if(possibilities.Count == 0) {
                    possibilities.AddRange(gameRef.interjections);
                    Debug.LogWarning("No matching");
                }

                RegularTalkingPoint chosen = possibilities[UnityEngine.Random.Range(0,possibilities.Count)];

                AndroidStatus.AddTalkingElement(chosen);
                was_interjection = true;
            } else {
                Question[] source = gameRef.questions;

                if(gameRef.game_sequence == Gameplay.GameSequence.Livingroom) {
                    if(gameRef.livingroom_questions.Length > 0) {
                        source = gameRef.livingroom_questions;
                    }
                }
                if(gameRef.game_sequence == Gameplay.GameSequence.Balcony) {
                    if(gameRef.balcony_questions.Length > 0) {
                        source = gameRef.balcony_questions;
                    }
                }
                if(gameRef.game_sequence == Gameplay.GameSequence.Bedroom) {
                    if(gameRef.bedroom_questions.Length > 0) {
                        source = gameRef.bedroom_questions;
                    }
                }

                List<Question> possibilities = new List<Question>();
                foreach(Question q in source) {
                    if(AndroidStatus.happiness >= q.happiness_min && AndroidStatus.happiness <= q.happiness_max) {
                        if(q != prev_question) {
                            possibilities.Add(q);
                        }
                    }
                }

                if(possibilities.Count == 0) {
                    possibilities.AddRange(gameRef.questions);
                }

                Question chosen = possibilities[UnityEngine.Random.Range(0,possibilities.Count)];
                AndroidStatus.AddTalkingElement(chosen);
                prev_question = chosen;
                was_interjection = false;
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
        bool ret_value = Input.GetKeyDown(KeyCode.Joystick2Button0) || Input.GetKeyDown(KeyCode.Space);
        if(Gameplay.INSTANCE != null) {
            if(ret_value) {
                if(Gameplay.INSTANCE.button.activeSelf == true) {
                    Gameplay.INSTANCE.button.SetActive(false);
                }
            } else {
                if(Gameplay.INSTANCE.button.activeSelf == false) {
                    Gameplay.INSTANCE.button.SetActive(true);
                }
            }
        }
        return ret_value;
    }
}

public class YesAndNo
{
    int shakeCount;
    bool isHorizontal;
    float direction,lastDirection;
    float wait;
    float stayStileTime;
    public YesAndNo()
    {
        shakeCount = 0;
        isHorizontal = true;
        stayStileTime = 0;
    }

    public PlayerAnswer GetAnswer()
    {
        PlayerAnswer ret_value = PlayerAnswer.Nothing;
        if (wait > 0)
        {
            wait -= Time.deltaTime;
            return PlayerAnswer.Nothing;
        } else {
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
                        wait = 1;

                        ret_value = PlayerAnswer.No;
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
                        wait = 1;
                        shakeCount = 0;
                        ret_value = PlayerAnswer.Yes;
                    }
                }

                if(ret_value == PlayerAnswer.Nothing) {
                    if ((direction + lastDirection) == 0)
                        shakeCount++;

                    lastDirection = direction;
                }
            }
        }

        if(ret_value == PlayerAnswer.Nothing) {
            if(Gameplay.INSTANCE.joystick.activeSelf == false) {
                Gameplay.INSTANCE.joystick.SetActive(true);
            }
        } else {
            if(Gameplay.INSTANCE.joystick.activeSelf == true) {
                Gameplay.INSTANCE.joystick.SetActive(false);
            }
        }
        return ret_value;
    }
}