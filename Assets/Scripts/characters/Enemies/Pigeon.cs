using System.Collections;
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

    void Update()
    {
        target = FindTargetOnRange(players, targetRange);
    }

    override protected void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
}
