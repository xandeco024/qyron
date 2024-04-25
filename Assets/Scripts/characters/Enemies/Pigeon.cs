using UnityEngine;

public class Pigeon : Enemy
{
    void Awake()
    {
        GetComponentsOnCharacter();
    }

    void Start()
    {
        players = FindObjectsOfType<PlayableCharacter>();
        SetStats();
    }

    private void OnDrawGizmos()
    {

    }
}
