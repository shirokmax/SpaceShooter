using UnityEngine;

namespace SpaceShooter
{
    [RequireComponent(typeof(MeshRenderer))]
    public class BackgroundElement : MonoBehaviour
    {
        [Range(0.0f, 4.0f)]
        [SerializeField] private float m_ParallaxStrength;

        [SerializeField] private float m_TextureScale;
        [SerializeField] private float m_InitialOffsetMult;
        [SerializeField] private bool m_InitialOffsetFromCenter;

        [SerializeField] private Color m_Color;

        [Range(0.0f, 1.0f)]
        [SerializeField] private float brightness;

        private Material m_QuadMaterial;
        private Vector2 m_InitialOffset;

        private void Start()
        {
            m_QuadMaterial = GetComponent<MeshRenderer>().material;
            m_QuadMaterial.EnableKeyword("_EMISSION");

            m_QuadMaterial.color = m_Color;
            m_QuadMaterial.SetColor("_EmissionColor", new Color(brightness, brightness, brightness));

            m_QuadMaterial.mainTextureScale = Vector2.one * m_TextureScale;

            if (m_InitialOffsetFromCenter == true)
            {
                Vector2 center = new Vector2((m_QuadMaterial.mainTextureScale.x / 2) - 0.5f, (m_QuadMaterial.mainTextureScale.y / 2) - 0.5f);

                m_InitialOffset = -center + Random.insideUnitCircle * m_InitialOffsetMult;
            }
            else
            {
                m_InitialOffset = Random.insideUnitCircle * m_InitialOffsetMult;
            }
        }

        private void Update()
        {
            Vector2 offset = m_InitialOffset;

            offset.x += transform.position.x / transform.localScale.x / m_ParallaxStrength;
            offset.y += transform.position.y / transform.localScale.y / m_ParallaxStrength;

            m_QuadMaterial.mainTextureOffset = offset;
        }
    }
}