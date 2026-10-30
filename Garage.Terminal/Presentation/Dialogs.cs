using System.Collections.ObjectModel;
using Garage.Terminal.Forms;
using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Garage.Terminal.Presentation;

internal sealed class Dialogs(IApplication app)
{
    public Window Modal(string title, int height = 22)
    {
        Window window = Theme.Apply(new Window
        {
            Title = title, X = Pos.Center(), Y = Pos.Center(), Width = Dim.Percent(88), Height = height,
            BorderStyle = LineStyle.Rounded
        });
        return window;
    }

    public void Message(string title, string message)
    {
        using Window window = Modal(title, 15);
        int contentHeight = Math.Max(8, message.Count(character => character == '\n') + 3);
        View scroll = Theme.Apply(new View
        {
            X = 2, Y = 1, Width = Dim.Fill(2), Height = Dim.Fill(4),
            CanFocus = true, ViewportSettings = ViewportSettingsFlags.HasScrollBars
        });
        scroll.SetContentSize(new System.Drawing.Size(100, contentHeight));
        scroll.Add(Theme.Apply(new Label { Text = message, Width = 100, Height = contentHeight }));
        window.Add(scroll);
        Button close = Theme.Button("_Close", () => app.RequestStop(window));
        close.X = Pos.Center(); close.Y = Pos.AnchorEnd(2);
        window.Add(close);
        app.Run(window);
    }

    public bool Confirm(string title, string message, string accept = "_Continue")
    {
        bool confirmed = false;
        using Window window = Modal(title, 12);
        window.Add(Theme.Apply(new Label { Text = message, X = 2, Y = 1, Width = Dim.Fill(2), Height = 5 }));
        Button cancel = Theme.Button("_Cancel", () => app.RequestStop(window));
        cancel.X = 2; cancel.Y = Pos.AnchorEnd(2);
        Button proceed = Theme.Button(accept, () => { confirmed = true; app.RequestStop(window); });
        proceed.X = Pos.Right(cancel) + 3; proceed.Y = cancel.Y;
        window.Add(cancel, proceed);
        app.Run(window);
        return confirmed;
    }

    public int? Choose(string title, IReadOnlyList<string> choices, int selected = 0)
    {
        if (choices.Count == 0) { Message(title, "There are no options available yet."); return null; }
        int? result = null;
        using Window window = Modal(title);
        ListView list = Theme.Apply(new ListView { X = 1, Y = 1, Width = Dim.Fill(1), Height = Dim.Fill(4), CanFocus = true });
        list.SetSource(new ObservableCollection<string>(choices));
        list.RowRender += (_, row) => row.RowAttribute = row.Row == list.SelectedItem ? Theme.Surface.Focus : Theme.Surface.Normal;
        list.SelectedItem = Math.Clamp(selected, 0, choices.Count - 1);
        void Select() { result = list.SelectedItem; app.RequestStop(window); }
        list.Accepted += (_, e) => { e.Handled = true; Select(); };
        Button choose = Theme.Button("_Select", Select);
        choose.X = 2; choose.Y = Pos.AnchorEnd(2);
        Button cancel = Theme.Button("_Cancel", () => app.RequestStop(window));
        cancel.X = Pos.Right(choose) + 3; cancel.Y = choose.Y;
        window.Add(list, choose, cancel);
        app.Run(window);
        return result;
    }

    public string? JsonFile(string title)
    {
        using OpenDialog dialog = Theme.Apply(new OpenDialog
        {
            Title = title,
            Path = Path.Combine(AppContext.BaseDirectory, "Data"),
            AllowsMultipleSelection = false,
            AllowedTypes = [new AllowedType("JSON files", ".json")]
        });
        app.Run(dialog);
        return dialog.Canceled ? null : dialog.FilePaths.FirstOrDefault();
    }

