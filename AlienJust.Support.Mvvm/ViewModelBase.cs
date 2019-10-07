using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace AlienJust.Support.Mvvm
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RaisePropertyChanged<T>(Expression<Func<T>> property)
        {
            RaisePropertyChanged(CompaRiser.GetPropName(property));
        }

        public void SetProp<T>(Func<bool> comparer, Action assignAction, Expression<Func<T>> property)
        {
            CompaRiser.CompaRise(comparer, assignAction, RaisePropertyChanged, property);
        }

        public string GetPropName<T>(Expression<Func<T>> property)
        {
            return CompaRiser.GetPropName(property);
        }
    }
}