using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyPlayer : MonoBehaviour {
        
        private bool ready;
        public bool Ready { get => ready; }
        private string[] characterNames = { "Qyron", "Qyana", "Meowcello", "Gark" };
        private string selectedCharacterName;
        public string SelectedCharacterName { get => selectedCharacterName; }
        private int selectedCharacterIndex;

        private GameObject playerFrameObject;
        private GameObject toJoinObject;
        private GameObject characterObject;
        private Animator playerFrameAnimator;
        private Animator characterAnimator;
        private LobbyManager lobbyManager;


        void Awake()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                GetComponent<PlayerInput>().SwitchCurrentActionMap("Lobby");
                lobbyManager = FindObjectOfType<LobbyManager>();
            }
            else 
            {
                GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
                Destroy(this);
            }
        }

        void OnEnable()
        {
            ResetLobbyPlayer();
        }

        void Start()
        {

        }

        void Update()
        {
            if (playerFrameObject != null)
            {
                if(lobbyManager.LockedCharacterNamesList.Contains(selectedCharacterName) && !ready)
                {
                    playerFrameAnimator.SetBool("unavaliable", true);
                    characterAnimator.GetComponent<Image>().color = Color.gray;
                    //canLockCharacter = false;
                }
                else
                {
                    playerFrameAnimator.SetBool("unavaliable", false);
                    characterAnimator.GetComponent<Image>().color = Color.white;
                    //canLockCharacter = true;
                }

                playerFrameAnimator.SetFloat("blend", selectedCharacterIndex);
                characterAnimator.SetFloat("blend", selectedCharacterIndex);
            }
        }

        private  void ResetLobbyPlayer()
        {
            ready = false;
            selectedCharacterIndex = 0;
            selectedCharacterName = characterNames[selectedCharacterIndex];
            playerFrameAnimator.SetBool("ready", false);
            playerFrameAnimator.SetBool("empty", false);
            playerFrameAnimator.SetBool("unavaliable", false);
            playerFrameAnimator.SetFloat("blend", selectedCharacterIndex);
            characterAnimator.SetFloat("blend", selectedCharacterIndex);
        }

        public void SetPlayerFrame(GameObject playerFrame)
        {
            playerFrameObject = playerFrame;
            playerFrameAnimator = playerFrameObject.GetComponent<Animator>();
            playerFrameAnimator.SetBool("empty", false);
            characterObject = playerFrameObject.transform.GetChild(0).gameObject;
            characterAnimator = characterObject.GetComponent<Animator>();
            characterObject.SetActive(true);
            toJoinObject = playerFrameObject.transform.GetChild(1).gameObject;
            toJoinObject.SetActive(false);
        }

        public void NextCharacter(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (!ready)
                {
                    selectedCharacterIndex++;

                    if (selectedCharacterIndex >= characterNames.Length)
                    {
                        selectedCharacterIndex = 0;
                    }

                    selectedCharacterName = characterNames[selectedCharacterIndex];
                }
            }
        }

        public void PreviousCharacter(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (!ready)
                {
                    selectedCharacterIndex--;

                    if (selectedCharacterIndex < 0)
                    {
                        selectedCharacterIndex = characterNames.Length - 1;
                    }

                    selectedCharacterName = characterNames[selectedCharacterIndex];
                }
            }
        }

        public void ToggleReady(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (!lobbyManager.LockedCharacterNamesList.Contains(selectedCharacterName) && !ready)
                {
                    ready = true;
                    lobbyManager.ToggleLockedCharacter(selectedCharacterName, ready);
                    playerFrameAnimator.SetBool("ready", ready);
                }
                else if (ready)
                {
                    ready = false;
                    lobbyManager.ToggleLockedCharacter(selectedCharacterName, ready);
                    playerFrameAnimator.SetBool("ready", ready);
                }
            }
        }

        public void UnsetReady()
        {
            ready = false;
            lobbyManager.ToggleLockedCharacter(selectedCharacterName, ready);
            playerFrameAnimator.SetBool("ready", ready);
        }

        public void Leave(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                playerFrameAnimator.SetBool("empty", true);
                characterObject.SetActive(false);
                toJoinObject.SetActive(true);
                lobbyManager.LeavePlayer(this.GetComponent<PlayerInput>());
                Destroy(gameObject);
            }
        }

        public void EnablePlayerActionMap()
        {
            GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        }
    }
