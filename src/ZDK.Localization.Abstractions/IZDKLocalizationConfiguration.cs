using System.Globalization;

namespace ZDK.Localization.Abstractions;

public interface IZDKLocalizationConfiguration
{
	public CultureInfo DefaultCulture { get; }
	public CultureInfo[] SupportedCultures { get; }
	public ZDKMissingLocalizationKeyHandleMethod MissingLocalizationKeyHandleMethod { get; }
}