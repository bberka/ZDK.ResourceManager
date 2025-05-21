using System.Globalization;

namespace ZDK.Localization.Abstractions;

public interface IZDKLocalization : IDisposable
{
	/// <summary>
	///  Loads the localization resources from the specified source.
	/// </summary>
	/// <returns></returns>
	IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> GetLocalizationData();

	/// <summary>
	/// Gets localized string from the resource file
	/// </summary>
	/// <param name="key"> The key to look up the localized string.</param>
	/// <returns> The localized string.  </returns>
	string GetString(string key);

	/// <summary>
	///  Gets localized string from the resource file with the specified culture.
	/// </summary>
	/// <param name="key">The key to look up the localized string.</param>
	/// <param name="cultureInfo"> The culture info to use for localization.</param>
	/// <returns> The localized string with the specified culture.  </returns>
	string GetString(string key, CultureInfo cultureInfo);

	/// <summary>
	///  Gets localized string from the resource file with the specified culture and arguments.
	/// </summary>
	/// <param name="key">The key to look up the localized string.</param>
	/// <param name="args">The arguments to format the localized string. Which can be localization keys.</param>
	/// <returns> The localized string with the specified culture and arguments.  </returns>
	string GetString(string key, params object[] args);

	/// <summary>
	///  Gets localized string from the resource file with the specified culture, arguments and key.
	/// </summary>
	/// <param name="key">The key to look up the localized string.</param>
	/// <param name="cultureInfo">The culture info to use for localization.</param>
	/// <param name="args">The arguments to format the localized string. Which can be localization keys.</param>
	/// <returns>  The localized string with the specified culture and arguments.  </returns>
	string GetString(string key, CultureInfo cultureInfo, params object[] args);

	/// <summary>
	///  Gets localized string from the resource file using an enum key.
	/// </summary>
	/// <param name="keyEnum"> The enum key to look up the localized string.</param>
	/// <typeparam name="T"> The enum type.</typeparam>
	/// <returns> The localized string.  </returns>
	string GetString<T>(T keyEnum) where T : Enum;

	/// <summary>
	///  Gets localized string from the resource file using an enum key and culture.
	/// </summary>
	/// <param name="keyEnum">The enum key to look up the localized string.</param>
	/// <param name="cultureInfo">The culture info to use for localization.</param>
	/// <typeparam name="T">The enum type.</typeparam>
	/// <returns> The localized string with the specified culture.  </returns>
	string GetString<T>(T keyEnum, CultureInfo cultureInfo) where T : Enum;

	/// <summary>
	///  Gets localized string from the resource file using an enum key, culture and arguments.
	/// </summary>
	/// <param name="keyEnum">The enum key to look up the localized string.</param>
	/// <param name="args"> The arguments to format the localized string. Which can be localization keys.</param>
	/// <typeparam name="T">The enum type.</typeparam>
	/// <returns> The localized string with the specified culture and arguments.</returns>
	string GetString<T>(T keyEnum, params object[] args) where T : Enum;

	/// <summary>
	///  Gets localized string from the resource file using an enum key, culture, arguments and key.
	/// </summary>
	/// <param name="keyEnum">The enum key to look up the localized string.</param>
	/// <param name="cultureInfo">The culture info to use for localization.</param>
	/// <param name="args"> The arguments to format the localized string. Which can be localization keys.</param>
	/// <typeparam name="T">The enum type.</typeparam>
	/// <returns> The localized string with the specified culture and arguments.  </returns>
	string GetString<T>(T keyEnum, CultureInfo cultureInfo, params object[] args) where T : Enum;

	/// <summary>
	///  Gets localized string from the resource file using a key.
	/// </summary>
	/// <param name="key"> The key to look up the localized string.</param>
	/// <returns> The localized string.  </returns>
	string this[string key] { get; }

	/// <summary>
	///  Gets localized string from the resource file using a key and culture.
	/// </summary>
	/// <param name="key"></param>
	/// <param name="cultureInfo">The culture info to use for localization.</param>
	/// <returns> The localized string with the specified culture.  </returns>
	string this[string key, CultureInfo cultureInfo] { get; }
}