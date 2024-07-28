using System;
using UnityEngine;

namespace Sherko.Utils
{
    [Serializable]
    public class IntervalRunner
    {
        private float _timer;

        public IntervalRunner(bool runFirstTime)
        {
            _timer = runFirstTime ? Mathf.Infinity : 0;
        }

        public void RunAtIntervals(Action action, float interval)
        {
            RunAtIntervals(action, interval, true);
        }

        /// <summary>
        /// Used in Update method to run an action in intervals
        /// </summary>
        /// <param name="action">action to execute</param>
        /// <param name="interval"></param>
        /// <param name="condition">execute condition</param>
        /// <returns>true if execution can be done regardless of the extra condition , false if not</returns>
        public bool RunAtIntervals(Action action, float interval, bool condition)
        {
            bool executed = _timer >= interval;
            if (_timer >= interval && condition)
            {
                action();
                Reset();
            }
            Tick();
            return executed;
        }

        public void Tick()
        {
            _timer += Time.deltaTime;
        }

        public void Reset()
        {
            _timer = 0f;
        }
    }
}

