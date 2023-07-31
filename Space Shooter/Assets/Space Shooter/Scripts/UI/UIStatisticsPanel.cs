using UnityEngine;
using UnityEngine.UI;

namespace SpaceShooter
{
    public class UIStatisticsPanel : MonoBehaviour
    {
        [SerializeField] private Text m_ScoreText;
        [SerializeField] private Text m_SpaceshipKillsText;
        [SerializeField] private Text m_AsteroidKillsText;
        [SerializeField] private Text m_DeathsCountText;
        [SerializeField] private Text m_PlaytimeText;

        private void Start()
        {
            m_ScoreText.text = "Score: " + PlayerPrefs.GetInt("AllTimeStatistics:Score", 0);
            m_SpaceshipKillsText.text = "Spaceships Killed: " + PlayerPrefs.GetInt("AllTimeStatistics:SpaceshipKills", 0);
            m_AsteroidKillsText.text = "Asteroids destroyed: " + PlayerPrefs.GetInt("AllTimeStatistics:AsteroidKills", 0);
            m_DeathsCountText.text = "Deaths: " + PlayerPrefs.GetInt("AllTimeStatistics:DeathsCount", 0);
            m_PlaytimeText.text = "Time played: " + TimeFormat.Format((int)PlayerPrefs.GetFloat("AllTimeStatistics:Playtime", 0));
        }
    }
}
