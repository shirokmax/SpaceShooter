using UnityEngine;

namespace SpaceShooter
{
    public class UIMapPanel : MonoBehaviour
    {
        [SerializeField] private GameObject m_MapPanel;
        [SerializeField] private GameObject m_MapPanelButton;

        private void Start()
        {
            m_MapPanel.SetActive(false);

            if (Application.isMobilePlatform == true)
                m_MapPanelButton.SetActive(true);
            else
                m_MapPanelButton.SetActive(false);
        }

        private void Update()
        {
            if (Application.isMobilePlatform == false)
            {
                if (Input.GetKey(KeyCode.Tab))
                    m_MapPanel.SetActive(true);
                else
                    m_MapPanel.SetActive(false);
            }
        }

        public void OnMapPanelButtonClick()
        {
            if (m_MapPanel.activeSelf == false)
                m_MapPanel.SetActive(true);
            else
                m_MapPanel.SetActive(false);
        }
    }
}
