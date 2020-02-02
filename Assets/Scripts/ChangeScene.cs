using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] SpriteRenderer renderer;
    [SerializeField] Sprite[] rooms;

    Vector3 ogScale;

    int currentScene = 0;

    private void Awake() {
        ogScale = transform.localScale;
        NextScene();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.S)) {
            NextScene();
        }

        transform.localScale = Vector3.Lerp(transform.localScale, ogScale, 5 * Time.deltaTime);

    }

    public void NextScene() {
        if(currentScene < rooms.Length) {
            renderer.sprite = rooms[currentScene];
            currentScene++;
            transform.localScale = new Vector3(1.2f, 0.8f, 1);
        }
    }
}
