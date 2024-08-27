using System.Collections;
using System.Collections.Generic;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mathf = UnityEngine.Mathf;

struct DoorRef
{
    public GameObject door;
    public bool bSet;
}

public enum DoorState
{
    Closed, Opening, Open, Closing
}

public class Door : MonoBehaviour
{
    void Start()
    {
        _ui = UIController.Instance;
        
        soundSource = gameObject.AddComponent<AudioSource>();
        soundSource.spatialize = true;
        soundSource.spatialBlend = 1.0f;
        soundSource.volume = 0.8f;
        
        doors = new DoorRef[2];
        rotationLimits = new float[6];
        startRotationYaw = new float[2];

        Transform tform1 = null, tform2 = null;
        if (transform.childCount > 0)
        {
            tform1 = transform.GetChild(0);
            if (transform.childCount > 1)
            {
                tform2 = transform.GetChild(1);
            }
        }

        if (tform1 != null)
        {
            doors[0].door = tform1.gameObject;
            doors[0].bSet = true;
        }

        if (tform2 != null)
        {
            doors[1].door = tform2.gameObject;
            doors[1].bSet = true;
        }

        for (int i = 0; i < 2; ++i)
        {
            if (!doors[i].bSet)
            {
                continue;
            }

            float refYaw = doors[i].door.transform.localRotation.y;
            rotationLimits[i * 3] = refYaw;
            rotationLimits[i * 3 + 1] = NormalizeAxis(i == 0 ? refYaw + targetOpenYaw : refYaw - targetOpenYaw);
            rotationLimits[i * 3 + 2] = NormalizeAxis(i == 0 ? refYaw - targetOpenYaw : refYaw + targetOpenYaw);
        }

        bShouldTick = false;
    }

    public void ShowInteractUI()
    {
        if (!bShowingUI || (interactionCount > 2 && !bUpdatedLockedInteractDisplay))
        {
            if (interactionCount > 2)
            {
                bUpdatedLockedInteractDisplay = true;
                _ui.ShowInteract("Enter (locked)");
            }
            else
            {
                _ui.ShowInteract("Enter");
            }
            bShowingUI = true;
        }
    }

    public void HideInteractUI()
    {
        if (bShowingUI)
        {
            _ui.HideInteract();
            bShowingUI = false;
        }
    }

    void Update()
    {
        if (hackySceneChangeTimer > 0.0f)
        {
            hackySceneChangeTimer += Time.deltaTime;
            if (hackySceneChangeTimer > 1.0f)
            {
                SceneManager.LoadScene(loadSceneNumber);
            }
        }
        if (!bShouldTick || bPauseRotate) return;
        switch (state)
        {
        case DoorState.Opening:
            UpdateOpening();
            break;
        case DoorState.Closing:
            UpdateClosing();
            break;
        }
    }

    public DoorInteractResult TryToggleOpen(Player p)
    {
        interactionCount += 1;
        if (lockedByKey != KeyType.None)
        {
            if (!p.inventory.KeyOwned[(int)lockedByKey])
            {
                return DoorInteractResult.LockedInteraction;
            }
        }
        if (bMoves)
        {
            ToggleOpen(p.transform.position);
            return state == DoorState.Closing ? DoorInteractResult.Closing : DoorInteractResult.Opening;
        }
        else
        {
            if (hackySceneChangeTimer == 0.0f)
            {
                hackySceneChangeTimer = 0.01f;
            }
        }
        return DoorInteractResult.Opening;
    }

    public void ToggleOpen(Vector3 fromPoint)
    {
        if (state == DoorState.Open)
        {
            Close();
        }
        else
        {
            OpenAwayFrom(fromPoint);
        }
        SetStartRotations();
    }

    void SetStartRotations()
    {
        rotationTimer = 0.0f;
        if (doors[0].bSet)
        {
            startRotationYaw[0] = doors[0].door.transform.localRotation.eulerAngles.y;
        }
        if (doors[1].bSet)
        {
            startRotationYaw[1] = doors[1].door.transform.localRotation.eulerAngles.y;
        }
    }

    public void SetPaused(bool bValue)
    {
        bPauseRotate = bValue;
    }

    public void TogglePaused()
    {
        bPauseRotate = !bPauseRotate;
    }

    void Close()
    {
        state = DoorState.Closing;
        bOpenAfterClose = false;
        bShouldTick = true;
    }

