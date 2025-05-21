using ZDK.Localization.Abstractions;

namespace ZDK.Localization.Excel;

public class ZDKExcelLocalizationProvider : IZDKLocalizationProvider
{
	public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> LoadLocalizationData() {
		throw new NotImplementedException();
	}
}