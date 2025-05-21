using Microsoft.Extensions.Logging;
using ZDK.Localization.Abstractions;

namespace ZDK.Localization.Csv;

public class ZDKCsvLocalization : ZDKLocalizationBase
{
	public ZDKCsvLocalization(ZDKCsvLocalizationConfiguration localizationConfiguration,
	                          IZDKLocalizationProvider localizationProvider,
	                          ILogger<ZDKCsvLocalization> logger) : base(localizationConfiguration,
	                                                                     logger) {
		LocalizationData = localizationProvider.LoadLocalizationData();
	}
}