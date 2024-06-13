using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyPlayer : MonoBehaviour
{
    private bool ready;
    public bool Ready => ready;

    private CharacterData characterData;
    public CharacterData CharacterData => characterData;

    private int selectedCharacterIndex;

    private GameObject playerFrameObject;
    private GameObject toJoinObject;
    private GameObject characterObject;
    private Animator playerFrameAnimator;
    private Animator characterAnimator;
    private LobbyManager lobbyManager;
    private PlayableCharacter playableCharacter;

    void Awake()
    {
        lobbyManager = FindObjectOfType<LobbyManager>();
        playableCharacter = GetComponent<PlayableCharacter>();

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            GetComponent<PlayerInput>().SwitchCurrentActionMap("Lobby");
        }
        else
        {
            //if enabled, switch to player action map
            PlayerInput pi = GetComponent<PlayerInput>();
            if (pi.currentActionMap.name == "Lobby")
            {
                pi.SwitchCurrentActionMap("Player");
            }
            Destroy(this);
        }
    }

    void OnEnable()
    {
        ResetLobbyPlayer();
    }

    void Update()
    {
        if (playerFrameObject != null)
        {
            bool isUnavailable = lobbyManager.LockedCharacterNamesList.Contains(characterData.name) && !ready;
            playerFrameAnimator.SetBool("unavailable", isUnavailable);
            characterAnimator.GetComponent<Image>().color = isUnavailable ? Color.gray : Color.white;

            playerFrameAnimator.SetFloat("blend", selectedCharacterIndex);
            characterAnimator.SetFloat("blend", selectedCharacterIndex);
        }
    }

    private void ResetLobbyPlayer()
    {
        ready = false;
        selectedCharacterIndex = 0;
        characterData = lobbyManager.CharacterData[selectedCharacterIndex];
        playerFrameAnimator.SetBool("ready", false);
        playerFrameAnimator.SetBool("empty", false);
        playerFrameAnimator.SetBool("unavailable", false);
        playerFrameAnimator.SetFloat("blend", selectedCharacterIndex);
        characterAnimator.SetFloat("blend", selectedCharacterIndex);
    }

    public void SetPlayerFrame(GameObject playerFrame)
    {
        playerFrameObject = playerFrame;
        playerFrameAnimator = playerFrameObject.GetComponent<Animator>();
        characterObject = playerFrameObject.transform.GetChild(0).gameObject;
        characterAnimator = characterObject.GetComponent<Animator>();
        toJoinObject = playerFrameObject.transform.GetChild(1).gameObject;

        playerFrameAnimator.SetBool("empty", false);
        characterObject.SetActive(true);
        toJoinObject.SetActive(false);
    }

    public void NextCharacter(InputAction.CallbackContext context)
    {
        if (context.performed && !ready)
        {
            selectedCharacterIndex = (selectedCharacterIndex + 1) % lobbyManager.CharacterData.Length;
            //selectedCharacterName = characterNames[selectedCharacterIndex];
            characterData = lobbyManager.CharacterData[selectedCharacterIndex];
            playableCharacter.SetCharacterData(lobbyManager.CharacterData[selectedCharacterIndex]);
        }
    }

    public void PreviousCharacter(InputAction.CallbackContext context)
    {
        if (context.performed && !ready)
        {
            selectedCharacterIndex = (selectedCharacterIndex - 1 + lobbyManager.CharacterData.Length) % lobbyManager.CharacterData.Length;
            //selectedCharacterName = characterNames[selectedCharacterIndex];
            characterData = lobbyManager.CharacterData[selectedCharacterIndex];
            playableCharacter.SetCharacterData(lobbyManager.CharacterData[selectedCharacterIndex]);
        }
    }

    public void ToggleReady(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!ready && !lobbyManager.LockedCharacterNamesList.Contains(characterData.name))
            {
                SetReadyState(true);
            }
            else if (ready)
            {
                SetReadyState(false);
            }
        }
    }

    private void SetReadyState(bool state)
    {
        ready = state;
        lobbyManager.ToggleLockedCharacter(characterData.name, ready);
        playerFrameAnimator.SetBool("ready", ready);
    }

    public void UnsetReady()
    {
        SetReadyState(false);
    }

    public void Leave(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerFrameAnimator.SetBool("empty", true);
            characterObject.SetActive(false);
            toJoinObject.SetActive(true);
            lobbyManager.LeavePlayer(GetComponent<PlayerInput>());
            Destroy(gameObject);
        }
    }

    public void EnablePlayerActionMap()
    {
        GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
    }
}