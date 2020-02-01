using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityLibrary;

public class CommunicationsManager : MonoBehaviour {
    public enum Mood {
        Happy,
        Sad,
        Aroused,
        Curious,
        Angry,
        Disgusted,
    }

    public Text said_line;
    public Text mood_text;

    public AudioSource happy_source;
    public AudioSource sad_source;
    public AudioSource aroused_source;
    public AudioSource curious_source;
    public AudioSource disgusted_source;
    public AudioSource angry_source;


    Mood current_mood = Mood.Happy;

    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.W)) { 
            Say("Why don't you love me anymore?");
        }
        if(Input.GetKeyDown(KeyCode.S)) {
            Say("That's the sweetest thing i've ever heard");
        }

        if(Input.GetKeyDown(KeyCode.A)) {
            Say("Oh, baby, just like that");
        }

        if(Input.GetKeyDown(KeyCode.D)) {
            Say("Remember the time we used to have, in italy?");
        }

        if(Input.GetKeyDown(KeyCode.P)) {
            Mood[] moods = (Mood[]) System.Enum.GetValues(typeof(Mood));
            SetMood(moods[UnityEngine.Random.Range(0,moods.Length)]);
        }
    }

    void SetMood(Mood mood) {
        current_mood = mood;
        mood_text.text = current_mood.ToString();
    }

    public AudioSource GetMoodSource(Mood mood) {
        switch(mood) {
            case Mood.Angry:
                return angry_source;
            case Mood.Aroused:
                return aroused_source;
            case Mood.Curious:
                return curious_source;
            case Mood.Disgusted:
                return disgusted_source;
            case Mood.Happy:
                return happy_source;
            case Mood.Sad:
                return sad_source;
        }
        return angry_source;
    }

    void Say(string text) {
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
        said_line.text = line;
        AudioSource source = GetMoodSource(current_mood);
        source.clip = data;
        source.Play();
    }
}
