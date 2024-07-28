using UnityEngine;

namespace Sherko.Utils
{
    public class ValueTransferer
    {
        private float _current;

        public float InitValue { get; }
        public float Speed { get; }

        public ValueTransferer(float init, float speed)
        {
            InitValue = _current = init;
            Speed = speed;
        }

        public float SmoothTransfer(float target)
        {
            if (Mathf.Abs(_current - target) < 0.01f) return _current;
            return _current = Mathf.Lerp(_current, target, Speed * Time.deltaTime);
        }

        public float SmoothReset()
        {
            if (Mathf.Abs(_current - InitValue) < 0.01f) return _current;
            return _current = Mathf.Lerp(_current, InitValue, Speed * Time.deltaTime);
        }
    }
}

