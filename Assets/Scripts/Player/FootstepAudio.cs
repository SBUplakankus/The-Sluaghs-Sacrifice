using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class FootstepAudio : MonoBehaviour
{
    public AudioClip[] footstepSounds;
    private AudioSource _audioSource;
    private PlayerMovement _playerMovement;

    public enum FloorType
    {
        Wood,
        Gravel
    };

    public FloorType currentFloor;

    private Rigidbody _controller; 

    private const float StepInterval = 0.65f; // Time between steps
    private float _nextStepTime = 0f; // Timer to track when to play the next step

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _controller = GetComponent<Rigidbody>();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        var speed = _controller.velocity.magnitude;
        if (speed < 0.5f) return;
        if (!(Time.time >= _nextStepTime)) return;
        PlayFootstep();
        _nextStepTime = Time.time + StepInterval;
    }

    private void PlayFootstep()
    {
        _audioSource.clip = currentFloor switch
        {
            FloorType.Gravel => footstepSounds[0],
            FloorType.Wood => footstepSounds[1],
            _ => _audioSource.clip
        };

        _audioSource.pitch = UnityEngine.Random.Range(0.5f, 0.7f);
        _audioSource.Play();
    }
}
