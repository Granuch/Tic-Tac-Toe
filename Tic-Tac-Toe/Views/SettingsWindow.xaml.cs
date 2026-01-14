using System.Windows;
using System.Windows.Controls;
using Tic_Tac_Toe.Services;

namespace Tic_Tac_Toe.Views;

public partial class SettingsWindow : Window
{
    private readonly ILocalizationService _localizationService;
    private bool _isInitializing = true;

    public SettingsWindow(ILocalizationService localizationService)
    {
        InitializeComponent();
        
        _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
        
        // Set current language selection
        foreach (ComboBoxItem item in CmbLanguage.Items)
        {
            if (item.Tag?.ToString() == _localizationService.CurrentLanguage)
            {
                CmbLanguage.SelectedItem = item;
                break;
            }
        }
        
        _isInitializing = false;
    }

    private void CmbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isInitializing) return;
        
        if (CmbLanguage.SelectedItem is ComboBoxItem selectedItem && 
            selectedItem.Tag is string cultureCode)
        {
            _localizationService.SetLanguage(cultureCode);
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
