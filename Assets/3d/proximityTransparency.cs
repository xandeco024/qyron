using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class proximityTransparency : MonoBehaviour
{
    private Qyron qyron;

    [SerializeField] float distance;    
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material transparentMaterial;


    void Start()
    {
        qyron = FindObjectOfType<Qyron>();
        Debug.Log(qyron.gameObject.name);
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, qyron.transform.position) < distance)
        {
            GetComponent<Renderer>().material = transparentMaterial;
        }
        else
        {
            GetComponent<Renderer>().material = normalMaterial;
        }
    }
}
