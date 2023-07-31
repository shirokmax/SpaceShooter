using UnityEngine;
using UnityEngine.UI;

namespace SpaceShooter
{
    public class UIEpisodeSelect : MonoBehaviour
    {
        [SerializeField] private Episode m_Episode;
        [SerializeField] private Text m_EpisodeNameText;
        [SerializeField] private Image m_PreviewImage;
        [SerializeField] private GameObject m_CompletedPanel;

        private void Start()
        {
            if (m_EpisodeNameText != null)
                m_EpisodeNameText.text = m_Episode.EpisodeName;

            if (m_PreviewImage != null)
                m_PreviewImage.sprite = m_Episode.PreviewImage;

            if (PlayerPrefs.GetInt("Episode:" + m_Episode.EpisodeName, 0) == 1)
                m_CompletedPanel.SetActive(true);
        }

        public void OnStartEpisodeButtonClick()
        {
            LevelSequenceController.Instance.StartEpisode(m_Episode);
        }
    }
}