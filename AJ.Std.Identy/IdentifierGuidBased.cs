using System;
using AJ.Std.Identy.Contracts;

namespace AJ.Std.Identy {
  // System depended
  public class IdentifierGuidBased : IIdentifier {
    private readonly Guid _guid;

    public IdentifierGuidBased(Guid guid) {
      _guid = guid;
    }

    public override string ToString() {
      return _guid.ToString().ToLower();
    }

    public string IdentyString => ToString();
  }
}
