{
  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
  };

  outputs =
    { nixpkgs, ... }:
    let
      supportedSystems = [
        "x86_64-linux"
        "aarch64-linux"
        "x86_64-darwin"
        "aarch64-darwin"
      ];
      forAllSystems = nixpkgs.lib.genAttrs supportedSystems;
    in
    {
      packages = forAllSystems (system: {
        default = nixpkgs.legacyPackages.${system}.callPackage ./nix { };
      });

      devShells = forAllSystems (
        system:
        let
          pkgs = nixpkgs.legacyPackages.${system};
        in
        {
          default = pkgs.mkShell {
            packages = [
              pkgs.dotnetCorePackages.sdk_10_0
              pkgs.fzf
              pkgs.ouch
              (pkgs.writeShellScriptBin "snavi-dev-publish" ''
                set -euo pipefail
                mkdir -p publish
                temp=$(mktemp -d -p publish)

                dotnet publish src/Snavi/Snavi.csproj -c Release -o "$temp/Snavi"
                ouch compress "$temp/Snavi"/* "$temp/Snavi.zip"

                dotnet pack src/Snavi.ArgumentSuggester/Snavi.ArgumentSuggester.csproj -o "$temp"
              '')
            ];
            shellHook = ''
              export DOTNET_ROOT="${pkgs.dotnetCorePackages.sdk_10_0}/share/dotnet"
            '';
          };
        }
      );
    };
}
