using UnityEngine.SceneManagement;
using UnityEngine;

namespace SpaceShooter
{
    public class LevelSequenceController : MonoSingleton<LevelSequenceController>
    {
        public static string MainMenuSceneName = "scene_main_menu";

        public Episode CurrentEpisode { get; private set; }

        public int CurrentLevel { get; private set; }

        public bool LastLevelResult { get; private set; }

        public PlayerStatistics LevelStatistic { get; private set; }

        [SerializeField] private SpaceShip m_DefaultShip;

        public static SpaceShip PlayerShip;

        private void Start()
        {
            PlayerShip = m_DefaultShip;
        }

        public void StartEpisode(Episode ep)
        {
            CurrentEpisode = ep;
            CurrentLevel = 0;

            if (CurrentEpisode.Levels.Length == 0) return;

            LevelStatistic = new PlayerStatistics();

            SceneManager.LoadScene(CurrentEpisode.Levels[CurrentLevel]);
        }

        public void RestartLevel()
        {
            SceneManager.LoadScene(CurrentEpisode.Levels[CurrentLevel]);
        }

        public void FinishCurrentLevel(bool success)
        {
            LastLevelResult = success;
            CalculateLevelStatistics(success);

            UIResultPanel.Instance.ShowResults(LevelStatistic, success);
        }

        public void AdvanceLevel()
        {
            LevelStatistic.Reset();

            CurrentLevel++;

            if (CurrentEpisode.Levels.Length <= CurrentLevel)
            {
                PlayerPrefs.SetInt("Episode:" + CurrentEpisode.EpisodeName, 1);

                SceneManager.LoadScene(MainMenuSceneName);
            }
            else
                SceneManager.LoadScene(CurrentEpisode.Levels[CurrentLevel]);
        }

        private void CalculateLevelStatistics(bool success)
        {
            LevelStatistic.Reset();

            if (success == true)
            {
                if (LevelController.Instance.LevelTime < LevelController.Instance.ReferenceTime)
                {
                    LevelStatistic.ScoreMult = Mathf.Clamp(LevelController.Instance.ReferenceTime / LevelController.Instance.LevelTime, 1f, 5f);

                    LevelStatistic.Score = (int)(Player.Instance.Score * LevelStatistic.ScoreMult);
                }
                else
                {
                    LevelStatistic.Score = Player.Instance.Score;
                }
            }
            else
            {
                LevelStatistic.Score = Player.Instance.Score;
            }

            LevelStatistic.SpaceshipKills = Player.Instance.NumKills;
            LevelStatistic.Time = (int)LevelController.Instance.LevelTime;

            SaveAllTimeStatistic();
        }

        private void SaveAllTimeStatistic()
        {
            PlayerPrefs.SetInt("AllTimeStatistics:Score", PlayerPrefs.GetInt("AllTimeStatistics:Score", 0) + LevelStatistic.Score);
            PlayerPrefs.SetInt("AllTimeStatistics:SpaceshipKills", PlayerPrefs.GetInt("AllTimeStatistics:SpaceshipKills", 0) + LevelStatistic.SpaceshipKills);
            PlayerPrefs.SetInt("AllTimeStatistics:AsteroidKills", PlayerPrefs.GetInt("AllTimeStatistics:AsteroidKills", 0) + Player.Instance.AsteroidKills);
            PlayerPrefs.SetInt("AllTimeStatistics:DeathsCount", PlayerPrefs.GetInt("AllTimeStatistics:DeathsCount", 0) + Player.Instance.DeathsCount);
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                PlayerPrefs.DeleteAll();
            }
        }
#endif
    }
}
