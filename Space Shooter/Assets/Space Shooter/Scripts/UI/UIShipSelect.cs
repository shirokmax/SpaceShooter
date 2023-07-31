using UnityEngine;
using UnityEngine.UI;

namespace SpaceShooter
{
    public class UIShipSelect : MonoBehaviour
    {
        [SerializeField] private SpaceShip m_ShipPrefab;

        [SerializeField] private Text m_ShipName;
        [SerializeField] private Image m_PreviewImage;
        [SerializeField] private Transform m_HitpointsBar;
        [SerializeField] private Transform m_SpeedBar;
        [SerializeField] private Transform m_MobilityBar;

        private const int MAX_HP = 300;
        private const float MAX_SPEED = 500;
        private const float MAX_Mobility = 1000;

        private void Start()
        {
            m_ShipName.text = m_ShipPrefab.Nickname;
            m_PreviewImage.sprite = m_ShipPrefab.PreviewSprite;

            float hp = Mathf.Clamp01((float)m_ShipPrefab.MaxHitPoints / MAX_HP);
            int hpCellsCount = (int)(hp * m_HitpointsBar.childCount);
            
            for(int i = 0; i < hpCellsCount; i++)
            {
                m_HitpointsBar.GetChild(i).gameObject.SetActive(true);
            }

            float speed = Mathf.Clamp01(m_ShipPrefab.Thrust / MAX_SPEED);
            int speedCellsCount = (int)(speed * m_SpeedBar.childCount);

            for (int i = 0; i < speedCellsCount; i++)
            {
                m_SpeedBar.GetChild(i).gameObject.SetActive(true);
            }

            float mobility = Mathf.Clamp01(m_ShipPrefab.Mobility / MAX_Mobility);
            int mobilityCellsCount = (int)(mobility * m_MobilityBar.childCount);

            for (int i = 0; i < mobilityCellsCount; i++)
            {
                m_MobilityBar.GetChild(i).gameObject.SetActive(true);
            }
        }

        public void OnSelectShipButtonClick()
        {
            LevelSequenceController.PlayerShip = m_ShipPrefab;
        }
    }
}

