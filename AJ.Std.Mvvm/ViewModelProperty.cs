using System;
using System.Collections.Generic;

namespace AJ.Std.Mvvm
{
	public class ViewModelProperty<TT> where TT : struct {
		public string Name { get; }
		private readonly Action<string> _propChangeAction;

		public Dictionary<string, Action<string>> DependedProperties { get; }

		private TT _value;
		public TT Value {
			get => _value;
			set {
				if (_value.Equals(value)) {
					_value = value;
					_propChangeAction?.Invoke(Name);
					NotifyAllDepended();
				}
			}
		}

		public ViewModelProperty(string propertyName, TT defaultValue, Action<string> propertyChangeAction) {
			_value = defaultValue;
			Name = propertyName;
			_propChangeAction = propertyChangeAction;
			DependedProperties = new Dictionary<string, Action<string>>();
		}



		public void NotifyAllDepended() {
			foreach (var dependedProp in DependedProperties) {
				dependedProp.Value?.Invoke(dependedProp.Key);
			}
		}
	}
}