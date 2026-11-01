using Terminal.Gui.ViewBase;

namespace Garage.Terminal.Components;

// Pages belong to the controller. Switching pages keeps their input and focus state available.
public sealed class ScreenHost : View
{
    public View? CurrentPage { get; private set; }

    public ScreenHost()
    {
        CanFocus = true;
        Width = Dim.Fill();
        Height = Dim.Fill();
    }

    public void ShowPage(View page)
    {
        ArgumentNullException.ThrowIfNull(page);
        if (ReferenceEquals(page, CurrentPage))
            return;
        if (ReferenceEquals(page, this))
            throw new ArgumentException("A screen cannot display itself.", nameof(page));
        if (page.SuperView is not null)
            throw new ArgumentException("Remove the page from its current view before displaying it here.", nameof(page));

        if (CurrentPage is not null)
            Remove(CurrentPage);
        CurrentPage = page;
        page.X = 0;
        page.Y = 0;
        page.Width = Dim.Fill();
        page.Height = Dim.Fill();
        Add(page);
        page.SetFocus();
        SetNeedsLayout();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && CurrentPage is not null)
        {
            Remove(CurrentPage);
            CurrentPage = null;
        }
        base.Dispose(disposing);
    }
}
