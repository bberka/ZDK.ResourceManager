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
	public required string SourcePath { get; init; }

	/// <summary>
	/// Defines the default culture to be used for localization when no culture is explicitly specified. Default is en-US.
	/// </summary>
	public CultureInfo DefaultCulture { get; init; } = new("en-US");

	/// <summary>
	/// Specifies the cultures supported for localization.
	/// </summary>
	public required CultureInfo[] SupportedCultures { get; init; }

	/// <summary>
	/// Defines the strategy for handling missing localization keys. Default is ReturnKey.
	/// </summary>
	public ZDKMissingLocalizationKeyHandleMethod MissingLocalizationKeyHandleMethod { get; init; } = ZDKMissingLocalizationKeyHandleMethod.ReturnKey;

	/// <summary>
	/// Reloads the localization data when the file changes. Default is true.
	/// </summary>
	public bool ReloadOnFileChange { get; init; } = true;
	
	/// <summary>
	/// Reloads the localization data when the file changes. Default is SingleFileWithAllCultures
	/// </summary>
	public ZDKCsvLocalizationReadMethod ReadMethod { get; init; } = ZDKCsvLocalizationReadMethod.SingleFileWithAllCultures;
	
	/// <summary>
	/// Gets the separator character used in the CSV file(s).
	/// Common values are ',' and ';' and '\t'. Default is ','.
	/// </summary>
	public char Separator { get; init;} = ',';
}