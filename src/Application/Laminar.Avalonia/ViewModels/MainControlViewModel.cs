using System;
using System.Collections.ObjectModel;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Laminar.Avalonia.ViewModels;

public partial class MainControlViewModel : ViewModelBase
{
    [ObservableProperty] private bool _sidebarExpanded = true;

    public class TreeTester
    {
        public ObservableCollection<TreeTester>? SubNodes { get; }
        public string Title { get; }

        public TreeTester(string title)
        {
            Title = title;
        }

        public TreeTester(ObservableCollection<TreeTester> subNodes, string title)
            : this(title)
        {
            SubNodes = subNodes;
        }
    }
}