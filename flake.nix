{
  description = "A Nix-flake-based C# development environment";

  inputs.nixpkgs.url = "github:nixos/nixpkgs/nixos-unstable"; # "https://flakehub.com/f/NixOS/nixpkgs/0.1.*.tar.gz";

  outputs = {
    self,
    nixpkgs,
  }: let
    supportedSystems = ["x86_64-linux" "aarch64-linux" "x86_64-darwin" "aarch64-darwin"];
    forEachSupportedSystem = f:
      nixpkgs.lib.genAttrs supportedSystems (system:
        f {
          pkgs = import nixpkgs {inherit system;};
        });
  in {
    devShells = forEachSupportedSystem ({pkgs}: {
      default = pkgs.mkShell {
        packages = with pkgs; [
          #dotnet-sdk_6
          #dotnet-sdk_7
          #dotnet-sdk_8
          (with dotnetCorePackages;
            combinePackages [
              #sdk_6_0
              #sdk_7_0
              sdk_8_0
              sdk_10_0
            ])
          #omnisharp-roslyn
          mono
          #msbuild
        ];
      };
    });
  };
}
