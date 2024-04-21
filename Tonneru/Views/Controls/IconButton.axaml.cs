using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Tonneru.Views.Controls;

public class IconButton : Button
{
	public static readonly StyledProperty<string> IconProperty =
		AvaloniaProperty.Register<IconButton, string>(nameof(Icon), defaultValue: "&#xE038;");
	public string Icon
	{
		get => GetValue(IconProperty);
		set => SetValue(IconProperty, value);
	}
}