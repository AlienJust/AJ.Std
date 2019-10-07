using AlienJust.Support.Conversion.Contracts;

namespace AlienJust.Support.Conversion {
	public sealed class BuilderOneToOneNullableString<TRaw> : IBuilderOneToOne<TRaw?, string> where TRaw : struct {
		private readonly IBuilderOneToOne<TRaw, string> _builder;
		private readonly string _nullValue;

		public BuilderOneToOneNullableString(IBuilderOneToOne<TRaw, string> builder, string nullValue) {
			_builder = builder;
			_nullValue = nullValue;
		}

		public string Build(TRaw? source) {
			if (!source.HasValue) return _nullValue;
			return _builder.Build(source.Value);
		}
	}
}