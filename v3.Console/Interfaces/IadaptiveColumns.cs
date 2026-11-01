using System;
using System.Collections.Generic;
using System.Text;

namespace Garage.Terminal.Interfaces
{
    public interface IAdaptiveComponent
    {
        int MinimumWidth { get; }
        int PreferredWidth { get; }

        int MeasureHeight(int availableWidth);

        event Action? SizeHintsChanged;
    }
}
