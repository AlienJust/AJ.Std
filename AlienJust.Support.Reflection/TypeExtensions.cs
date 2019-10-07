using System;
using System.IO;
using System.Reflection;

namespace AlienJust.Support.Reflection
{
	public static class TypeExtensions {
		public static string GetAssemblyDirectoryPath(this Type type) {
			return Path.GetDirectoryName(Assembly.GetAssembly(type).Location);
		}
	}
}