using Avalonia;
using Avalonia.Controls;

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
	
	public static readonly DirectProperty<IconButton, double> SpacingProperty =
		AvaloniaProperty.RegisterDirect<IconButton, double>(
			nameof(Spacing),
			o => o.Spacing,
			(o, v) => o.Spacing = v,
			6);
	private double _spacing = 6;
	public double Spacing
	{
		get
		{
			if (Content is null or "" or TextBlock { Text: null or "" })
				return 0;
			return _spacing;
		}
		private set => SetAndRaise(SpacingProperty, ref _spacing, value);
	}
}