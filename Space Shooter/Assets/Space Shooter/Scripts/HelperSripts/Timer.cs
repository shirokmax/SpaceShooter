public class Timer
{
    private float m_CurrentTime;
    private float m_StartTime;

    public bool IsFinished => m_CurrentTime <= 0;

    public Timer(float startTime)
    {
        Start(startTime);
    }

    /// <summary>
    /// Задает StartTime и запускает таймер.
    /// </summary>
    /// <param name="startTime"Стартовое время таймера.></param>
    public void Start(float startTime)
    {
        m_StartTime = startTime;

        Restart();
    }

    /// <summary>
    /// Перезапускает таймер по последнему значению StartTime.
    /// </summary>
    public void Restart()
    {
        m_CurrentTime = m_StartTime;
    }

    /// <summary>
    /// Обнуляет таймер.
    /// </summary>
    public void Reset()
    {
        RemoveTime(m_StartTime);
    }

    /// <summary>
    /// Вычитает заданное время из текущего.
    /// </summary>
    /// <param name="deltaTime">Вычитаемое время.</param>
    public void RemoveTime(float deltaTime)
    {
        if (m_CurrentTime <= 0) return;

        m_CurrentTime -= deltaTime;
    }
}
