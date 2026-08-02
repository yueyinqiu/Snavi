#pragma warning disable CS0618
using System.Collections.ObjectModel;
using FuzzySharp;
using Terminal.Gui;
using Terminal.Gui.App;
using Terminal.Gui.Input;
using Terminal.Gui.Views;
using Terminal.Gui.ViewBase;

namespace Snavi.Interaction;

public sealed class TerminalUi : IUi
{
    private readonly List<string> _warnings = [];
    private bool _initialized;

    public int? Pick(string prompt, IReadOnlyList<PickerItem> items)
    {
        EnsureInitialized();
        return RunPicker(prompt, items);
    }

    public string? Prompt(string message)
    {
        EnsureInitialized();
        var dialog = new Dialog { Title = message, Width = Dim.Percent(60), Height = Dim.Percent(20) };
        var input = new TextField { X = 0, Y = 0, Width = Dim.Fill() };
        dialog.Add(input);
        string? result = null;
        dialog.Accepted += (_, _) =>
        {
            result = input.Value;
            Application.RequestStop(dialog);
        };
        dialog.KeyDown += (_, e) =>
        {
            if (e == Key.Esc)
                Application.RequestStop(dialog);
        };
        Application.Run(dialog);
        return result;
    }

    public void Warn(string message) => _warnings.Add(message);

    public void Close()
    {
        if (!_initialized)
            return;
        _initialized = false;
        Application.Shutdown();
        foreach (var warning in _warnings)
            Console.Error.WriteLine($"警告: {warning}");
        _warnings.Clear();
    }

    private void EnsureInitialized()
    {
        if (_initialized)
            return;
        Application.Init();
        _initialized = true;
    }

    private int? RunPicker(string prompt, IReadOnlyList<PickerItem> items)
    {
        var dialog = new Dialog
        {
            Title = prompt,
            Width = Dim.Percent(82),
            Height = Dim.Percent(76),
        };
        var query = new TextField { X = 0, Y = 0, Width = Dim.Fill() };
        var list = new ListView { X = 0, Y = 2, Width = Dim.Percent(50), Height = Dim.Fill() };
        var preview = new TextView
        {
            ReadOnly = true,
            X = Pos.Percent(50),
            Y = 2,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };
        dialog.Add(query, list, preview);

        var source = new ObservableCollection<string>();
        list.SetSource(source);
        var order = new List<int>();

        void UpdatePreview()
        {
            var selected = list.SelectedItem;
            preview.Text = selected is { } s && s >= 0 && s < order.Count && items[order[s]].Preview is { } p
                ? p
                : string.Empty;
        }

        void ApplyFilter(string query)
        {
            order.Clear();
            source.Clear();
            if (string.IsNullOrEmpty(query))
            {
                for (var i = 0; i < items.Count; i++)
                {
                    order.Add(i);
                    source.Add(items[i].Display);
                }
            }
            else
            {
                var matches = items
                    .Select((item, i) => (
                        i,
                        item,
                        Score: Math.Max(
                            Fuzz.PartialRatio(query, item.Display),
                            Fuzz.PartialRatio(query, item.Preview ?? string.Empty))))
                    .Where(x => x.Score >= 30)
                    .OrderByDescending(x => x.Score)
                    .ThenBy(x => x.i)
                    .ToList();
                foreach (var (index, item, _) in matches)
                {
                    order.Add(index);
                    source.Add(item.Display);
                }
            }
            if (order.Count > 0)
                list.SelectedItem = 0;
            UpdatePreview();
        }

        int? result = null;
        query.ValueChanged += (_, e) => ApplyFilter(e.NewValue ?? string.Empty);
        list.ValueChanged += (_, e) => UpdatePreview();
        dialog.Accepted += (_, _) =>
        {
            var selected = list.SelectedItem;
            result = selected is { } s && s >= 0 && s < order.Count ? order[s] : null;
            Application.RequestStop(dialog);
        };
        dialog.KeyDown += (_, e) =>
        {
            if (e == Key.Esc)
                Application.RequestStop(dialog);
        };

        ApplyFilter(string.Empty);
        Application.Run(dialog);
        return result;
    }
}
