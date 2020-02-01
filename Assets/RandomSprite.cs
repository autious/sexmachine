using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    [SerializeField] SpriteRenderer renderer;
    [SerializeField] Sprite[] sprites;
    int lastFrame;

    float noseTime;
    List<Sprite> checkFrames = new List<Sprite>();

    private void Start() {
        ChangeNose();
    }

    // Update is called once per frame
    void Update()
    {
        if(noseTime > 0) {
            noseTime -= Time.deltaTime;
        } else {
            ChangeNose();
        }
    }
   
    void ChangeNose() {
        noseTime = Random.Range(0.5f, 1.2f);

        checkFrames.Clear();

        for(int i = 0; i < sprites.Length; i++) {
            if(i != lastFrame) {
                checkFrames.Add(sprites[i]);
            }
        }

        lastFrame = Random.Range(0, checkFrames.Count);
        renderer.sprite = checkFrames[lastFrame];

    }
}
