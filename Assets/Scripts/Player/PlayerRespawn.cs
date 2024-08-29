using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public ParticleSystem respawnEffect;
    public AudioSource respawnAudio;
    
    /// <summary>
    /// Play the Respawn Audio and Particles
    /// </summary>
    public void RespawnEffects()
    {
        respawnEffect.Play();
        respawnAudio.Play();
    }
}
