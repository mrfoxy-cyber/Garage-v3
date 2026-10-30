namespace Garage.ConsoleApp.Menus;

internal delegate string MenuTextProvider();

internal sealed class MenuOption
{
    private readonly Action action;
    private readonly MenuTextProvider textProvider;

    public string Text => textProvider();

    public MenuOption(string text, Action action)
        : this(() => text, action)
    {
    }

    public MenuOption(MenuTextProvider textProvider, Action action)
    {
        this.textProvider = textProvider;
        this.action = action;
    }

    public MenuOption(string text, ConsoleMenu submenu)
        : this(text, submenu.Show)
    {
    }

    public void Execute()
    {
        action();
    }

    public override string ToString()
    {
        return Text;
    }
}