    public IReadOnlyList<int>? ChooseMany(string title, IReadOnlyList<string> choices, List<int>? selected = null)
    {
        IReadOnlyList<int>? result = null;
        using Window window = Modal(title);
        window.Add(Theme.Apply(new Label { X = 1, Y = 0, Text = "Space toggles a choice. No marks means all types.", Width = Dim.Fill(1) }, Theme.Muted));
        ListView list = Theme.Apply(new ListView { X = 1, Y = 2, Width = Dim.Fill(1), Height = Dim.Fill(4), ShowMarks = true, MarkMultiple = true, CanFocus = true });
        list.SetSource(new ObservableCollection<string>(choices));
        foreach (int index in selected ?? []) list.Source?.SetMark(index, true);
        Button apply = Theme.Button("_Apply", () => { result = list.GetAllMarkedItems().ToList(); app.RequestStop(window); });
        apply.X = 2; apply.Y = Pos.AnchorEnd(2);
        Button cancel = Theme.Button("_Cancel", () => app.RequestStop(window));
        cancel.X = Pos.Right(apply) + 2; cancel.Y = apply.Y;
        window.Add(list, apply, cancel);
        app.Run(window);
        return result;
    }

    public Dictionary<string, object?>? Form(string title, IReadOnlyList<FieldDefinition> fields,
        string finish = "_Continue", IReadOnlyDictionary<string, object?>? initial = null)
    {
        Dictionary<string, string> text = fields.ToDictionary(field => field.Name,
            field => initial?.GetValueOrDefault(field.Name)?.ToString() ?? field.Initial);
        int page = 0;
        int pages = Math.Max(1, (fields.Count + 5) / 6);
        while (true)
        {
            int direction = 0;
            using Window window = Modal($"{title}  /  Step {page + 1} of {pages}", 23);
            window.Add(Theme.Apply(new Label
            {
                X = 2, Y = 0, Width = Dim.Fill(2), Height = 2,
                Text = "Tab moves between fields. Enter opens choices. Esc cancels.",
            }, Theme.Muted));
            List<FieldDefinition> current = fields.Skip(page * 6).Take(6).ToList();
            Dictionary<string, View> controls = [];
            for (int index = 0; index < current.Count; index++)
            {
                FieldDefinition field = current[index];
                int y = 3 + index * 2;
                window.Add(Theme.Apply(new Label { Text = field.Label + (field.Required ? " *" : ""), X = 2, Y = y, Width = 29 }));
                Type type = Nullable.GetUnderlyingType(field.Type) ?? field.Type;
                View control;
                if (type.IsEnum || type == typeof(bool))
                {
                    Button choice = Theme.Button(text[field.Name].Length == 0 ? "Any" : text[field.Name], () => { });
                    choice.Accepted += (_, _) =>
                    {
                        List<string> options = type == typeof(bool)
                            ? ["Yes", "No"]
                            : Enum.GetNames(type).Where(n => n != "Unspecified").ToList();
                        if (!field.Required) options = ["Any", .. options];
                        int? selected = Choose(field.Label, options, Math.Max(0, options.IndexOf(text[field.Name])));
                        if (selected is int pick)
                        {
                            text[field.Name] = options[pick] == "Any" ? "" : options[pick];
                            choice.Text = options[pick];
                        }
                    };
                    control = choice;
                }
                else control = Theme.Apply(new TextField { Text = text[field.Name] }, Theme.Input);
                control.X = 32; control.Y = y; control.Width = Dim.Fill(2);
                controls[field.Name] = control;
                window.Add(control);
            }
            Label error = Theme.Apply(new Label { X = 2, Y = 16, Width = Dim.Fill(2), Height = 3 }, Theme.Danger);
            window.Add(error);
            void Remember()
            {
                foreach (FieldDefinition field in current)
                    if (controls[field.Name] is TextField edit) text[field.Name] = edit.Text;
            }
            Button cancel = Theme.Button("_Cancel", () => { direction = 0; app.RequestStop(window); });
            cancel.X = 2; cancel.Y = Pos.AnchorEnd(2);
            Button back = Theme.Button("_Back", () => { Remember(); direction = -1; app.RequestStop(window); });
            back.Enabled = page > 0; back.X = Pos.Right(cancel) + 2; back.Y = cancel.Y;
            Button next = Theme.Button(page == pages - 1 ? finish : "_Next", () =>
            {
                Remember();
                foreach (FieldDefinition field in current)
                {
                    try { field.Parse(text[field.Name]); }
                    catch (ArgumentException exception) { error.Text = exception.Message; controls[field.Name].SetFocus(); return; }
                }
                direction = 1; app.RequestStop(window);
            });
            next.X = Pos.Right(back) + 2; next.Y = cancel.Y;
            window.Add(cancel, back, next);
            app.Run(window);
            if (direction == 0) return null;
            page += direction;
            if (page == pages) return fields.ToDictionary(field => field.Name, field => field.Parse(text[field.Name]));
        }
    }
}
