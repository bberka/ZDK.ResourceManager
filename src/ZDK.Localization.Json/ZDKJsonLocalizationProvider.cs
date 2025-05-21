using ZDK.Localization.Abstractions;

namespace ZDK.Localization.Providers.Json;

public class ZDKJsonLocalizationProvider : IZDKLocalizationProvider
{
	public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> GetLocalizationData() {
		throw new NotImplementedException();
	}
}