using UnityEngine;
using UnityEngine.UI;

namespace SpaceShooter
{
    [RequireComponent(typeof(Text), typeof(ImpactEffect))]
    public class UIShipDamageTakenText : MonoBehaviour
    {
        [SerializeField] private float m_TextSlideSpeed;

        private Text m_Text;
        private ImpactEffect m_TextImpactEffect;

        private void Awake()
        {
            m_Text = GetComponent<Text>();
            m_TextImpactEffect = GetComponent<ImpactEffect>();
        }

        private void FixedUpdate()
        {
            transform.Translate(Vector3.up * m_TextSlideSpeed * Time.fixedDeltaTime);

            if (m_TextImpactEffect != null)
            {
                float transparency = (m_TextImpactEffect.LifeTime - m_TextImpactEffect.LifeTimer) / m_TextImpactEffect.LifeTime;

                m_Text.color = new Color(m_Text.color.r, m_Text.color.g, m_Text.color.b, transparency);
            }
        }
    }
}
