using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityLibrary;

public class CommunicationsManager : MonoBehaviour {

    public static CommunicationsManager INSTANCE;
    private void Awake() {
        INSTANCE = this;
    }

    public Text said_line;
    public Text mood_text;

    public AudioSource happy_source;
    public AudioSource sad_source;
    public AudioSource blush_source;
    public AudioSource idle_source;
    public AudioSource angry_source;

    FaceSystem.Emotion current_mood = FaceSystem.Emotion.idle;

    void Start() {
    }

    // Update is called once per frame
    void Update() {
        //if(Input.GetKeyDown(KeyCode.W)) { 
        //    Say("Why don't you love me anymore?");
        //}
        //if(Input.GetKeyDown(KeyCode.S)) {
        //    Say("That's the sweetest thing i've ever heard");
        //}

        //if(Input.GetKeyDown(KeyCode.A)) {
        //    Say("Oh, baby, just like that");
        //}

        //if(Input.GetKeyDown(KeyCode.D)) {
        //    Say("Remember the time we used to have, in italy?");
        //}

        //if(Input.GetKeyDown(KeyCode.P)) {
        //    FaceSystem.Emotion[] moods = (FaceSystem.Emotion[]) System.Enum.GetValues(typeof(FaceSystem.Emotion));
        //    SetMood(moods[UnityEngine.Random.Range(0,moods.Length)]);
        //}
    }

    public void SetMood(FaceSystem.Emotion mood) {
        current_mood = mood;
        mood_text.text = current_mood.ToString();
    }

    public AudioSource GetMoodSource(FaceSystem.Emotion mood) {
        switch(mood) {
            case FaceSystem.Emotion.angry:
                return angry_source;
            case FaceSystem.Emotion.blush:
                return blush_source;
            case FaceSystem.Emotion.idle:
                return idle_source;
            case FaceSystem.Emotion.happy:
                return happy_source;
            case FaceSystem.Emotion.sad:
                return sad_source;
        }
        return angry_source;
    }

    public void Say(string text) {
        VoiceMood vm = GetMoodSource(current_mood).GetComponent<VoiceMood>();
        if(vm != null) {
            Speech.instance.QueueMessage(new Speech.IncomingMessage{ type = Speech.IncomingMessageType.SetVoice, message = vm.voice });
            Speech.instance.QueueMessage(new Speech.IncomingMessage{ type = Speech.IncomingMessageType.SetPitch, param1 = vm.pitch });
            Speech.instance.QueueMessage(new Speech.IncomingMessage{ type = Speech.IncomingMessageType.SetRange, param1 = vm.range });
            Speech.instance.QueueMessage(new Speech.IncomingMessage{ type = Speech.IncomingMessageType.SetRate, param1 = vm.rate });
            Speech.instance.QueueMessage(new Speech.IncomingMessage{ type = Speech.IncomingMessageType.SetWordGap, param1 = vm.wordgap });
            Speech.instance.QueueMessage(new Speech.IncomingMessage{ type = Speech.IncomingMessageType.SetIntonation, param1 = vm.intonation });
            //Speech.instance.QueueMessage(new Speech.IncomingMessage{ type = Speech.IncomingMessageType.SetCapitals, param1 = vm.capitals });
        }
        Speech.instance.Say(text, VoiceGeneratedCallback);
    }

    void VoiceGeneratedCallback(string line, AudioClip data) {
        //said_line.text = line;
        AudioSource source = GetMoodSource(current_mood);
        source.clip = data;
        source.Play();
    }
}
