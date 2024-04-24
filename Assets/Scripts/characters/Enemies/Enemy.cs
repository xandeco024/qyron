using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    Character[] targets = new Character[4];

    void Start()
    {
        // Find all targets in the scene
        Character[] foundTargets = FindObjectsOfType<Character>();
        
    }

    void Update()
    {
        
    }
}
