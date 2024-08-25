using System;
using UnityEngine;

public enum KeyType
{
    Brass, Iron, Silver, Steel, None
}

public class Key : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        GameObject brassKey = null;
        GameObject ironKey = null;
        GameObject silverKey = null;
        GameObject steelKey = null;
        Transform[] transforms = GetComponentsInChildren<Transform>();
        foreach (Transform tform in transforms)
        {
            if (tform.gameObject.name == "key1")
            {
                brassKey = tform.gameObject;
                tform.gameObject.SetActive(false);
            }
            else if (tform.gameObject.name == "key2")
            {
                ironKey = tform.gameObject;
                tform.gameObject.SetActive(false);
            }
            else if (tform.gameObject.name == "key3")
            {
                silverKey = tform.gameObject;
                tform.gameObject.SetActive(false);
            }
            else if (tform.gameObject.name == "key4")
            {
                steelKey = tform.gameObject;
                tform.gameObject.SetActive(false);
            }
            else if (tform.gameObject.name == "SpotLight")
            {
                light = tform.gameObject.GetComponent<Light>();
                light.intensity = 0.0f;
            }
        }

        switch (type)
        {
            case KeyType.Brass:
                activeKey = brassKey;
                break;
            case KeyType.Iron:
                activeKey = ironKey;
                break;
            case KeyType.Silver:
                activeKey = silverKey;
                break;
            case KeyType.Steel:
                activeKey = steelKey;
                break;
        }
        activeKey.SetActive(true);
        light.enabled = false;

        light.transform.position = transform.position + Vector3.up * 0.875f;
        light.transform.forward = Vector3.down;
    }

    private void Update()
    {
        const float INTENSITY_VELOCITY = 8.0f;
        
        float intensityDelta = INTENSITY_VELOCITY * Time.deltaTime;
        if (bLightShouldBeActive)
        {
            light.enabled = true;
            light.intensity = Mathf.Clamp(light.intensity + intensityDelta, 0.0f, maxLightIntensity);
        }
        else if (light.intensity > 0.0f)
        {
            float absRemainDelta = light.intensity;
            if (absRemainDelta <= intensityDelta)
            {
                light.intensity = 0.0f;
                light.enabled = false;
            }
            else
            {
                light.intensity -= intensityDelta;
            }
        }
    }

    public void SetLightActive(bool bValue)
    {
        bLightShouldBeActive = bValue;
    }

    public float maxLightIntensity = 6.0f;
    
    public KeyType type = KeyType.Brass;
    private GameObject activeKey;
    private new Light light;
    private bool bLightShouldBeActive;

}
