using System.Globalization;
using ZDK.Localization.Abstractions;

namespace ZDK.Localization.Csv;

public class ZDKCsvLocalizationConfiguration : IZDKLocalizationConfiguration
{
	public required string CsvFilePath { get; init; }
	public required CultureInfo DefaultCulture { get; init; }
	public required CultureInfo[] SupportedCultures { get; init; }
	public required ZDKMissingLocalizationKeyHandleMethod MissingLocalizationKeyHandleMethod { get; init; }
}