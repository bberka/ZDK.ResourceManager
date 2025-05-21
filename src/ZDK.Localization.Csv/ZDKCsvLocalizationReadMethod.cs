namespace ZDK.Localization.Csv;

/// <summary>
/// Specifies the method used to read localization data from CSV files.
/// </summary>
public enum ZDKCsvLocalizationReadMethod
{
	/// <summary>
	/// Localization data is stored in a single CSV file where columns represent different cultures. In this case source is the file path.
	/// </summary>
	SingleFileWithAllCultures,

	/// <summary>
	/// Localization data is stored in multiple CSV files, where each file contains data for a single culture. In this case source is the directory path.
	/// </summary>
	MultipleFilesEachWithOneCulture,
}