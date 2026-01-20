using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainInputManager : MonoBehaviour
{
    // Singleton
    public static MainInputManager Instance { get; private set; }

    private InputMaster inputMaster;
    public InputMaster InputMaster { get => inputMaster; }
    private GameObject lockedObject;
    public GameObject LockedObject { get => lockedObject; set => lockedObject = value; }
    private GameObject selectedObject;
    private EventSystem eventSystem;

    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

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
