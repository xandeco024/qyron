using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Rendering.UI;

[System.Serializable]

public class EnemySpawn
{
    public GameObject enemyPrefab;
    public Vector3 spawnPoint;
    public bool join;
    public Vector3 joinPoint;
    public int wave;
    private bool spawned;
    public bool Spawned { get { return spawned; } }
    private Enemy enemy;


    public Enemy Spawn(Vector3 segmentPosition)
    {
        enemy = GameObject.Instantiate(enemyPrefab, segmentPosition + spawnPoint, Quaternion.identity).GetComponent<Enemy>();
        this.spawned = true;
        return enemy;
    }

    public void SetSpawned(bool value)
    {
        this.spawned = value;
    }
}

[System.Serializable]
class Wave {
    public int waveIndex;
    public int nextWaveUnlockAtDeathEnemies = 0;
    public List<EnemySpawn> enemySpawns;
}

public class Segment : MonoBehaviour
{
    private GameManager gameManager;
    private LevelManager levelManager;

    [SerializeField] private int index;

    public int Index { get { return index; }}

    [SerializeField] private Vector3 size;
    public Vector3 Size { get { return size; }}

    [SerializeField] private Vector3 spawnPoint;
    public Vector3 SpawnPoint { get { return spawnPoint; }}
    private List<PlayableCharacter> playersOnSegment = new List<PlayableCharacter>();
    public List<PlayableCharacter> PlayersOnSegment { get { return playersOnSegment; }}

    private BoxCollider boxCollider;

    [SerializeField] private bool complete;
    public bool Complete { get { return complete; } set { complete = value; }}

    private bool activated = false;
    public bool Activated { get { return activated; } set { activated = value; }}

    List<Enemy> enemies = new List<Enemy>();
    [SerializeField] int currentWave = 0;
    public int CurrentWave { get { return currentWave; }}
    [SerializeField] List<Wave> waves = new List<Wave>();

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.size = size;
        levelManager = FindObjectOfType<LevelManager>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {

        playersOnSegment = gameManager.PlayerList.FindAll(player => boxCollider.bounds.Contains(player.transform.position));

        if (playersOnSegment.Count > 0 && !activated && !complete)
        {
            activated = true;
        }

        if (activated && waves.Count > 0)
        {
            HandleWave();
        }
        else if (activated && (waves == null || waves.Count == 0)) // add logic to handle segment completion when there's no wave
        {
            Debug.Log("Completou " + index + " por não ter wave.");
            complete = true;
            activated = false;
        }

        playersOnSegment.ForEach(player => RestrictPlayerToSegment(player));
    }

    void HandleEnemySpawn()
    {
        foreach (EnemySpawn enemySpawn in waves[currentWave].enemySpawns)
        {
            if (!enemySpawn.Spawned)
            {
                enemies.Add(enemySpawn.Spawn(transform.position));
            }
        }
    }

    void HandleWave()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] == null)
            {
                enemies.RemoveAt(i);
                break;
            }
        }
        if (enemies.Count == 0 && currentWave == 0) 
        {
            HandleEnemySpawn();
        }

        if (waves[currentWave].nextWaveUnlockAtDeathEnemies == enemies.Count && currentWave < waves.Count - 1)
        {
            currentWave++;
            HandleEnemySpawn();
        }

        if (enemies.Count == 0 && currentWave == waves.Count - 1)
        {
            complete = true;
        }
    }

    void RestrictPlayerToSegment(PlayableCharacter player)
    {
        // só vai limitar no segmento que não está completo
        if (!complete && player.transform.position.x > transform.position.x + size.x / 2 - 1)
        {
            player.transform.position = new Vector3(transform.position.x + size.x / 2 - 1, player.transform.position.y, player.transform.position.z);
        }
        
        //só limita para voltar no segmento 0 pra ele não ir pro segmento -1 (inexistente.)
        if (index == 0 && player.transform.position.x < transform.position.x - size.x / 2 + 1)
        {
            player.transform.position = new Vector3(transform.position.x - size.x / 2 + 1, player.transform.position.y, player.transform.position.z);
        }

        // a limitação Z sempre ocorre em todo player no segmento.
        if (player.transform.position.z > transform.position.z + size.z / 2 - 1)
        {
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z + size.z / 2 - 1);
        }
        else if (player.transform.position.z < transform.position.z - size.z / 2 + 1)
        {
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z - size.z / 2 + 1);
        }
    }

    public Vector3 TranslateSpawnPoint()
    {
        return new Vector3(transform.position.x - size.x / 2 + spawnPoint.x, transform.position.y + spawnPoint.y, transform.position.z + spawnPoint.z);
    }
    public Vector3 TranslateEnemyJoinPoint(Vector3 joinPoint)
    {
        return new Vector3(transform.position.x + joinPoint.x, transform.position.y + joinPoint.y, transform.position.z + joinPoint.z);
    }
    public Vector3 TranslateEnemySpawnPoint(Vector3 spawnPoint)
    {
        return new Vector3(transform.position.x + spawnPoint.x, transform.position.y + spawnPoint.y, transform.position.z + spawnPoint.z);
    }


    void OnDrawGizmos()
    {
        foreach (Wave wave in waves)
        {
            foreach (EnemySpawn enemySpawn in wave.enemySpawns)
            {   
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(TranslateEnemySpawnPoint(enemySpawn.spawnPoint), 0.5f);
                if (enemySpawn.join)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(TranslateEnemySpawnPoint(enemySpawn.spawnPoint), TranslateEnemyJoinPoint(enemySpawn.joinPoint));
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(TranslateEnemyJoinPoint(enemySpawn.joinPoint), 0.5f);
                }
            }
        }
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(TranslateSpawnPoint(), 0.5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, size);
    }

    public void Reset()
    {
        complete = false;
        activated = false;
        currentWave = 0;
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }
        foreach (Wave wave in waves)
        {
            foreach (EnemySpawn enemySpawn in wave.enemySpawns)
            {
                enemySpawn.SetSpawned(false);
            }
        }
        enemies.Clear();
    }
}
