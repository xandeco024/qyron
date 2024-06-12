using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainInputManager : MonoBehaviour
{
    private InputMaster inputMaster;
    public InputMaster InputMaster { get => inputMaster; }
    private GameObject lockedObject;
    public GameObject LockedObject { get => lockedObject; set => lockedObject = value; }
    private GameObject selectedObject;
    private EventSystem eventSystem;

    void Awake()
    {
        inputMaster = new InputMaster();
        eventSystem = EventSystem.current;
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
