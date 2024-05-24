using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LobbyPlayer : MonoBehaviour {
        
        private bool ready;
        public bool Ready { get => ready; }
        private string[] characterNames = { "Qyron", "Qyana", "Meowcello", "Gark" };
        public string[] AvaliableCharacters;
        private string selectedCharacterName;
        private int selectedCharacterIndex;
        private GameObject playerFrameObject;
        private Animator playerFrameAnimator;


        void Awake()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                GetComponent<PlayerInput>().SwitchCurrentActionMap("Lobby");
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

        }

        public void NextCharacter(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                selectedCharacterIndex++;
                if (selectedCharacterIndex >= characterNames.Length)
                {
                    selectedCharacterIndex = 0;
                }
                selectedCharacterName = characterNames[selectedCharacterIndex];
                playerFrameAnimator.SetFloat("blend", selectedCharacterIndex);
            }
        }

        public void PreviousCharacter(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                selectedCharacterIndex--;
                if (selectedCharacterIndex < 0)
                {
                    selectedCharacterIndex = characterNames.Length - 1;
                }
                selectedCharacterName = characterNames[selectedCharacterIndex];
                playerFrameAnimator.SetFloat("blend", selectedCharacterIndex);
            }
        }

        public void ToggleReady(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                ready = !ready;
                Debug.Log("Ready: " + ready);
                playerFrameAnimator.SetBool("ready", ready);
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
