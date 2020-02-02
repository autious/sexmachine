using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanlines : MonoBehaviour
{
    [SerializeField] Material scanelinesMaterial;

    [SerializeField] Vector2 scrollSpeed;
    Vector2 offset;

    // Update is called once per frame
    void Update()
    {
        offset += scrollSpeed * Time.deltaTime;
        scanelinesMaterial.mainTextureOffset = offset;
    }
}
