using System;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Laminar.Avalonia.ViewModels;

public partial class MainControlViewModel : ViewModelBase
{
    [ObservableProperty] private bool _sidebarExpanded = true;
}