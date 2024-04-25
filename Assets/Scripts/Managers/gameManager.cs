using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> musicList;
    private int currentMusicIndex = 0;

    private AudioSource gameManagerAudioSource;

    void Start()
    {
        gameManagerAudioSource = GetComponent<AudioSource>();
        PlayMusic();
    }


    void Update()
    {

        if (!gameManagerAudioSource.isPlaying)
        {
            currentMusicIndex++;
            if (currentMusicIndex >= musicList.Count)
            {
                currentMusicIndex = 0;
            }
            PlayMusic();
        }
    }
    
    void PlayMusic()
    {
        gameManagerAudioSource.clip = musicList[currentMusicIndex];
        gameManagerAudioSource.Play();
    }

}
