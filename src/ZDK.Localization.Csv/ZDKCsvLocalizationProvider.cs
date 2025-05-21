using System.Globalization;
using CsvHelper;
using Microsoft.Extensions.Logging;
using ZDK.Localization.Abstractions;

namespace ZDK.Localization.Csv;

public class ZDKCsvLocalizationProvider : IZDKLocalizationProvider
{
	private readonly ILogger<ZDKCsvLocalizationProvider> _logger;
	private readonly IZDKLocalizationConfiguration _configuration;

	public ZDKCsvLocalizationProvider(ILogger<ZDKCsvLocalizationProvider> logger,
	                                  IZDKLocalizationConfiguration configuration) {
		_logger = logger;
		_configuration = configuration;
	}

	public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> LoadLocalizationData() {
		if (_configuration is not ZDKCsvLocalizationConfiguration csvConfiguration)
			throw new ArgumentException("Invalid configuration type", nameof(_configuration));


		var csvFilePath = csvConfiguration.CsvFilePath;

		_logger.LogInformation("Attempting to load localization CSV from source: {Source}", csvFilePath);

		if (!File.Exists(csvFilePath)) {
			_logger.LogWarning("Localization CSV file not found at source: {Source}", csvFilePath);
			return new Dictionary<string, IReadOnlyDictionary<string, string>>();
		}

		var localizationKeys = new Dictionary<string, Dictionary<string, string>>();
		try {
			using var fileStream = new FileStream(csvFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			using var reader = new StreamReader(fileStream);
			using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

			csv.Read();
			csv.ReadHeader();
			var headers = csv.HeaderRecord;

			var languageIndices = new Dictionary<string, int>();
			if (headers != null) {
				for (var i = 0; i < headers.Length; i++) {
					var header = headers[i]?.Trim();
					if (string.IsNullOrEmpty(header)) continue;
					if (header.Equals("key", StringComparison.OrdinalIgnoreCase)) continue;

					languageIndices[header] = i;
					localizationKeys[header] = new Dictionary<string, string>();
				}
			}
			else {
				_logger.LogWarning("Localization CSV file has no header record: {Source}", csvFilePath);
				return new Dictionary<string, IReadOnlyDictionary<string, string>>();
			}


			while (csv.Read()) {
				var key = csv.GetField("key")?.Trim();
				if (string.IsNullOrEmpty(key)) {
					_logger.LogWarning("Skipping row with no key in localization CSV: {Source}", csvFilePath);
					continue;
				}

				foreach (var (languageCode, columnIndex) in languageIndices) {
					var value = csv.GetField<string>(columnIndex)?.Trim() ?? string.Empty;
					localizationKeys[languageCode][key] = value;
				}
			}

			_logger.LogInformation("Successfully loaded localization CSV from {Source}. Loaded {LanguageCount} languages", csvFilePath, localizationKeys.Count);
		}
		catch (CsvHelperException ex) {
			_logger.LogError(ex, "CSV Helper error while loading localization CSV from {Source}: {ErrorMessage}", csvFilePath, ex.Message);
			return new Dictionary<string, IReadOnlyDictionary<string, string>>();
		}
		catch (Exception ex) {
			_logger.LogError(ex, "Error loading localization CSV from {Source}: {ErrorMessage}", csvFilePath, ex.Message);
			return new Dictionary<string, IReadOnlyDictionary<string, string>>();
		}

		return localizationKeys.ToDictionary(
		                                     kvp => kvp.Key, IReadOnlyDictionary<string, string> (kvp) => kvp.Value.ToDictionary(
		                                                                                                                         innerKvp => innerKvp.Key,
		                                                                                                                         innerKvp => innerKvp.Value
		                                                                                                                        )
		                                    );
	}
}