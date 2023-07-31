using UnityEngine;

public abstract class AlphaColorChangeCurve : MonoBehaviour
{
    [SerializeField] private AnimationCurve m_ColorCurve;

    protected float m_CurrentTime;
    private float m_CurveTotalTime;

    protected virtual void Awake()
    {
        m_CurveTotalTime = m_ColorCurve.keys[m_ColorCurve.keys.Length - 1].time;
    }

    /// <summary>
    /// Изменяет альфа канал заданного цвета. Данный метод нужно использовать в Update.
    /// </summary>
    /// <param name="clr"></param>
    /// <returns>Возвращает цвет с измененным альфа каналом по Animation Curve.</returns>
    protected Color GetUpdatedColor(Color clr)
    {
        Color color = new Color(clr.r, clr.g, clr.b, m_ColorCurve.Evaluate(m_CurrentTime));

        m_CurrentTime += Time.deltaTime;

        if (m_CurrentTime >= m_CurveTotalTime)
            m_CurrentTime = 0;

        return color;
    }
}
