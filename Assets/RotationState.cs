using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationState : MonoBehaviour
{
    public float rotationValue = 0;
    float rotationLerp = 0;
    Vector3 ogRot;

    private void Start() {
        ogRot = transform.localEulerAngles;
    }

    private void Update() {
        rotationLerp = Mathf.Lerp(rotationLerp, rotationValue, 12 * Time.deltaTime);
        transform.localEulerAngles = ogRot + new Vector3(0, 0, rotationLerp);
    }
}
