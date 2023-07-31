using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceShooter
{
    public class UIPausePanel : MonoBehaviour
    {
        [SerializeField] private GameObject m_PausePanel;
        private bool m_IsPaused;

        private void Start()
        {
            m_PausePanel.SetActive(false);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (m_IsPaused == true)
                    OnResumeButtonClick();
                else
                    OnPauseButtonClick();
            }
        }

        public void OnPauseButtonClick()
        {
            m_IsPaused = true;

            Time.timeScale = 0;

            m_PausePanel.SetActive(true);
        }

        public void OnResumeButtonClick()
        {
            m_IsPaused = false;

            Time.timeScale = 1;

            m_PausePanel.SetActive(false);
        }

        public void OnMainMenuButtonClick()
        {
            m_IsPaused = false;

            Time.timeScale = 1;

            m_PausePanel.SetActive(false);

            SceneManager.LoadScene(LevelSequenceController.MainMenuSceneName);
        }
    }
}
