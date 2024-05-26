using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> musicList;
    private int currentMusicIndex = 0;

    private AudioSource audioSource;


    private bool paused;



    [SerializeField] bool movePlayersToSpawn;
    [SerializeField] private Vector3 spawnPosition;
    private List<PlayableCharacter> playerList = new List<PlayableCharacter>();
    public List<PlayableCharacter> PlayerList { get => playerList; }



    [Header("Time")]
    [SerializeField] float secondDuration;
    [SerializeField] int hours, minutes;
    public int Hours { get => hours; }
    public int Minutes { get => minutes; }



    
    void Awake()
    {
        //O nome do player que vem do lobby é Player(Clone), então se ele existir, se já tiver um player na cena, ele deve ser destruido.
        PlayableCharacter[] foundPlayers = FindObjectsOfType<PlayableCharacter>();

        bool destroyExistingPlayers = false;

        foreach (PlayableCharacter player in foundPlayers)
        {
            if (player.gameObject.name == "Player(Clone)")
            {
                destroyExistingPlayers = true;
                break;
            }
        }

        if (destroyExistingPlayers)
        {
            foreach (PlayableCharacter player in foundPlayers)
            {
                if (player.gameObject.name == "Player")
                {
                    Destroy(player.gameObject);
                }
                else
                {
                    playerList.Add(player);
                }
            }
        }
        else
        {
            playerList.AddRange(foundPlayers);
        }
    }

    void Start()
    {
        if (movePlayersToSpawn) MovePlayersToSpawn(playerList);
        audioSource = GetComponent<AudioSource>();
        PlayMusic();

        InvokeRepeating("HandleTime", 0, secondDuration);
    }


    void Update()
    {
        if (!audioSource.isPlaying)
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
        audioSource.clip = musicList[currentMusicIndex];
        audioSource.Play();
    }

    private void MovePlayersToSpawn(List<PlayableCharacter> players)
    {
        int offset = 0;
        int offsetIncrease = 2;

        for (int i = 0; i < players.Count; i++)
        {
            players[i].rb.velocity = Vector3.zero;
            players[i].transform.position = spawnPosition + new Vector3(offset, 0, 0);
            offset += offsetIncrease;
        }
    }

    void HandleTime()
    {   
        minutes++;
        if (minutes >= 60)
        {
            hours++;
            minutes = 0;
        }

        if (hours >= 24)
        {
            hours = 0;
        }
    }
}
