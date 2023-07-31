using UnityEngine;

namespace SpaceShooter
{
    public class BlackHole : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_LightAroundSprite;
        [SerializeField] private AnimationCurve m_BrightnessCurve;

        private float m_CurrentTime;
        private float m_CurveTotalTime;

        private void Start()
        {
            m_CurveTotalTime = m_BrightnessCurve.keys[m_BrightnessCurve.keys.Length - 1].time;
        }

        private void Update()
        {
            Color color = new Color(m_LightAroundSprite.color.r, m_LightAroundSprite.color.g, m_LightAroundSprite.color.b, m_BrightnessCurve.Evaluate(m_CurrentTime));
            m_LightAroundSprite.color = color;

            m_CurrentTime += Time.deltaTime;

            if (m_CurrentTime >= m_CurveTotalTime)
                m_CurrentTime = 0;
        }
    }
}
