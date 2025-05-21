using ZDK.Localization.Abstractions;

namespace ZDK.Localization.Providers.Json;

public class ZDKJsonLocalizationProvider : IZDKLocalizationProvider
{
	public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> LoadLocalizationData() {
		throw new NotImplementedException();
	}
}