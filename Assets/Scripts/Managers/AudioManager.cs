using System;
using UnityEngine;

public enum AudioClipRef
{
    KeyPickup_Strings,
    KeyPickup_Foreboding,
    KeyPickup_Whispers,
    DoorInteract
}

public enum AudioSequence
{
    None,
    DoorOpen,
    DoorClose
}

public class AudioManager : MonoBehaviour
{
    void Start()
    {
        pickupSource = gameObject.AddComponent<AudioSource>();
        pickupSource.volume = 0.5f;
    }

    void Update()
    {
        UpdateDoorSequence();
    }

    void UpdateDoorSequence()
    {
         if (doorSequence != AudioSequence.None)
         {
             if (!managingDoor.soundSource.isPlaying)
             {
                 doorSequenceStage += 1;
                 if (doorSequenceStage == 1)
                 {
                     if (doorSequence == AudioSequence.DoorOpen)
                     {
                         managingDoor.soundSource.clip = managingDoor.openClip;
                     }
                     else
                     {
                         managingDoor.soundSource.clip = managingDoor.closeClip;
                     }
                     managingDoor.soundSource.Play();
                 }
                 else if (doorSequenceStage == 2)
                 {
                      if (doorSequence == AudioSequence.DoorOpen)
                      {
                          doorSequence = AudioSequence.None;
                      }
                      else
                      {
                          managingDoor.soundSource.clip = managingDoor.fullyClosedClip;
                          managingDoor.soundSource.Play();
                      }
                 }
                 else
                 {
                     doorSequence = AudioSequence.None;
                 }
             }
         }        
    }
    
    public void PlaySequence(GameObject obj, AudioSequence sequence)
    {
        switch (sequence)
        {
        case AudioSequence.DoorOpen:
        case AudioSequence.DoorClose:
            managingDoor = obj.GetComponentInParent<Door>();
            doorSequence = sequence;
            doorSequenceStage = 0;
            managingDoor.soundSource.clip = managingDoor.interactClip;
            managingDoor.soundSource.Play();
            break;
        }
    }

    public void PlayClip(AudioClipRef audioRef, GameObject obj=null)
    {
        AudioSource source = null;
        AudioClip clip = null;
        switch (audioRef)
        {
        case AudioClipRef.KeyPickup_Strings:
            source = pickupSource;
            clip = keyPickupStrings;
            break;
        case AudioClipRef.KeyPickup_Foreboding:
            source = pickupSource;
            clip = keyPickupForeboding;
            break;
        case AudioClipRef.KeyPickup_Whispers:
            source = pickupSource;
            clip = keyPickupWhispers;
            break;
        case AudioClipRef.DoorInteract:
            if (obj != null)
            {
                Door door = obj.transform.GetComponentInParent<Door>();
                door.soundSource.clip = door.interactClip;
                door.soundSource.Play();
            }
            break;
        }

        if (source != null)
        {
            source.clip = clip;
            source.Play();
        }
    }
    
    private AudioSource pickupSource;
    
    private Door managingDoor;
    private AudioSequence doorSequence = AudioSequence.None;
    private int doorSequenceStage;
    
    [SerializeField] private AudioClip keyPickupStrings;
    [SerializeField] private AudioClip keyPickupForeboding;
    [SerializeField] private AudioClip keyPickupWhispers;
}