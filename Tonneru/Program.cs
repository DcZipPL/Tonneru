using Avalonia;
using Avalonia.ReactiveUI;
using System;
using System.Globalization;
using System.IO;
using Tomlyn;
using Tonneru.Models;

namespace Tonneru
{
	class Program
	{
		// Initialization code. Don't use any Avalonia, third-party APIs or any
		// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
		// yet and stuff might break.
		[STAThread]
		public static void Main(string[] args)
		{
			if (File.Exists("preferences.toml"))
			{
				try
				{
					var toml = File.ReadAllText("preferences.toml");
					var ci = new CultureInfo(Toml.ToModel<Preference>(toml).Language);
					Tonneru.Assets.Resources.Culture = ci;
					CultureInfo.CurrentCulture = ci;
					CultureInfo.CurrentUICulture = ci;
				}
				catch
				{
					Console.WriteLine("Failed to load configuration file.");
				}
			}
			
			BuildAvaloniaApp()
				.StartWithClassicDesktopLifetime(args);
		}

		// Avalonia configuration, don't remove; also used by visual designer.
		public static AppBuilder BuildAvaloniaApp()
			=> AppBuilder.Configure<App>()
				.UsePlatformDetect()
				.LogToTrace()
				.UseReactiveUI();
	}
}