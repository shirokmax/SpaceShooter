using UnityEngine;
using UnityEngine.UI;

namespace SpaceShooter
{
    public class SpaceShipView : MonoBehaviour
    {
        [SerializeField] private SpaceShip m_Ship;

        [Space]
        [SerializeField] private GameObject m_HitPointsBar;
        [SerializeField] private Image m_HitPointsBarImage;

        [Space]
        [SerializeField] private Canvas m_HitPointsBarCanvas;
        [SerializeField] private Canvas m_MapPlayerIconCanvas;
        [SerializeField] private Canvas m_MinimapPlayerIconCanvas;
        [SerializeField] private Canvas m_MinimapIconCanvas;

        [Space]
        [SerializeField] private Text m_DamageTakenText;

        [Space]
        [SerializeField] private ImpactEffect m_ShipExplosionVFX;

        [Space]
        [SerializeField] private GameObject m_SpeedBoostVFX;
        [SerializeField] private GameObject m_InvincibleVFX;

        [Space]
        [SerializeField] private ImpactEffect m_InvincibleOnImpactSFX;
        [SerializeField] private ImpactEffect m_InvincibleOffImpactSFX;

        [Space]
        [SerializeField] private ImpactEffect m_SpeedBoostOnImpactSFX;
        [SerializeField] private ImpactEffect m_SpeedBoostOffImpactSFX;

        private void Start()
        {
            m_InvincibleVFX.SetActive(false);
            m_SpeedBoostVFX.SetActive(false);

            m_DamageTakenText.gameObject.SetActive(false);

            if (m_Ship == Player.Instance.ActiveShip)
            {
                m_MapPlayerIconCanvas.gameObject.SetActive(true);
                m_MinimapPlayerIconCanvas.gameObject.SetActive(true);
                m_MinimapIconCanvas.gameObject.SetActive(false);
            }
            else
            {
                m_MapPlayerIconCanvas.gameObject.SetActive(false);
                m_MinimapPlayerIconCanvas.gameObject.SetActive(false);
                m_MinimapIconCanvas.gameObject.SetActive(true);
            }

            m_Ship.EventOnDeath.AddListener(OnDestroyShip);
            m_Ship.EventChangeHitPoints.AddListener(HitPointsBarUpdate);
            m_Ship.EventDamageTaken.AddListener(SpawnDamageTakenText);
        }

        private void FixedUpdate()
        {
            PowerupEffects();
            HitPointsBarUpdate();
        }

        private void HitPointsBarUpdate()
        {
            if (m_HitPointsBarImage == null) return;

            if (m_Ship.CurrentHitPoints == m_Ship.MaxHitPoints || m_Ship == Player.Instance.ActiveShip)
                m_HitPointsBar.SetActive(false);
            else
                m_HitPointsBar.SetActive(true);

            float hitPointsNormalized = (float)m_Ship.CurrentHitPoints / m_Ship.MaxHitPoints;

            m_HitPointsBarImage.fillAmount = hitPointsNormalized;
        }

        private void SpawnDamageTakenText(Destructible fromDest, int damage)
        {
            if (fromDest != Player.Instance.ActiveShip) return;
            if (m_Ship == Player.Instance.ActiveShip) return;

            Text dmgText = Instantiate(m_DamageTakenText, m_HitPointsBarCanvas.transform);

            dmgText.gameObject.SetActive(true);

            Vector3 textPos = Vector3.Lerp(m_DamageTakenText.transform.position, m_DamageTakenText.transform.position + m_DamageTakenText.transform.right * 0.5f, Random.Range(0f, 1f));
            dmgText.transform.position = textPos;

            dmgText.text = damage.ToString();
        }

        private void PowerupEffects()
        {
            InvinciblePowerupEffect();
            SpeedPowerupEffect();
        }

        private void InvinciblePowerupEffect()
        {
            if (m_Ship.isInvincibleWasOn == true)
            {
                Instantiate(m_InvincibleOnImpactSFX, transform.position, Quaternion.identity);

                m_Ship.isInvincibleWasOn = false;
            }

            if (m_Ship.IsIndestructible == true && m_InvincibleVFX.activeSelf == false)
            {
                m_InvincibleVFX.SetActive(true);
            }

            if (m_Ship.IsIndestructible == false && m_InvincibleVFX.activeSelf == true)
            {
                m_InvincibleVFX.SetActive(false);
                Instantiate(m_InvincibleOffImpactSFX, transform.position, Quaternion.identity);
            }
        }

        private void SpeedPowerupEffect()
        {
            if (m_Ship.isSpeedBoostWasOn == true)
            {
                Instantiate(m_SpeedBoostOnImpactSFX, transform.position, Quaternion.identity);

                m_Ship.isSpeedBoostWasOn = false;
            }

            if (m_Ship.IsSpeedBoostActive == true && m_SpeedBoostVFX.activeSelf == false)
            {
                m_SpeedBoostVFX.SetActive(true);
            }

            if (m_Ship.IsSpeedBoostActive == false && m_SpeedBoostVFX.activeSelf == true)
            {
                m_SpeedBoostVFX.SetActive(false);
                Instantiate(m_SpeedBoostOffImpactSFX, transform.position, Quaternion.identity);
            }
        }

        private void OnDestroyShip()
        {
            Instantiate(m_ShipExplosionVFX, transform.position, Quaternion.identity);
        }
    }
}
