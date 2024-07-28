using System;
using UnityEngine;

namespace Sherko.Utils
{
    [Serializable]
    public class Consumer
    {
        public Consumer(float capacity)
        {
            _capacity = capacity;
        }
        
        [SerializeField] private float _capacity;
        private float _current;

        public event Action OnValueChange;
        public event Action OnConsumerFull;
        public event Action OnConsumerEmpty;
        
        public float Capacity
        {
            get => _capacity;
            set => _capacity = value;
        } 
        public float Current
        {
            get => _current;
            set => _current = Mathf.Clamp(value, 0, Capacity);
        }

        public bool IsFull => Current >= Capacity;
        public bool IsEmpty => Current == 0;

        public void Consume(float amount)
        {
            if (IsEmpty) return;
            Current -= amount;
            OnValueChange?.Invoke();
            if (IsEmpty) OnConsumerEmpty?.Invoke();
        }

        public void Fill(float amount)
        {
            if (IsFull) return;
            Current += amount;
            OnValueChange?.Invoke();
            if (IsFull) OnConsumerFull?.Invoke();
        }

        public void Empty()
        {
            if (IsEmpty) return;
            Current = 0;
            OnValueChange?.Invoke();
            if (IsEmpty) OnConsumerEmpty?.Invoke();
        }

        public void Full()
        {
            if (IsFull) return;
            Current = Capacity;
            OnValueChange?.Invoke();
            if (IsFull) OnConsumerFull?.Invoke();
        }
    }
}

