namespace ZDK.Localization.Abstractions;

public sealed class ZDKMissingLocalizationKeyException : Exception
{
	public ZDKMissingLocalizationKeyException(string key, string? culture = null)
		: base($"The localization key '{key}' was not found in the culture '{culture}'.") {
		Key = key;
		Culture = culture;
	}

	public string Key { get; }
	public string? Culture { get; }
}