using UnityEngine;
using UnityEngine.UI;

namespace SpaceShooter
{
    public class UIPlayerShipStats : MonoBehaviour
    {
        [SerializeField] private Text m_LivesCount;

        [Space]
        [SerializeField] private Image m_HitPointsBar;
        [SerializeField] private Image m_EnergyBar;

        [Space]
        [SerializeField] private Text m_AmmoText;

        [Space]
        [SerializeField] private GameObject m_NoEffectTextObj;

        [Space]
        [SerializeField] private GameObject m_EffectsPanel;

        [Space]
        [SerializeField] private GameObject m_InvincibleEffectPanel;
        [SerializeField] private GameObject m_SpeedUpEffectPanel;
        [SerializeField] private GameObject m_NewWeaponEffectPanel;

        [Space]
        [SerializeField] private Image m_InvincibleEffectTimeBar;
        [SerializeField] private Image m_SpeedUpEffectTimeBar;
        [SerializeField] private Image m_NewWeaponEffectTimeBar;

        private void Start()
        {
            m_InvincibleEffectPanel.SetActive(false);
            m_SpeedUpEffectPanel.SetActive(false);
            m_NewWeaponEffectPanel.SetActive(false);
  
            Player.Instance.EventRespawnShip.AddListener(AssignListeners);

            InitStats();
            AssignListeners();
        }

        private void AssignListeners()
        {
            if (Player.Instance.ActiveShip == null) return;

            Player.Instance.ActiveShip.EventChangeHitPoints.AddListener(OnChangeHitPoints);
            Player.Instance.ActiveShip.EventChangeEnergy.AddListener(OnChangeEnergy);
            Player.Instance.ActiveShip.EventChangeAmmo.AddListener(OnChangeAmmo);
            Player.Instance.ActiveShip.EventOnDeath.AddListener(OnChangeLivesCount);

            InitStats();
        }

        private void InitStats()
        {
            OnChangeLivesCount();
            OnChangeHitPoints();
            OnChangeEnergy();
            OnChangeAmmo();
        }

        private void Update()
        {
            CheckNoEffects();

            InvincibleEffectShow();
            SpeedUpEffectShow();
            NewWeaponEffectShow();
        }

        private void OnChangeHitPoints()
        {
            if (Player.Instance.ActiveShip == null) return;

            float hitPointsNormalized = (float)Player.Instance.ActiveShip.CurrentHitPoints / Player.Instance.ActiveShip.MaxHitPoints;

            m_HitPointsBar.fillAmount = hitPointsNormalized;
        }

        private void OnChangeEnergy()
        {
            if (Player.Instance.ActiveShip == null) return;

            float energyNormalized = Player.Instance.ActiveShip.Energy / Player.Instance.ActiveShip.MaxEnergy;

            m_EnergyBar.fillAmount = energyNormalized;
        }

        private void OnChangeAmmo()
        {
            if (Player.Instance.ActiveShip == null) return;

            m_AmmoText.text = Player.Instance.ActiveShip.Ammo.ToString();
        }

        private void OnChangeLivesCount()
        {
            m_LivesCount.text = "Lifes: " + Player.Instance.NumLives;
        }

        public void CheckNoEffects()
        {
            if (m_EffectsPanel.GetComponentsInChildren<Transform>(false).Length - 1 > 0)
            {
                m_NoEffectTextObj.SetActive(false);
            }
            else
            {
                m_NoEffectTextObj.SetActive(true);
            }
        }

        public void InvincibleEffectShow()
        {
            if (Player.Instance.ActiveShip?.InvincibleTimer > 0)
            {
                m_InvincibleEffectPanel.SetActive(true);

                float timeNormalized = Player.Instance.ActiveShip.InvincibleTimer / Player.Instance.ActiveShip.InvincibleLastDurationTime;

                m_InvincibleEffectTimeBar.fillAmount = timeNormalized;
            }
            else
            {
                m_InvincibleEffectPanel.SetActive(false);
            }
        }

        public void SpeedUpEffectShow()
        {
            if (Player.Instance.ActiveShip?.SpeedBoostTimer > 0)
            {
                m_SpeedUpEffectPanel.SetActive(true);

                float timeNormalized = Player.Instance.ActiveShip.SpeedBoostTimer / Player.Instance.ActiveShip.SpeedBoostLastDurationTime;

                m_SpeedUpEffectTimeBar.fillAmount = timeNormalized;
            }
            else
            {
                m_SpeedUpEffectPanel.SetActive(false);
            }
        }

        public void NewWeaponEffectShow()
        {
            if (Player.Instance.ActiveShip?.Turrets.Length != 0 && 
                Player.Instance.ActiveShip?.Turrets[0].AssignLoadoutTimer > 0)
            {
                m_NewWeaponEffectPanel.SetActive(true);

                float timeNormalized = Player.Instance.ActiveShip.Turrets[0].AssignLoadoutTimer / Player.Instance.ActiveShip.Turrets[0].AssignLoadoutLastDurationTime;

                m_NewWeaponEffectTimeBar.fillAmount = timeNormalized;
            }
            else
            {
                m_NewWeaponEffectPanel.SetActive(false);
            }
        }
    }
}
