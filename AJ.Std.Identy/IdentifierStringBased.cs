using AJ.Std.Identy.Contracts;

namespace AJ.Std.Identy {
  public sealed class IdentifierStringBased : IIdentifier {
    public IdentifierStringBased(string identyString) {
      IdentyString = identyString;
    }

    public string IdentyString { get; }

    public override string ToString() {
      return IdentyString;
    }
  }
}