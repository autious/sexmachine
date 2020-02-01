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
    AudioSource current_source = null;

    void Start() {
    }

    float prev_amplitude = 0.0f;
    float[] amplitude_sample_buffer = new float[256];
    // Update is called once per frame
    void Update() {
        SpeakSystem ss = GameObject.FindObjectOfType<SpeakSystem>();
        if(current_source != null && current_source.isPlaying) { 
            float pos = current_source.time;
            AudioClip clip = current_source.clip;

            int frequency = clip.frequency;

            int current_sample_pos = (int)(pos*frequency);

            if(current_sample_pos + amplitude_sample_buffer.Length < clip.samples) {
                clip.GetData(amplitude_sample_buffer, current_sample_pos);

                float sum = 0.0f;
                for(int i = 0; i < amplitude_sample_buffer.Length; i++) {
                    sum += UnityEngine.Mathf.Abs(amplitude_sample_buffer[i]);
                }
                sum /= amplitude_sample_buffer.Length;

                if(sum > prev_amplitude * 1.4f) {
                    if(ss != null) {
                        if(ss.IsSpeaking == false) {
                            ss.DoSpeak();
                        }
                    }
                }
                prev_amplitude = sum; 
            }
        } else {
            prev_amplitude = 0;
        }
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

        //happy_source.Stop();
        //sad_source.Stop();
        //blush_source.Stop();
        //idle_source.Stop();
        //angry_source.Stop();
        if(current_source != null) {
            current_source.Stop();
        }

        current_source = GetMoodSource(current_mood);
        current_source.clip = data;
        current_source.Play();
    }
}
