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

    public string voice = "ms-us1";
    public int pitch = 50;
    public int range = 50;
    public int rate = 200;
    public int wordgap = 10;
    //public int capitals = 0;
    public int intonation = 0;

    public AudioSource happy_source;
    public AudioSource sad_source;
    public AudioSource aroused_source;
    public AudioSource curious_source;
    public AudioSource disgusted_source;
    public AudioSource angry_source;

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
    }

    void Say(string text) {
        Speech.instance.QueueMessage(new Speech.IncomingMessage{ type = Speech.IncomingMessageType.SetVoice, message = voice });
        Speech.instance.QueueMessage(new Speech.IncomingMessage{ type = Speech.IncomingMessageType.SetPitch, param1 = pitch });
        Speech.instance.QueueMessage(new Speech.IncomingMessage{ type = Speech.IncomingMessageType.SetRange, param1 = range });
        Speech.instance.QueueMessage(new Speech.IncomingMessage{ type = Speech.IncomingMessageType.SetRate, param1 = rate });
        Speech.instance.QueueMessage(new Speech.IncomingMessage{ type = Speech.IncomingMessageType.SetWordGap, param1 = wordgap });
        Speech.instance.QueueMessage(new Speech.IncomingMessage{ type = Speech.IncomingMessageType.SetIntonation, param1 = intonation });
        //Speech.instance.QueueMessage(new Speech.IncomingMessage{ type = Speech.IncomingMessageType.SetCapitals, param1 = capitals });
        Speech.instance.Say(text, VoiceGeneratedCallback);
    }

    void VoiceGeneratedCallback(string line, AudioClip data) {
        said_line.text = line;
        happy_source.clip = data;
        happy_source.Play();
    }
}
