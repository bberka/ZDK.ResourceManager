using System.Globalization;

namespace ZDK.Localization.Abstractions;

/// <summary>
///  Defines the contract for a localization provider.
/// </summary>
public interface IZDKLocalizationProvider
{
	/// <summary>
	///  Loads the localization resources from the specified source.
	/// </summary>
	/// <returns></returns>
	IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> LoadLocalizationData();
}