    void OpenAwayFrom(Vector3 point)
    {
        Vector3 toPointScaled = point - transform.position;
        Vector3 forwardDir = transform.forward;
        float scaledCosTheta = Vector3.Dot(toPointScaled, forwardDir);
        bool bWasPositiveMotion = bPositiveMotion;
        if (scaledCosTheta > 0.0f)
        {
            bPositiveMotion = true;
        }
        else
        {
            bPositiveMotion = false;
        }

        bOpenAfterClose = false;
        if (state == DoorState.Closed)
        {
            state = DoorState.Opening;
        }
        else if (bWasPositiveMotion != bPositiveMotion)
        {
            state = DoorState.Closing;
            bOpenAfterClose = true;

        }

        bShouldTick = true;
    }

    void UpdateOpening()
    {
        bool[] bFinishedRotation = new bool[2];
        rotationTimer += Time.deltaTime;
        for (int i = 0; i < 2; ++i)
        {
            if (!doors[i].bSet)
            {
                bFinishedRotation[i] = true;
                continue;
            }

            float targetYaw = bPositiveMotion ? rotationLimits[i * 3 + 1] : rotationLimits[i * 3 + 2];
            bFinishedRotation[i] = RotateDoor(doors[i].door, startRotationYaw[i], targetYaw);
        }

        if (bFinishedRotation[0] && bFinishedRotation[1])
        {
            state = DoorState.Open;
            bShouldTick = false;
        }
    }

    void UpdateClosing()
    {
        bool[] bFinishedRotation = new bool[2];
        rotationTimer += Time.deltaTime;
        for (int i = 0; i < 2; ++i)
        {
            if (!doors[i].bSet)
            {
                bFinishedRotation[i] = true;
                continue;
            }
            bFinishedRotation[i] = RotateDoor(doors[i].door, startRotationYaw[i],rotationLimits[i * 3]);
        }

        if (bFinishedRotation[0] && bFinishedRotation[1])
        {
            if (bOpenAfterClose)
            {
                state = DoorState.Opening;
            }
            else
            {
                state = DoorState.Closed;
                bShouldTick = false;
            }
        }
    }

    float SigmoidOnRange(float Value, float Min, float Max)
    {
        const float SIGMOID_MAX_VALUE = 9.0f;
        float BaseInput = Value - Min;
        float BaseMax = Max - Min;
        float ScaleRatio = (2.0f * SIGMOID_MAX_VALUE) / BaseMax;
        float Input = (BaseInput * ScaleRatio) - SIGMOID_MAX_VALUE;
        return 1.0f / (1.0f + Mathf.Exp(-Input));
    }

    bool RotateDoor(GameObject door, float startYaw, float targetYaw)
    {
        float normVal = SigmoidOnRange(rotationTimer, 0.0f, timeToRotate);
        bool bNearEnough = normVal >= 0.999f;
        if (bNearEnough)
        {
            door.transform.localRotation = Quaternion.AngleAxis(targetYaw, Vector3.up);
            return true;
        }
        float epsilon = NormalizeAxis(targetYaw - startYaw);
        float newAxisVal = NormalizeAxis(startYaw + normVal * epsilon);
        door.transform.localRotation = Quaternion.AngleAxis(newAxisVal, Vector3.up);
        return false;
    }

    // pulled from Unreal Engine (do not sell a game with this code in it)
    private float ClampAxis(float angle)
    {
        // returns Angle in the range (-360,360)
        angle %= 360.0f;
        if (angle < 0.0f)
        {
            // shift to [0,360) range
            angle += 360.0f;
        }

        return angle;
    }

    // pulled from Unreal Engine (do not sell a game with this code in it)
    private float NormalizeAxis(float angle)
    {
        // returns Angle in the range [0,360)
        angle = ClampAxis(angle);
        if (angle > 180.0f)
        {
            // shift to (-180,180]
            angle -= 360.0f;
        }

        return angle;
    }

    public bool bMoves = false;
    public int loadSceneNumber = 3;
    
    public AudioSource soundSource;
    public AudioClip interactClip;
    public AudioClip openClip;
    public AudioClip closeClip;
    public AudioClip fullyClosedClip;
    
    public DoorState state;
    public float targetOpenYaw = 90.0f;
    public float timeToRotate = 2.0f;
    public KeyType lockedByKey = KeyType.None;

    private UIController _ui;
    private DoorRef[] doors;
    private float[] rotationLimits;
    private float[] startRotationYaw;
    private float rotationTimer;
    private bool bPositiveMotion;
    private bool bOpenAfterClose;
    private bool bPauseRotate;
    private bool bShouldTick;
    private bool bShowingUI;
    private int interactionCount;
    private bool bUpdatedLockedInteractDisplay;
    private float hackySceneChangeTimer;

}