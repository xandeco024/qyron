using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Segment : MonoBehaviour
{
    private LevelManager levelManager;

    [SerializeField] private int index;

    public int Index { get { return index; }}

    [SerializeField] private Vector3 size;
    public Vector3 Size { get { return size; }}

    [SerializeField] private bool complete;
    public bool Complete { get { return complete; } set { complete = value; }}

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    void Update()
    {
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, size);
    }
}
