{
  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
  };

  outputs = { nixpkgs, ... }: let
    supportedSystems = [ "x86_64-linux" "aarch64-linux" "x86_64-darwin" "aarch64-darwin" ];
    forAllSystems = nixpkgs.lib.genAttrs supportedSystems;
  in {
    devShells = forAllSystems (system: let
      pkgs = nixpkgs.legacyPackages.${system};
    in {
      default = pkgs.mkShell {
        packages = [
          pkgs.dotnetCorePackages.sdk_10_0
          pkgs.fzf
          (pkgs.writeShellScriptBin "snavi-dev-publish" ''
            dotnet publish src/Snavi/Snavi.csproj -c Release -o publish
          '')
        ];
        shellHook = ''
          export DOTNET_ROOT="${pkgs.dotnetCorePackages.sdk_10_0}/share/dotnet"
        '';
      };
    });
  };
}
