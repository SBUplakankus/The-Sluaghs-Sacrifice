using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public ParticleSystem respawnEffect;
    public AudioSource respawnAudio;
    public ScreenFade screenFade;
    private UIController _ui;
    private bool _hintOpen;
    private const float HintTimer = 4f;
    private float _hintCooldown;

    private void Start()
    {
        _ui = UIController.Instance;
        _hintOpen = false;
    }

    private void Update()
    {
        if (!_hintOpen) return;
        _hintCooldown -= Time.deltaTime;
        if (_hintCooldown > 0) return;
        _hintOpen = false;
        _ui.HideHint();

    }

    /// <summary>
    /// Play the Respawn Audio and Particles
    /// </summary>
    public void RespawnEffects()
    {
        
        _hintCooldown = HintTimer;
        _hintOpen = true;
        screenFade.RespawnFade();
        respawnEffect.Play();
        respawnAudio.Play();
        _ui.ShowHint("The Sluagh caught you");
        Debug.Log("Show");
    }
}
