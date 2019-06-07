using Microsoft.Xna.Framework;

namespace FEIG.Utils
{
    /// <summary>
    /// A timer that counts down from 'TotalTime' value using 'TimeLeft' value.
    /// </summary>
    public class SimpleTimer
    {
        public float StartTime { get; private set; }
        public float TimeLeft { get; private set; }

        public SimpleTimer(float startTime)
        {
            Set(startTime);
        }

        public void Set(float startTime)
        {
            StartTime = startTime;
            TimeLeft = startTime;
        }

        public void Tick(GameTime gameTime)
        {
            TimeLeft -= gameTime.ElapsedGameTime.Milliseconds;
        }

        /// <summary>
        /// Sets timer to zero, but keeps current delay
        /// </summary>
        public void Reset()
        {
            TimeLeft = StartTime;
        }
    }
}
