using UnityEngine;
using UnityEngine.Events;

namespace SpaceShooter
{
    public class Player : MonoSingleton<Player>
    {
        #region Properties
        [SerializeField] private int m_NumLives;
        public int NumLives => m_NumLives;
        
        [SerializeField] private SpaceShip m_Ship;
        public SpaceShip ActiveShip => m_Ship;

        [SerializeField] private SpaceShip m_PlayerShipPrefab;

        [SerializeField] private CameraController m_CameraController;
        [SerializeField] private SpaceshipController m_SpaceshipController;

        [SerializeField] private Transform m_RespawnPoint;

        private int m_Score;
        public int Score => m_Score;

        private int m_NumKills;
        public int NumKills => m_NumKills;

        private int m_AsteroidKills;
        public int AsteroidKills => m_AsteroidKills;

        private int m_DeathsCount;
        public int DeathsCount => m_DeathsCount;

        private UnityEvent m_EventRespawnShip = new UnityEvent();
        public UnityEvent EventRespawnShip => m_EventRespawnShip;

        #endregion

        protected override void Awake()
        {
            base.Awake();

            if (m_Ship != null)
                Destroy(m_Ship.gameObject);

            Respawn();
        }

        private void OnDisable()
        {
            PlayerPrefs.SetFloat("AllTimeStatistics:Playtime", PlayerPrefs.GetFloat("AllTimeStatistics:Playtime", 0) + Time.timeSinceLevelLoad);
        }

        private void OnshipDeath()
        {
            m_NumLives--;
            m_DeathsCount++;

            if (m_NumLives > 0)
                Invoke(nameof(Respawn), 2f);
            else
                LevelSequenceController.Instance.FinishCurrentLevel(false);
        }

        private void Respawn()
        {
            if (LevelSequenceController.PlayerShip != null)
            {
                SpaceShip newPlayerShip = Instantiate(LevelSequenceController.PlayerShip, m_RespawnPoint.position, Quaternion.identity);

                m_Ship = newPlayerShip;

                m_CameraController.SetTarget(m_Ship.transform);
                m_SpaceshipController.SetTargetShip(m_Ship);

                m_Ship.EventOnDeath.AddListener(OnshipDeath);

                m_EventRespawnShip.Invoke();
            }
        }

        #region Score
        public void AddScore(int num)
        {
            m_Score += num;
        }

        public void AddKill()
        {
            m_NumKills++;
        }

        public void RemoveKill()
        {
            m_NumKills--;
        }

        public void AddAsteroidKill()
        {
            m_AsteroidKills++;
        }

        #endregion
    }
}
