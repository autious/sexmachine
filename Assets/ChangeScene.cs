using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] SpriteRenderer renderer;
    [SerializeField] Sprite[] rooms;

    int currentScene = 0;

    private void Awake() {
        NextScene();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.S)) {
            NextScene();
        }
    }

    public void NextScene() {
        if(currentScene < rooms.Length) {
            renderer.sprite = rooms[currentScene];
            currentScene++;
        }
    }
}
