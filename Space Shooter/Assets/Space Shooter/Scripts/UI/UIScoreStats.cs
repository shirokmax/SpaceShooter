using UnityEngine;
using UnityEngine.UI;

namespace SpaceShooter
{
    public class UIScoreStats : MonoBehaviour
    {
        [SerializeField] private Text m_ScoreText;

        private int m_LastScore;

        private void Start()
        {
            m_ScoreText.text = "Score : 0"; 
        }

        private void Update()
        {
            ScoreUpdate();
        }

        private void ScoreUpdate()
        {
            if (Player.Instance != null)
            {
                int currentScore = Player.Instance.Score;

                if (m_LastScore != currentScore)
                {
                    m_LastScore = currentScore;

                    m_ScoreText.text = "Score : " + m_LastScore.ToString();
                }
            }
        }
    }
}
