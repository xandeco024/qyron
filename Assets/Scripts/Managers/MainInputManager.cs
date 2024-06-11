using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainInputManager : MonoBehaviour
{
    private InputMaster inputMaster;
    public InputMaster InputMaster { get => inputMaster; }
    private GameObject lockedObject;
    public GameObject LockedObject { get => lockedObject; set => lockedObject = value; }

    void Awake()
    {
        inputMaster = new InputMaster();
    }

    void OnEnable()
    {
        inputMaster.Enable();
    }

    void OnDisable()
    {
        inputMaster.Disable();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
