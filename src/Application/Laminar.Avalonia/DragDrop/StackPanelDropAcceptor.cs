using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Laminar.Avalonia.DragDrop;

public class StackPanelDropAcceptor : DropAcceptor<StackPanel>
{
    public double ReceptacleAreaFraction { get; set; } = 0.67;

    protected override IEnumerable<Receptacle> GetReceptacles(StackPanel stackPanel)
    {
        var isHorizontal = stackPanel.Orientation == Orientation.Horizontal;
        var heightOfFirstReceptacle =
            (isHorizontal ? stackPanel.Children[0].Bounds.Width : stackPanel.Children[0].Bounds.Height) *
            ReceptacleAreaFraction / 2;
        yield return new Receptacle(
            new RectangleGeometry(isHorizontal
                ? new Rect(0, 0, heightOfFirstReceptacle, stackPanel.DesiredSize.Height)
                : new Rect(0, 0, stackPanel.DesiredSize.Width, heightOfFirstReceptacle)), 0);
    }

    protected override IPen DebugReceptaclePen { get; } = new Pen(Brushes.Yellow, 1);
}