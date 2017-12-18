using System;
using System.Linq.Expressions;
using AJ.Std.Reflection;

namespace AJ.Std.Mvvm {
	internal static class CompaRiser {
		/// <summary>
		/// Runs comparer function and if its result is true then it runs assign action, also raises prop changed action
		/// </summary>
		/// <param name="comparer">Comparer function</param>
		/// <param name="assignAction">Action if success</param>
		/// <param name="notifyPropertyChanged">Notify property changed action if success</param>
		/// <param name="property">Property, used like ()=>PropName</param>
		/// <returns>Result of comparer</returns>
		public static bool CompaRise<T>(Func<bool> comparer, Action assignAction, Action<string> notifyPropertyChanged, Expression<Func<T>> property) {
			var result = comparer.Invoke();
			if (result) {
				assignAction.Invoke();
				notifyPropertyChanged.Invoke(GetPropName(property));
			}
			return result;
		}

		public static string GetPropName<T>(Expression<Func<T>> property) {
			return ReflectedProperty.GetName(property);
		}
	}
}
