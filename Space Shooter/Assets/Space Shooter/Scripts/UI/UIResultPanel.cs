using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SpaceShooter
{
    public class UIResultPanel : MonoSingleton<UIResultPanel>
    {
        [SerializeField] private Text m_ResultText;

        [SerializeField] private Text m_ScoreText;
        [SerializeField] private Text m_KillsText;
        [SerializeField] private Text m_TimeText;

        [SerializeField] private Text m_NextButtonText;

        private bool m_Success;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void ShowResults(PlayerStatistics levelResults, bool succes)
        {
            gameObject.SetActive(true);

            m_Success = succes;

            m_ResultText.text = succes ? "Level Complete!" : "Game Over!";

            if (levelResults.ScoreMult > 1)
                m_ScoreText.text = "Score: " + levelResults.Score.ToString() + "\n (" + levelResults.ScoreMult.ToString("F1") + "x level time bonus)";
            else
                m_ScoreText.text = "Score: " + levelResults.Score.ToString();

            m_KillsText.text = "Spaceship Killed: " + levelResults.SpaceshipKills.ToString();
            m_TimeText.text = "Level time: " + TimeFormat.Format(levelResults.Time);

            m_NextButtonText.text = succes ? "Next" : "Restart";

            Time.timeScale = 0;
        }

        public void OnNextActionButtonClick()
        {
            gameObject.SetActive(false);

            Time.timeScale = 1;

            if (m_Success)
                LevelSequenceController.Instance.AdvanceLevel();
            else
                LevelSequenceController.Instance.RestartLevel();
        }
    }
}
