using System;
using System.Diagnostics;

namespace MyLib
{
    public class MyTimer
    {
        public Stopwatch timer;

        public MyTimer()
        {
            timer = new Stopwatch();
        }
        public void Start()
        {
            timer.Start();
        }
        public string Result()
        {
            timer.Stop();
            var resultTime = timer.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                resultTime.Hours,
                resultTime.Minutes,
                resultTime.Seconds,
                resultTime.Milliseconds);
            return elapsedTime;
        }
    }
}
