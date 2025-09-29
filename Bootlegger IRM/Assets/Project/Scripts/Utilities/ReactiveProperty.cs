using System;

namespace Bootlegger
{
    public class ReactiveProperty<T>
    {
        public event Action ValueChanged;

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                ValueChanged?.Invoke();
            }
        }
        private T _value;
    }
}
