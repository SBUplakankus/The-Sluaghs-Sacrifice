using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class HangingBody : MonoBehaviour
{
    private void Start()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>();
        foreach (var t in transforms)
        {
            if (t.gameObject.name == "rotatepivot")
            {
                swingPivot = t;
            }
        }
        swingAmplitude = 4.0f;
        swingTime = Random.Range(0.0f, 3.0f);
    }

    // Update is called once per frame
    void Update()
    {
        const float LOSE_AMPLITUDE_SPEED = 3.0f;

        swingAmplitude -= Time.deltaTime * LOSE_AMPLITUDE_SPEED;
        swingAmplitude = Mathf.Max(swingAmplitude, 4.0f);
        swingRotTarget = Door.NormalizeAxis(Mathf.Sin(swingTime) * swingAmplitude);
        
        Quaternion orientation = swingPivot.transform.localRotation;
        float swingRot = Door.NormalizeAxis(orientation.eulerAngles.x);
        float targetDiff = Door.NormalizeAxis(swingRotTarget - swingRot);
        swingRot += (targetDiff - swingRot) * 0.2f;
        
        orientation.eulerAngles = new Vector3(swingRot, 0.0f, 0.0f);
        swingPivot.transform.localRotation = orientation;
        swingTime += Time.deltaTime;
    }

    public void BeginSwing(float amplitude=50.0f)
    {
        swingAmplitude = amplitude;
    }

    private Transform swingPivot;
    private float swingAmplitude;
    private float swingTime;
    private float swingRotTarget;
}
