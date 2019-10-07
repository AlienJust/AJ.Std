using System;
using System.Linq.Expressions;

namespace AlienJust.Support.Reflection {
	/// <summary>
	/// Allows to get useful information about properties
	/// </summary>
	public static class ReflectedProperty {
		public static string GetName<T>(Expression<Func<T>> property) {
			var expression = GetMemberInfo(property);
			return expression.Member.Name;
		}

		public static string GetFullName<T>(Expression<Func<T>> property) {
			var expression = GetMemberInfo(property);
			if (expression.Member.DeclaringType != null) return string.Concat(expression.Member.DeclaringType.FullName, ".", expression.Member.Name);
			throw new NullReferenceException(nameof(expression.Member.DeclaringType));
		}

		private static MemberExpression GetMemberInfo(Expression method) {
			if (!(method is LambdaExpression lambda))
				throw new Exception(nameof(method) + " must be of type " + typeof(LabelExpression).FullName);

			MemberExpression memberExpr;

			switch (lambda.Body.NodeType) {
				case ExpressionType.Convert:
					memberExpr =
						((UnaryExpression)lambda.Body).Operand as MemberExpression;
					break;
				case ExpressionType.MemberAccess:
					memberExpr = lambda.Body as MemberExpression;
					break;
				default:
					throw new ArgumentException("method");
			}

				return memberExpr;
		}
	}
}
