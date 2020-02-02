using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheersGame : MonoBehaviour
{
    public GlassPosition glass_position;
    public bool won = false;
    public bool lost = false;
    public int attempts = 0;

    public void Update()
    {
        attempts = glass_position.attempt_count;
        if(lost == false) {
            if(glass_position.succeded) {
                won = true;
            }
        }

        if(won == false) {
            if(glass_position.attempt_count >= 3) {
                lost = true;
            }
        }

    }
}
