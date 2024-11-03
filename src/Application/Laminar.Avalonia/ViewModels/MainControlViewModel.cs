using System;
using System.Collections.ObjectModel;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Laminar.Avalonia.ViewModels;

public partial class MainControlViewModel : ViewModelBase
{
    [ObservableProperty] private bool _sidebarExpanded = true;

    public ObservableCollection<TreeTester> Files { get; } =
    [
        new("Root File", [
            new("Sub File One"), 
            new("Sub File Two", [
                new("Sub Sub File One"),
                new("Sub Sub File Two")
            ])
        ])
    ];

    public class TreeTester
    {
        public ObservableCollection<TreeTester>? SubNodes { get; }
        public string Title { get; }

        public TreeTester(string title)
        {
            Title = title;
        }

        public TreeTester(string title, ObservableCollection<TreeTester> subNodes)
            : this(title)
        {
            SubNodes = subNodes;
        }
    }
}