using System;
using System.Collections.Generic;

namespace AlienJust.Support.Mvvm
{
    public class ViewModelProperty<T> where T : struct
    {
        public string Name { get; }
        private readonly Action<string> _propChangeAction;

        public Dictionary<string, Action<string>> DependedProperties { get; }

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (_value.Equals(value))
                {
                    _value = value;
                    _propChangeAction?.Invoke(Name);
                    NotifyAllDepended();
                }
            }
        }

        public ViewModelProperty(string propertyName, T defaultValue, Action<string> propertyChangeAction)
        {
            _value = defaultValue;
            Name = propertyName;
            _propChangeAction = propertyChangeAction;
            DependedProperties = new Dictionary<string, Action<string>>();
        }



        public void NotifyAllDepended()
        {
            foreach (var dependedProp in DependedProperties)
            {
                dependedProp.Value?.Invoke(dependedProp.Key);
            }
        }
    }
}