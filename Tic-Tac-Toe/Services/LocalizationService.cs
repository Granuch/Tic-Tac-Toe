using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Tic_Tac_Toe.Services;

public interface ILocalizationService : INotifyPropertyChanged
{
    string CurrentLanguage { get; }
    void SetLanguage(string cultureCode);
    string GetString(string key);
}

public class LocalizationService : ILocalizationService
{
    private const string SettingsFileName = "language.settings";
    private const string LangDictionaryPrefix = "/Resources/Lang.";
    
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "TicTacToe",
        SettingsFileName);

    public event PropertyChangedEventHandler? PropertyChanged;

    public string CurrentLanguage { get; private set; } = "en";

    public LocalizationService()
    {
        LoadSavedLanguage();
    }

    public void SetLanguage(string cultureCode)
    {
        CurrentLanguage = cultureCode;
        var culture = new CultureInfo(cultureCode);
        
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
        
        // Swap ResourceDictionary for dynamic UI update
        SwapLanguageDictionary(cultureCode);
        
        SaveLanguage(cultureCode);
        OnPropertyChanged(nameof(CurrentLanguage));
    }

    public string GetString(string key)
    {
        // Get string from current Application resources
        if (Application.Current?.Resources[key] is string value)
        {
            return value;
        }
        return key;
    }

    private void SwapLanguageDictionary(string cultureCode)
    {
        if (Application.Current == null) return;

        var newDictUri = new Uri($"pack://application:,,,{LangDictionaryPrefix}{cultureCode}.xaml");
        var newDict = new ResourceDictionary { Source = newDictUri };

        // Find and remove old language dictionary
        var mergedDicts = Application.Current.Resources.MergedDictionaries;
        var oldDict = mergedDicts
            .FirstOrDefault(d => d.Source?.OriginalString.Contains(LangDictionaryPrefix) == true);

        if (oldDict != null)
        {
            mergedDicts.Remove(oldDict);
        }

        // Add new language dictionary
        mergedDicts.Add(newDict);
    }

    private void LoadSavedLanguage()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var savedCulture = File.ReadAllText(SettingsPath).Trim();
                if (!string.IsNullOrEmpty(savedCulture))
                {
                    SetLanguage(savedCulture);
                    return;
                }
            }
        }
        catch
        {
            // Use default if settings can't be loaded
        }
        
        // Default to English
        SetLanguage("en");
    }

    private void SaveLanguage(string cultureCode)
    {
        try
        {
            var directory = Path.GetDirectoryName(SettingsPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(SettingsPath, cultureCode);
        }
        catch
        {
            // Ignore save errors
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
