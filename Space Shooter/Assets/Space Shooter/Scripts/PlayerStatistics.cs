namespace SpaceShooter
{
    public class PlayerStatistics
    {
        public int Score;
        public float ScoreMult;
        public int SpaceshipKills;
        public int Time;

        public PlayerStatistics()
        {
            Reset();
        }

        public void Reset()
        {
            Score = 0;
            ScoreMult = 1;
            SpaceshipKills = 0;
            Time = 0;
        }
    }
}
