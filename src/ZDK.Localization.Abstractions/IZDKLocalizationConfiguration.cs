using System.Globalization;
using System.Text;

namespace ZDK.Localization.Abstractions;

public interface IZDKLocalizationConfiguration
{
	public CultureInfo DefaultCulture { get; }
	public CultureInfo[] SupportedCultures { get; }
	public ZDKMissingLocalizationKeyHandleMethod MissingLocalizationKeyHandleMethod { get; }
	public Encoding Encoding { get;  }
}