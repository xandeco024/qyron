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

        if (movePlayersToSpawn)
        {
            foreach (PlayableCharacter player in playerList)
            {
                player.rb.velocity = Vector3.zero;
                player.transform.position = spawnPosition;
            }
        }
    }

    void Start()
    {


        audioSource = GetComponent<AudioSource>();
        PlayMusic();
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

    private void SetCameraFollow()
    {
        
    }

}
