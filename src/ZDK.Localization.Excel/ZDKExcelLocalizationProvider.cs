using ZDK.Localization.Abstractions;

namespace ZDK.Localization.Excel;

public class ZDKExcelLocalizationProvider : IZDKLocalizationProvider
{
	public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> GetLocalizationData() {
		throw new NotImplementedException();
	}
}