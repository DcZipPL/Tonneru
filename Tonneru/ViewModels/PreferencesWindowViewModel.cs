using System.Globalization;
using System.IO;
using ReactiveUI;
using Tomlyn;
using Tonneru.Models;

namespace Tonneru.ViewModels;

public class PreferencesWindowViewModel : ViewModelBase
{
	public PreferencesWindowViewModel()
	{
		SelectedCulture = Assets.Resources.Culture;
	}
	
	public CultureInfo _selectedCulture;
	
	public CultureInfo SelectedCulture
	{
		get => _selectedCulture;
		set => this.RaiseAndSetIfChanged(ref _selectedCulture, value);
	}
	
	public CultureInfo[] Cultures
	{
		get => App.SupportedCultures;
	}
	
	public void ApplyCulture()
	{
		Assets.Resources.Culture = SelectedCulture;
		CultureInfo.CurrentCulture = SelectedCulture;
		CultureInfo.CurrentUICulture = SelectedCulture;
		Preference config = new()
		{
			Language = SelectedCulture.Name
		};
		var toml = Toml.FromModel(config);
		File.WriteAllText("preferences.toml", toml);
	}
}