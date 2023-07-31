using UnityEngine;

namespace SpaceShooter
{
    public class UIMainMenu : MonoSingleton<UIMainMenu>
    {
        [SerializeField] private GameObject m_EpisodeSelection;
        [SerializeField] private GameObject m_ShipSelection;

        private void Start()
        {
            gameObject.SetActive(true);
            m_EpisodeSelection.SetActive(false);
            m_ShipSelection.SetActive(false);
        }

        public void OnButtonExit()
        {
            Application.Quit();
        }
    }
}
