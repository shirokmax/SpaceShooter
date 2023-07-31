using UnityEngine;
using UnityEngine.UI;

namespace SpaceShooter
{
    [RequireComponent(typeof(Text))]
    public class UIPlayerLivesText : AlphaColorChangeCurve
    {
        private Text text;
        private Color m_StartColor;

        protected override void Awake()
        {
            base.Awake();

            text = GetComponent<Text>();
            m_StartColor = text.color;
        }

        private void Update()
        {
            if (Player.Instance.NumLives == 1)
            {
                text.color = GetUpdatedColor(text.color);
            }
            else if (text.color != m_StartColor)
            {
                text.color = m_StartColor;
            }
        }
    }
}
