using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyPlayer : MonoBehaviour {
        
        private bool ready;
        public bool Ready { get => ready; }
        private string[] characterNames = { "Qyron", "Qyana", "Meowcello", "Gark" };
        public string[] AvaliableCharacters;
        private string selectedCharacterName;
        public string SelectedCharacterName { get => selectedCharacterName; }
        private int selectedCharacterIndex;
        private GameObject playerFrameObject;
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

        void Start()
        {

        }

        void Update()
        {
            if (playerFrameObject != null)
            {

            }
        }

        public void SetPlayerFrame(GameObject playerFrame)
        {
            playerFrameObject = playerFrame;
            playerFrameAnimator = playerFrameObject.GetComponent<Animator>();
            playerFrameAnimator.SetBool("empty", false);
            characterObject = playerFrameObject.transform.GetChild(0).gameObject;
            characterAnimator = characterObject.GetComponent<Animator>();
            characterObject.SetActive(true);
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

                    HandlePlayerFrame();
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
                    
                    HandlePlayerFrame();
                }
            }
        }

        private void HandlePlayerFrame()
        {
            selectedCharacterName = characterNames[selectedCharacterIndex];

            if(lobbyManager.LockedCharacterNames.Contains(selectedCharacterName))
            {
                playerFrameAnimator.SetBool("unavaliable", true);
                characterAnimator.GetComponent<Image>().color = Color.gray;
            }
            else
            {
                playerFrameAnimator.SetBool("unavaliable", false);
                characterAnimator.GetComponent<Image>().color = Color.white;
            }

            playerFrameAnimator.SetFloat("blend", selectedCharacterIndex);
            characterAnimator.SetFloat("blend", selectedCharacterIndex);
        }

        public void ToggleReady(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if(!lobbyManager.LockedCharacterNames.Contains(selectedCharacterName))
                {
                    ready = !ready;
                    Debug.Log("Ready: " + ready);
                    playerFrameAnimator.SetBool("ready", ready);
                }
            }
        }

        public void Leave(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Debug.Log("Leave");
                Destroy(gameObject);
            }
        }

        public void LeaveToMenu()
        {
            Destroy(gameObject);
        }

        public void EnablePlayerActionMap   ()
        {
            GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        }
    }
