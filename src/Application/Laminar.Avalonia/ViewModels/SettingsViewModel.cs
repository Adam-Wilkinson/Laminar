using System;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Laminar.Avalonia.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty] private string _theme = nameof(ThemeVariant.Default);
    [ObservableProperty] private double _fontSizeScale = 1.0;

    public SettingsViewModel()
    {
        if (Application.Current?.RequestedThemeVariant is null) return;

        Theme = Application.Current.RequestedThemeVariant.Key.ToString()!;
    }

    public string[] Themes { get; } =
        [nameof(ThemeVariant.Default), nameof(ThemeVariant.Dark), nameof(ThemeVariant.Light)];

    public double TransitionDuration
    {
        get => Application.Current is not null 
               && Application.Current.Resources.TryGetResource("AnimationDuration", Application.Current.ActualThemeVariant, out var durationObject) 
               && durationObject is TimeSpan duration
            ? duration.TotalMilliseconds
            : default;
        set
        {
            if (Application.Current is not null)
            {
                Application.Current.Resources["AnimationDuration"] = TimeSpan.FromMilliseconds(value);
            }
        }
    }
    
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

    partial void OnFontSizeScaleChanged(double value)
    {
        if (Application.Current is null) return;
        
        Application.Current.Resources["H1FontSize"] = value * 34;
        Application.Current.Resources["H2FontSize"] = value * 24;
        Application.Current.Resources["B1FontSize"] = value * 18;
        Application.Current.Resources["B2FontSize"] = value * 14;
    }
}