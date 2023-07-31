using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceShooter
{
    public class UIObjectivesPanel : MonoBehaviour
    {
        [SerializeField] private Image m_ObjectivesPanelButtonImage;
        [SerializeField] private Sprite m_ArrowLeft;
        [SerializeField] private Sprite m_ArrowRight;

        [Space]
        [SerializeField] private Transform m_ObjectiveTextsPanel;
        [SerializeField] private Text m_ObjectiveTextPrefab;

        [Space]
        [SerializeField] private bool m_HideObjectivesPanelAfterStartLevel;
        [SerializeField] private float m_TimeToHideObjectivesPanel;

        private List<Text> m_ObjectiveTexts;

        private void Awake()
        {
            m_ObjectiveTexts = new List<Text>();
        }

        private void Start()
        {
            InstantiateObjectiveTexts();

            ShowObjectivesPanel();

            if (m_HideObjectivesPanelAfterStartLevel == true)
                Invoke(nameof(CloseObjectivesPanel), m_TimeToHideObjectivesPanel);
        }

        private void Update()
        {
            UpdateObjectiveTexts();
        }

        public void OnObjectivesPanelButtonClick()
        {
            if (gameObject.activeSelf == false)
                ShowObjectivesPanel();
            else
                CloseObjectivesPanel();
        }

        private void ShowObjectivesPanel()
        {
            gameObject.SetActive(true);

            m_ObjectivesPanelButtonImage.sprite = m_ArrowLeft;
        }

        private void CloseObjectivesPanel()
        {
            gameObject.SetActive(false);

            m_ObjectivesPanelButtonImage.sprite = m_ArrowRight;
        }

        private void InstantiateObjectiveTexts()
        {
            foreach(ILevelCondition condition in LevelController.Instance.Conditions)
            {
                Text objectiveText = Instantiate(m_ObjectiveTextPrefab, m_ObjectiveTextsPanel);

                objectiveText.text = condition.Description;

                m_ObjectiveTexts.Add(objectiveText);
            }
        }

        private void UpdateObjectiveTexts()
        {
            for (int i = 0; i < LevelController.Instance.Conditions.Length; i++)
            {
                if (m_ObjectiveTexts[i].text != LevelController.Instance.Conditions[i].Description)
                    m_ObjectiveTexts[i].text = LevelController.Instance.Conditions[i].Description;
            }
        }
    }
}
