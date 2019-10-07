using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace AlienJust.Support.Mvvm
{
    /// <summary>
    /// Extends RelayCommand with "depending" features
    /// </summary>
    public class DependedCommand : RelayCommand
    {
        /// <summary>
        /// Properties that's value changes affect on CanExecute state of this command
        /// </summary>
        public List<PropertyListener> DependOnProperties { get; }

        /// <summary>
        /// Commands thats CanExecute state is depend on this command CanExecute state
        /// </summary>
        public List<RelayCommand> DependedCommands { get; }

        public DependedCommand(Action execute, Func<bool> canExecute)
            : base(execute, canExecute)
        {
            DependOnProperties = new List<PropertyListener>();
            DependedCommands = new List<RelayCommand>();
        }

        public void AddDependOnProp(INotifyPropertyChanged vm, string propertyName)
        {
            DependOnProperties.Add(new PropertyListener(this, vm, propertyName));
        }
        //
        public void AddDependOnProp<T>(INotifyPropertyChanged vm, Expression<Func<T>> property)
        {
            DependOnProperties.Add(new PropertyListener(this, vm, CompaRiser.GetPropName(property)));
        }

        /// <summary>
        /// Manually check all depended commands CanExecute state
        /// </summary>
        public void NotifyCommands()
        {
            foreach (var cmd in DependedCommands)
            {
                cmd.RaiseCanExecuteChanged();
            }
            RaiseCanExecuteChanged();
        }
    }
}