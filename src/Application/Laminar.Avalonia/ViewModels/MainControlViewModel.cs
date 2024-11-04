using System;
using System.Collections.ObjectModel;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Laminar.Avalonia.ViewModels;

public partial class MainControlViewModel(FileNavigatorViewModel fileNavigator) : ViewModelBase
{
    [ObservableProperty] private bool _sidebarExpanded = true;

    public FileNavigatorViewModel FileNavigator { get; } = fileNavigator;
}