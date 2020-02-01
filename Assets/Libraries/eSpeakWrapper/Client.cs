﻿// https://github.com/parhamdoustdar/espeak-ng-wrapper

using System;
using System.Runtime.InteropServices;

#pragma warning disable 0414

namespace ESpeakWrapper
{
    public class Client
    {
        enum AudioOutput
        {
            Playback,
            Retrieval,
            Synchronous,
            SynchronousPlayback
        };

        enum Error
        {
            EE_OK = 0,
            EE_INTERNAL_ERROR = -1,
            EE_BUFFER_FULL = 1,
            EE_NOT_FOUND = 2
        }

        enum PositionType
        {
            Character = 1,
            Word = 2,
            Sentence = 3
        }

        enum Parameter
        {
            Rate = 1,
            Volume = 2,
            Pitch = 3,
            Range = 4,
            Punctuation = 5,
            Capitals = 6,
            WordGap = 7,
            Intonation = 9,
        }

        enum ParameterType
        {
            Absolute = 0,
            Relative = 1
        }

        [Flags]
        enum SpeechFlags
        {
            CharsUtf8 = 1,
            SSML = 0x10,
        }

        static bool Initialized = false;

        public static int sampleRate;

        public static void Initialize(string path)
        {
            //var result = espeak_Initialize(AudioOutput.SynchronousPlayback, 0, path, 0);
            var result = espeak_Initialize(AudioOutput.Retrieval, 0, path, 0); // TODO allow receiving audio data..
            if (result == (int)Error.EE_INTERNAL_ERROR)
            {
                throw new Exception(string.Format("Could not initialize ESpeak. Maybe there is no espeak data at {0}?", path));
            } else {
                sampleRate = result;
            }

            espeak_SetSynthCallback(EventHandler.Handle);

            Initialized = true;
        }

        public static bool VoiceFinished() {
            bool ret = false;
            EventHandler.audio_files_mutex.WaitOne();
            if(EventHandler.audio_files.Count > 0) {
                ret = true; 
            }
            EventHandler.audio_files_mutex.ReleaseMutex();
            return ret;
        }

        public static byte[] PopVoice() {
            byte[] ret = null;
            EventHandler.audio_files_mutex.WaitOne();
            if(EventHandler.audio_files.Count > 0) {
                ret = EventHandler.audio_files[0];
                EventHandler.audio_files.RemoveAt(0);
            }
            EventHandler.audio_files_mutex.ReleaseMutex();
            return ret;
            
        }

        public static bool SetRate(int rate)
        {
            if (rate < 80 && rate > 450)
            {
                throw new Exception("The rate must be between 80 and 450.");
            }

            Error result = espeak_SetParameter(Parameter.Rate, rate, ParameterType.Absolute);
            return CheckResult(result);
        }

        public static bool SetRange(int range) {
            if(range < 0) {
                range = 0;
            } 
            if(range > 100) {
                range = 100;
            }

            Error result = espeak_SetParameter(Parameter.Range, range, ParameterType.Absolute);
            return CheckResult(result);
        }

        public static bool SetPitch(int pitch) {
            if(pitch < 0) {
                pitch = 0;
            }
            if(pitch > 100) {
                pitch = 100;
            }

            Error result = espeak_SetParameter(Parameter.Pitch, pitch, ParameterType.Absolute);
            return CheckResult(result);
        }

        public static bool SetWordgap(int wordgap) {
            if(wordgap < 0) {
                wordgap = 0;
            }
            Error result = espeak_SetParameter(Parameter.WordGap, wordgap, ParameterType.Absolute);
            return CheckResult(result);
        }

        public static bool SetVolume(int volume) {
            if(volume < 0) {
                volume = 0;
            }
            if(volume > 200) {
                volume = 200;
            }
            Error result = espeak_SetParameter(Parameter.Volume, volume, ParameterType.Absolute);
            return CheckResult(result);
        }

        public static bool SetIntonation(int intonation) {
            Error result = espeak_SetParameter(Parameter.Intonation, intonation, ParameterType.Absolute);
            return CheckResult(result);
        }

        public static bool SetCapitals(int capitals) {
            Error result = espeak_SetParameter(Parameter.Capitals, capitals, ParameterType.Absolute);
            return CheckResult(result);
        }

        static bool CheckResult(Error result)
        {
            if (result == Error.EE_OK)
            {
                return true;
            }
            else if (result == Error.EE_BUFFER_FULL)
            {
                return false;
            }
            else if (result == Error.EE_INTERNAL_ERROR)
            {
                throw new Exception("Internal error in ESpeak.");
            }
            else
            {
                return false;
            }
        }

        public static bool Speak(string text)
        {
            var result = espeak_Synth(text, text.Length * Marshal.SystemDefaultCharSize);
            return CheckResult(result);
        }

        public static bool SpeakSSML(string text)
        {
            var result = espeak_Synth(text, text.Length * Marshal.SystemDefaultCharSize, 0, PositionType.Character, 0, SpeechFlags.CharsUtf8 | SpeechFlags.SSML);
            return CheckResult(result);
        }

        public static bool Stop()
        {
            var result = espeak_Cancel();
            return CheckResult(result);
        }

        public static bool SetVoiceByName(string name)
        {
            var result = espeak_SetVoiceByName(name);
            return CheckResult(result);
        }

        public static Voice GetCurrentVoice()
        {
            IntPtr espeakVoicePtr = espeak_GetCurrentVoice();
            ESpeakVoice espeakVoice = (ESpeakVoice)Marshal.PtrToStructure(espeakVoicePtr, typeof(ESpeakVoice));

            if (espeakVoice.Equals(default(ESpeakVoice)))
            {
                throw new Exception("eSpeak returned an empty voice object. Did you call one of the ESpeak.SetVoice*() functions?");
            }

            return new Voice()
            {
                Name = espeakVoice.Name,
                Languages = espeakVoice.Languages.Substring(1),
                Priority = (int)espeakVoice.Languages[0],
                Identifier = espeakVoice.Identifier,
            };
        }

        [DllImport("libespeak-ng.dll", CharSet = CharSet.Ansi)]
        static extern Error espeak_SetVoiceByName([MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [DllImport("libespeak-ng.dll", CharSet = CharSet.Ansi)]
        static extern Error espeak_SetParameter(Parameter parameter, int value, ParameterType type);

        [DllImport("libespeak-ng.dll", CharSet = CharSet.Ansi)]
        static extern IntPtr espeak_GetCurrentVoice();

        [DllImport("libespeak-ng.dll", CharSet = CharSet.Ansi)]
        static extern Error espeak_Synth([MarshalAs(UnmanagedType.LPUTF8Str)] string text, int size, uint startPosition = 0, PositionType positionType = PositionType.Character, uint endPosition = 0, SpeechFlags flags = SpeechFlags.CharsUtf8, UIntPtr uniqueIdentifier = default(UIntPtr), IntPtr userData = default(IntPtr));

        [DllImport("libespeak-ng.dll", CharSet = CharSet.Ansi)]
        static extern int espeak_Initialize(AudioOutput output, int bufferLength, string path, int options);

        [DllImport("libespeak-ng.dll", CharSet = CharSet.Ansi)]
        static extern Error espeak_Cancel();

        [DllImport("libespeak-ng.dll", CharSet = CharSet.Ansi)]
        static extern void espeak_SetSynthCallback(EventHandler.SynthCallback callback);
    }
}
