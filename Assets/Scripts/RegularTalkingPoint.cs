using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "Regular Talking Point")]
public class RegularTalkingPoint: TalkingElement {
    
    public List<Question.StringEmotion> Text;
    int currentElement = 0;

    public RegularTalkingPoint() {
        currentElement = 0;
        happiness_min = -1.0f;
        happiness_max = 1.0f;
        if(Text == null) {
            Text = new List<Question.StringEmotion>();
        }
    }

    public override void Reset()
    {
        base.Reset();
        currentElement = 0;
    }

    public RegularTalkingPoint(Question.StringEmotion textIn)
    {
        currentElement = 0;
        happiness_min = -1.0f;
        happiness_max = 1.0f;
        if(Text == null) {
            Text = new List<Question.StringEmotion>();
        }
        Text.Add(textIn);
    }

    public override Question.StringEmotion GetText()
    {
        if(currentElement >= 0 && currentElement < Text.Count) {
            return Text[currentElement];
        } else {
            return null;
        }
    }

    public override bool GoNext()
    {
        if(InputManager.PushToTalk()) {
            currentElement++;
            if(currentElement >= Text.Count) {
                return true;
            }
        }
        return false;
    }
}
