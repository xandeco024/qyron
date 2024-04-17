using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class proximityTransparency : MonoBehaviour
{
    private PlayableCharacter[] players = new PlayableCharacter[4];

    [SerializeField] float distance;    
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material transparentMaterial;


    void Start()
    {
        players = FindObjectsOfType<PlayableCharacter>();
    }

    void Update()
    {

    }

    private void ProximityTransparencyHandler()
    {
        foreach (PlayableCharacter player in players)
        {
            if (Vector3.Distance(player.transform.position, transform.position) < distance)
            {
                GetComponent<Renderer>().material = transparentMaterial;
            }
            else
            {
                GetComponent<Renderer>().material = normalMaterial;
            }
        }
    }
}
