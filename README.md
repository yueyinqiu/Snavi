# Snavi

Snavi is a [navi](https://github.com/denisidoro/navi)-like interactive command-line cheatsheet tool but it's more **s**afe with **s**tructured cheat file and C**S**harp script support.

## Installation (Nix)

A package is exposed in `flake.nix` as `packages.<system>.default`. To use it as a flake input (in home-manager as an example):

```nix
{
  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
    home-manager.url = "github:nix-community/home-manager";
    snavi.url = "github:yueyinqiu/Snavi";
  };

  outputs =
    {
      nixpkgs,
      home-manager,
      snavi,
      ...
    }:
    let
      system = <your-system>;    # e.g. x86_64-linux
    in
    {
      homeConfigurations.<your-name> = home-manager.lib.homeManagerConfiguration {
        pkgs = nixpkgs.legacyPackages.${system};
        extraSpecialArgs = {
          snavi = snavi.packages.${system}.default;
        };
        modules = [
          ({ pkgs, snavi, ... }: {
            home.packages = [
              snavi
              pkgs.fzf
              pkgs.dotnetCorePackages.sdk_10_0
            ];
          })
          ({ ... }: {
            home.username = <your-name>;
            home.homeDirectory = <path/to/your/home>;
            home.stateVersion = <your-home-manager-state-version>;
          })
        ];
      };
    };
}
```

(I plan to implement a home-manager module, but probably not now.)

If you are using NUR, it's also provided in [my personal NUR](https://github.com/yueyinqiu/MyNurPackages) so you can easily access it.

## Installation (Others)

Download `Snavi.zip` in the Releases page, decompress it, and you will find a `Snavi.dll`.

Note that `fzf` and `dotnet` (sdk, >=10) are required.

## Usage

Check `sample-cheats` for cheat samples.

Then it could be run with:

```sh
dotnet path/to/Snavi.dll -- run -c path/to/git/commit.json -c path/to/git/checkout.json -c path/to/cat/cat.json
```

It's recommended to be used as a shell widget, you could add this to `.bashrc` and press `Ctrl + g`:

```sh
_snavi_bind() {
    local snavi="path/to/Snavi.dll"
    local cheats=(
        path/to/cheat1.json
        path/to/cheat2.json
    )

    local args=()
    for cheat in "${cheats[@]}"; do
        args+=("-c" "$cheat")
    done
    READLINE_LINE="$(dotnet "$snavi" run "${args[@]}")"
    READLINE_POINT=${#READLINE_LINE}
}
bind -x '"\C-g": _snavi_bind'
```
