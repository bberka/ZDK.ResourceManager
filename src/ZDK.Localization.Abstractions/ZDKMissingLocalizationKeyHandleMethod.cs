namespace ZDK.Localization.Abstractions;

/// <summary>
/// Specifies the method for handling requests for missing localization keys.
/// </summary>
public enum ZDKMissingLocalizationKeyHandleMethod
{
	/// <summary>
	/// Returns an empty string when a localization key is not found.
	/// </summary>
	ReturnEmptyString,

	/// <summary>
	/// Returns the requested key string when a localization key is not found.
	/// </summary>
	ReturnKey,

	/// <summary>
	/// Throws a <see cref="ZDKMissingLocalizationKeyException"/> when a localization key is not found.
	/// </summary>
	ThrowException // Good to include ThrowException
}