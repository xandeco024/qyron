using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{

    [Header("Qyron")]
    private GameObject qyron;
    private qyronMovement qyronMovement;
    private qyronCombat qyronCombat;
    //private Rigidbody2D qyronRB;
    //private SpriteRenderer qyronSR;
    //private BoxCollider2D qyronCol;

    //private qyronSFX qyronSFX;
    //private Animator qyronAnimator;

    [Header("Manager")]
    private bool isDead = false;

    [SerializeField] private List<AudioClip> musicList;
    private int currentMusicIndex = 0;

    private AudioSource gameManagerAudioSource;

    void Start()
    {
        qyron = GameObject.FindWithTag("Player");
        qyronMovement = qyron.GetComponent<qyronMovement>();
        qyronCombat = qyron.GetComponent<qyronCombat>();

        gameManagerAudioSource = GetComponent<AudioSource>();
        PlayMusic();
    }


    void Update()
    {
        if (qyronCombat.GetCurrentHealth() <= 0)
        {
            isDead = true;
            gameManagerAudioSource.Stop();
        }

        if (!gameManagerAudioSource.isPlaying && !isDead)
        {
            currentMusicIndex++;
            if (currentMusicIndex >= musicList.Count)
            {
                currentMusicIndex = 0;
            }
            PlayMusic();
        }
    }

    public bool GetIsDead()
    {
        return isDead;
    }

    void PlayMusic()
    {
        gameManagerAudioSource.clip = musicList[currentMusicIndex];
        gameManagerAudioSource.Play();
    }

}
