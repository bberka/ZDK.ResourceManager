using System.Globalization;
using ZDK.Localization.Abstractions;

namespace ZDK.Localization.Csv;

/// <summary>
///  Represents the configuration for CSV localization.
/// </summary>
public class ZDKCsvLocalizationConfiguration : IZDKLocalizationConfiguration
{
	
	/// <summary>
	///  Specifies the path to the CSV file containing localization data.
	/// </summary>
	public required string CsvFilePath { get; init; }

	/// <summary>
	/// Defines the default culture to be used for localization
	/// when no culture is explicitly specified.
	/// </summary>
	public required CultureInfo DefaultCulture { get; init; }

	/// <summary>
	/// Specifies the cultures supported for localization.
	/// </summary>
	public required CultureInfo[] SupportedCultures { get; init; }

	/// <summary>
	/// Defines the strategy for handling missing localization keys.
	/// </summary>
	public required ZDKMissingLocalizationKeyHandleMethod MissingLocalizationKeyHandleMethod { get; init; }
	
	/// <summary>
	/// Reloads the localization data when the file changes.
	/// </summary>
	public required bool ReloadOnFileChange { get; set; }
}