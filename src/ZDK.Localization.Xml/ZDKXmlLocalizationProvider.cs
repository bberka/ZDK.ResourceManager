using ZDK.Localization.Abstractions;

namespace ZDK.Localization.Xml;

public class ZDKXmlLocalizationProvider : IZDKLocalizationProvider
{

	public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> LoadLocalizationData() {
		throw new NotImplementedException();
	}
}