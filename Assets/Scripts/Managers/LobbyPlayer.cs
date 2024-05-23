using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyPlayer : MonoBehaviour
{
    [SerializeField] private PlayableCharacter[] characters;
    private PlayableCharacter selectedCharacter;
    public PlayableCharacter linkedCharacter;
    private int selectedCharacterIndex;
    private bool ready;
    public bool Ready { get => ready;}


    [Header("Components")]
    private Animator animator;
    private Image characterImage;
    private GameObject joinGameObject;
    private PlayerInput playerInput;
    private InputMaster inputMaster;

    void Start()
    {
        animator = GetComponent<Animator>();
        joinGameObject = transform.Find("Join").gameObject;
    }

    void Update()
    {

    }

    public void NextCharacter(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            selectedCharacterIndex++;
            if (selectedCharacterIndex >= characters.Length)
            {
                selectedCharacterIndex = 0;
            }
            selectedCharacter = characters[selectedCharacterIndex];

            animator.SetFloat("blend", selectedCharacterIndex);
        }
    }

    public void PreviousCharacter(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            selectedCharacterIndex--;
            if (selectedCharacterIndex < 0)
            {
                selectedCharacterIndex = characters.Length - 1;
            }
            selectedCharacter = characters[selectedCharacterIndex];

            animator.SetFloat("blend", selectedCharacterIndex);
        }
    }

    public void ToggleReady()
    {
        ready = !ready;
    }

    public void JoinPlayer(PlayerInput playerInput)
    {
        gameObject.AddComponent<PlayerInput>();
        this.playerInput.actions = playerInput.actions;
        this.playerInput.SwitchCurrentActionMap(playerInput.currentActionMap.name);

        animator.SetBool("empty", false);
        joinGameObject.SetActive(false);
    }

    public void LeavePlayer()
    {
        animator.SetBool("empty", true);
        joinGameObject.SetActive(true);
    }
}
