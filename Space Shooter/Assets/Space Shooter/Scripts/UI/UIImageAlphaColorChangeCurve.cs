using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Image))]
public class UIImageAlphaColorChangeCurve : AlphaColorChangeCurve
{
    private Image m_Image;

    protected override void Awake()
    {
        base.Awake(); 
        m_Image = GetComponent<Image>();
    }

    private void Update()
    {
        m_Image.color = GetUpdatedColor(m_Image.color);
    }
}
