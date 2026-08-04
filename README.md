# Snavi

Snavi is a [navi](https://github.com/denisidoro/navi)-like interactive command-line cheatsheet tool but it's more **s**afe with **s**tructured cheat file and C**S**harp script support.

Check `sample-cheats` for samples. It could be run with:

```sh
dotnet path/to/Snavi.dll -- run -c git/commit.json -c git/checkout.json -c cat/cat.json
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
