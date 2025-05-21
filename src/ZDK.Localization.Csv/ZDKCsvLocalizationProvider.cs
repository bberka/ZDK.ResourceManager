using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using ZDK.Localization.Abstractions;

namespace ZDK.Localization.Csv;

public class ZDKCsvLocalizationProvider : IZDKLocalizationProvider
{
	private readonly ILogger<ZDKCsvLocalizationProvider> _logger;

	private readonly ZDKCsvLocalizationConfiguration _configuration;

	public ZDKCsvLocalizationProvider(
		ILogger<ZDKCsvLocalizationProvider> logger,
		ZDKCsvLocalizationConfiguration configuration) {
		_logger = logger;
		_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
	}

	public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> LoadLocalizationData() {
		// Use the stored configuration to determine the read method
		return _configuration.ReadMethod switch {
			ZDKCsvLocalizationReadMethod.SingleFileWithAllCultures => LoadFromSingleFile(),
			ZDKCsvLocalizationReadMethod.MultipleFilesEachWithOneCulture => LoadFromMultipleFiles(),
			_ => throw new NotSupportedException($"Unsupported CSV ReadMethod: {_configuration.ReadMethod}"),
		};
	}

	// Private method to load from a single file with all cultures
	private IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> LoadFromSingleFile() {
		var localizationKeys = new Dictionary<string, Dictionary<string, string>>();
		var csvFilePath = _configuration.SourcePath; // This is the file path
		var separator = _configuration.Separator;

		_logger.LogInformation("Attempting to load localization from single CSV file: {Source} with separator '{Separator}'", csvFilePath, separator);

		if (!File.Exists(csvFilePath)) {
			_logger.LogWarning("Localization CSV file not found at source: {Source}", csvFilePath);
			return new Dictionary<string, IReadOnlyDictionary<string, string>>();
		}

		try {
			using var fileStream = new FileStream(csvFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			using var reader = new StreamReader(fileStream);

			var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture) {
				Delimiter = separator.ToString(),
				HasHeaderRecord = true
			};
			using var csv = new CsvReader(reader, csvConfig);

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

			_logger.LogInformation("Successfully loaded localization from single CSV file: {Source}. Loaded {LanguageCount} languages", csvFilePath, localizationKeys.Count);
		}
		catch (CsvHelperException ex) {
			_logger.LogError(ex, "CSV Helper error while loading localization from single CSV file {Source}: {ErrorMessage}", csvFilePath, ex.Message);
			return new Dictionary<string, IReadOnlyDictionary<string, string>>();
		}
		catch (Exception ex) {
			_logger.LogError(ex, "Error loading localization from single CSV file {Source}: {ErrorMessage}", csvFilePath, ex.Message);
			return new Dictionary<string, IReadOnlyDictionary<string, string>>();
		}

		return localizationKeys.ToDictionary(
		                                     kvp => kvp.Key, kvp => (IReadOnlyDictionary<string, string>)kvp.Value.ToDictionary(
		                                                                                                                        innerKvp => innerKvp.Key,
		                                                                                                                        innerKvp => innerKvp.Value
		                                                                                                                       )
		                                    );
	}

	// Private method to load from multiple files, each with one culture
	private IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> LoadFromMultipleFiles() {
		var localizationKeys = new Dictionary<string, Dictionary<string, string>>();
		var baseDirectory = _configuration.SourcePath;
		var separator = _configuration.Separator;

		_logger.LogInformation("Attempting to load localization from multiple CSV files in directory: {Directory} with separator '{Separator}'", baseDirectory, separator);

		if (!Directory.Exists(baseDirectory)) {
			_logger.LogWarning("Localization directory not found at source: {Directory}", baseDirectory);
			return new Dictionary<string, IReadOnlyDictionary<string, string>>();
		}

		foreach (var culture in _configuration.SupportedCultures) {
			var cultureFileName = $"{culture.Name}.csv"; // File name convention
			var csvFilePath = Path.Combine(baseDirectory, cultureFileName);

			if (!File.Exists(csvFilePath)) {
				_logger.LogWarning("Localization CSV file not found for culture {CultureName} at source: {Source}", culture.Name, csvFilePath);
				continue;
			}

			try {
				using var fileStream = new FileStream(csvFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
				using var reader = new StreamReader(fileStream);

				var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture) {
					Delimiter = separator.ToString(),
					HasHeaderRecord = true
				};
				using var csv = new CsvReader(reader, csvConfig);


				csv.Read();
				csv.ReadHeader();
				var headers = csv.HeaderRecord;

				if (headers == null || !headers.Any(h => h.Equals("key", StringComparison.OrdinalIgnoreCase))) {
					_logger.LogWarning("Localization CSV file for culture {CultureName} is missing 'key' header: {Source}", culture.Name, csvFilePath);
					continue;
				}

				var valueColumnIndex = -1;
				if (headers.Length > 1) {
					valueColumnIndex = 1;
				}
				else {
					_logger.LogWarning("Localization CSV file for culture {CultureName} has only 'key' column, no value column found: {Source}", culture.Name, csvFilePath);
					continue;
				}


				while (csv.Read()) {
					var key = csv.GetField("key")?.Trim();
					if (string.IsNullOrEmpty(key)) {
						_logger.LogWarning("Skipping row with no key in localization CSV for culture {CultureName}: {Source}", culture.Name, csvFilePath);
						continue;
					}

					var value = csv.GetField<string>(valueColumnIndex)?.Trim() ?? string.Empty;

					if (!localizationKeys.ContainsKey(key)) {
						localizationKeys[key] = new Dictionary<string, string>();
					}

					localizationKeys[key][culture.Name] = value;
				}

				_logger.LogInformation("Successfully loaded localization for culture {CultureName} from {Source}", culture.Name, csvFilePath);
			}
			catch (CsvHelperException ex) {
				_logger.LogError(ex, "CSV Helper error while loading localization for culture {CultureName} from {Source}: {ErrorMessage}", culture.Name, csvFilePath, ex.Message);
				continue;
			}
			catch (Exception ex) {
				_logger.LogError(ex, "Error loading localization for culture {CultureName} from {Source}: {ErrorMessage}", culture.Name, csvFilePath, ex.Message);
				continue;
			}
		}

		_logger.LogInformation("Completed loading localization from multiple CSV files. Total keys loaded: {TotalKeys}", localizationKeys.Count);

		return localizationKeys.ToDictionary(
		                                     kvp => kvp.Key, IReadOnlyDictionary<string, string> (kvp) => kvp.Value.ToDictionary(
		                                                                                                                         innerKvp => innerKvp.Key,
		                                                                                                                         innerKvp => innerKvp.Value
		                                                                                                                        )
		                                    );
	}
}