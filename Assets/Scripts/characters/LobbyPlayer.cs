using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LobbyPlayer : MonoBehaviour {
        
        private bool ready;
        public bool Ready { get => ready; }
        private string[] characterNames = { "Qyron", "Qyana", "Meowcello", "Gark" };
        public string[] AvaliableCharacters;
        private string selectedCharacterName;
        public string SelectedCharacterName { get => selectedCharacterName; }
        private int selectedCharacterIndex;
        


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
            }
        }

        public void ToggleReady(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                ready = !ready;
                Debug.Log("Ready: " + ready);
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

        public void SwitchActionMap()
        {
            GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        }
    }
