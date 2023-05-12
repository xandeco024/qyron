using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class qyronSFX : MonoBehaviour
{
    private AudioSource qyronAudioSource;

    [SerializeField] private AudioClip[] ataques;
    [SerializeField] private AudioClip[] miss;
    [SerializeField] private AudioClip[] movement;

    void Start()
    {
        qyronAudioSource = GetComponent<AudioSource>();
    }


    void Update()
    {
        
    }

    public void PlayAttackSFX(int attackSFXIndex)
    {
        qyronAudioSource.PlayOneShot(ataques[attackSFXIndex]);
    }

    public void PlayMissSFX(int missionSFXIndex)
    {
        qyronAudioSource.PlayOneShot(miss[missionSFXIndex]);
    }

    public void PlayMovementSFX(int movementSFXIndex) 
    {
        qyronAudioSource.PlayOneShot(movement[movementSFXIndex]);
    }
}
