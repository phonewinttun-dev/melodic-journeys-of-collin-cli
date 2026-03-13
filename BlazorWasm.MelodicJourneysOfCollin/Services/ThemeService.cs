using Microsoft.JSInterop;

namespace BlazorWasm.MelodicJourneysOfCollin.Services;

public sealed class ThemeService
{
    private const string JsNamespace = "appTheme";

    public ThemePreference Preference { get; private set; } = ThemePreference.System;
    public bool IsDark { get; private set; }
    public bool IsInitialized { get; private set; }

    public event Action? Changed;

    public async Task InitializeAsync(IJSRuntime jsRuntime)
    {
        var snapshot = await jsRuntime.InvokeAsync<ThemeSnapshot>($"{JsNamespace}.getThemeState");
        ApplySnapshot(snapshot);
    }

    public async Task SetPreferenceAsync(ThemePreference preference, IJSRuntime jsRuntime)
    {
        var snapshot = await jsRuntime.InvokeAsync<ThemeSnapshot>($"{JsNamespace}.setPreference", ToJsValue(preference));
        ApplySnapshot(snapshot);
    }

    public async Task CycleAsync(IJSRuntime jsRuntime)
    {
        var next = Preference switch
        {
            ThemePreference.System => ThemePreference.Light,
            ThemePreference.Light => ThemePreference.Dark,
            _ => ThemePreference.System
        };

        await SetPreferenceAsync(next, jsRuntime);
    }

    private void ApplySnapshot(ThemeSnapshot snapshot)
    {
        Preference = snapshot.Preference?.ToLowerInvariant() switch
        {
            "light" => ThemePreference.Light,
            "dark" => ThemePreference.Dark,
            _ => ThemePreference.System
        };

        IsDark = snapshot.IsDark;
        IsInitialized = true;
        Changed?.Invoke();
    }

    private static string ToJsValue(ThemePreference preference) => preference switch
    {
        ThemePreference.Light => "light",
        ThemePreference.Dark => "dark",
        _ => "system"
    };

    private sealed class ThemeSnapshot
    {
        public string? Preference { get; set; }
        public bool IsDark { get; set; }
    }
}
