using Garage.Terminal.Components;
using Garage.Terminal.Contracts;
using Garage.Terminal.Theming;

namespace Garage.Terminal.Views;

public sealed class HomeView : PageView
{
    public CreateGarageInput CreateInput { get; }
    public IReadOnlyList<GarageCard> GarageCards { get; }

    public HomeView(GarageTheme theme, IReadOnlyList<GarageChoice> garages) : base("Home", theme, showBack: false)
    {
        CreateInput = new(theme);
        AddPanel("Create a garage", CreateInput);

        List<GarageCard> cards = [];

        foreach (GarageChoice garage in garages)
        {
            GarageCard card = new(garage, theme);

            cards.Add(card);
            AddPanel(garage.Name, card);
        }

        GarageCards = cards.AsReadOnly();
    }
}
