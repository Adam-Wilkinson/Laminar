using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Laminar.Avalonia.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty] private string _theme = nameof(ThemeVariant.Default);

    public SettingsViewModel()
    {
        if (Application.Current?.RequestedThemeVariant is null) return;

        Theme = Application.Current.RequestedThemeVariant.Key.ToString()!;
    }

    public string[] Themes { get; } =
        [nameof(ThemeVariant.Default), nameof(ThemeVariant.Dark), nameof(ThemeVariant.Light)];

    partial void OnThemeChanged(string value)
    {
        if (Application.Current is null) return;

        Application.Current.RequestedThemeVariant = value switch
        {
            nameof(ThemeVariant.Light) => ThemeVariant.Light,
            nameof(ThemeVariant.Dark) => ThemeVariant.Dark,
            _ => ThemeVariant.Default,
        };
    }
}