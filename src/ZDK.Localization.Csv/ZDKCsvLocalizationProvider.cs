using System.Collections.Immutable;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using ZDK.Localization.Abstractions;
using ZDK.ResourceManager.Abstractions;

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

	public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> LoadLocalizationData(IImmutableSet<IZDKResourceFile> resourceFiles) {
		// Use the stored configuration to determine the read method
		return _configuration.ReadMethod switch {
			ZDKCsvLocalizationReadMethod.SingleFileWithAllCultures => LoadFromSingleFile(resourceFiles),
			ZDKCsvLocalizationReadMethod.MultipleFilesEachWithOneCulture => LoadFromMultipleFiles(resourceFiles),
			_ => throw new NotSupportedException($"Unsupported CSV ReadMethod: {_configuration.ReadMethod}"),
		};
	}

	// Private method to load from a single file with all cultures
	private IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> LoadFromSingleFile(IImmutableSet<IZDKResourceFile> resourceFiles) {
		_logger.LogInformation("Attempting to load localization from single CSV file: {Source} with separator '{Separator}'", _configuration.SourcePath, _configuration.Separator);

		var file = resourceFiles.FirstOrDefault(f => f.FileUri.Equals(_configuration.SourcePath, StringComparison.OrdinalIgnoreCase));
		if (file == null) {
			_logger.LogWarning("Localization CSV file not found at source: {Source}", _configuration.SourcePath);
			return new Dictionary<string, IReadOnlyDictionary<string, string>>();
		}

		_logger.LogInformation("Found localization CSV file: {FileName}", file.FileUri);
		var localizationData = ReadCsv(file);
		if (localizationData.Count == 0) {
			_logger.LogWarning("No localization data found in file: {FileName}", file.FileUri);
			return new Dictionary<string, IReadOnlyDictionary<string, string>>();
		}
		
		_logger.LogInformation("Loaded localization data from file: {FileName}", file.FileUri);

		var localizationKeys = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
		foreach (var (cultureCode, data) in localizationData) {
			if (!localizationKeys.TryGetValue(cultureCode, out var value)) {
				value = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
				localizationKeys[cultureCode] = value;
			}

			foreach (var (key, localizedValue) in data) {
				if (!value.TryAdd(key, localizedValue)) {
					_logger.LogWarning("Duplicate key '{Key}' found in culture '{CultureCode}' in file: {FileName}", key, cultureCode, file.FileUri);
				}
			}
		}
		
		_logger.LogInformation("Completed loading localization from single CSV file. Total keys loaded: {TotalKeys}", localizationKeys.Count);
		return localizationKeys.ToDictionary(
		                                     kvp => kvp.Key, IReadOnlyDictionary<string, string> (kvp) => kvp.Value.ToDictionary(
		                                                                                                                         innerKvp => innerKvp.Key,
		                                                                                                                         innerKvp => innerKvp.Value
		                                                                                                                        )
		                                    );
	}

	private IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> LoadFromMultipleFiles(IImmutableSet<IZDKResourceFile> resourceFiles) {
		_logger.LogInformation("Attempting to load localization from multiple CSV files in directory: {Directory} with separator '{Separator}'", _configuration.SourcePath, _configuration.Separator);

		if (!Directory.Exists(_configuration.SourcePath)) {
			_logger.LogWarning("Localization directory not found at source: {Directory}", _configuration.SourcePath);
			return new Dictionary<string, IReadOnlyDictionary<string, string>>();
		}

		var files = resourceFiles.Where(file => file.FileUri.StartsWith(_configuration.SourcePath, StringComparison.OrdinalIgnoreCase))
		                         .ToList();
		if (files.Count == 0) {
			_logger.LogWarning("No localization CSV files found in directory: {Directory}", _configuration.SourcePath);
			return new Dictionary<string, IReadOnlyDictionary<string, string>>();
		}

		_logger.LogInformation("Found {FileCount} localization CSV files in directory: {Directory}", files.Count, _configuration.SourcePath);
		var localizationKeys = new Dictionary<string, Dictionary<string, string>>();
		foreach (var file in files) {
			_logger.LogInformation("Loading localization from file: {FileName}", file.FileUri);
			var localizationData = ReadCsv(file);
			foreach (var (cultureCode, data) in localizationData) {
				if (!localizationKeys.TryGetValue(cultureCode, out var value)) {
					value = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
					localizationKeys[cultureCode] = value;
				}

				foreach (var (key, localizedValue) in data) {
					if (!value.TryAdd(key, localizedValue)) {
						_logger.LogWarning("Duplicate key '{Key}' found in culture '{CultureCode}' in file: {FileName}", key, cultureCode, file.FileUri);
					}
				}
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

	private Dictionary<string, Dictionary<string, string>> ReadCsv(IZDKResourceFile file) {
		var allLocalizationData = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
		var separator = _configuration.Separator;
		var keyColumnName = _configuration.KeyColumnName;

		_logger.LogInformation("Attempting to parse single CSV resource: {ResourceName} with separator '{Separator}'", file.FileUri, separator);

		using var stream = file.GetStream();
		using var reader = new StreamReader(stream, _configuration.Encoding);
		var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture) {
			Delimiter = separator.ToString(),
			HasHeaderRecord = true
		};
		using var csv = new CsvReader(reader, csvConfig);
		csv.ReadHeader();
		var headers = GetValidCultureHeadersDictionary(csv.HeaderRecord, file);
		foreach (var cultureCode in headers.Keys) {
			allLocalizationData.TryAdd(cultureCode, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
		}

		try {
			while (csv.Read()) {
				var key = csv.GetField(keyColumnName)?.Trim();
				if (string.IsNullOrEmpty(key)) {
					_logger.LogWarning("Skipping row with no key in localization CSV: {ResourceName}", file.FileUri);
					continue;
				}

				foreach (var (cultureCode, columnIndex) in headers) {
					var value = csv.GetField<string>(columnIndex)?.Trim() ?? string.Empty;
					if (string.IsNullOrEmpty(value)) {
						continue;
					}

					allLocalizationData[cultureCode][key] = value;
				}
			}

			_logger.LogInformation("Successfully parsed localization data from resource {ResourceName}", file.FileUri);
		}
		catch (CsvHelperException ex) {
			_logger.LogError(ex, "CSV Helper error while loading localization from {ResourceName}",file.FileUri);
			throw;
		}
		catch (Exception ex) {
			_logger.LogError(ex, "Error loading localization from {ResourceName}", file.FileUri);
			throw;
		}

		_logger.LogInformation("Completed parsing CSV resource {ResourceName}. Total languages processed: {LanguageCount}", file.FileUri, allLocalizationData.Count);
		return allLocalizationData;
	}

	private Dictionary<string, int> GetValidCultureHeadersDictionary(string[]? headers, IZDKResourceFile file) {
		headers = headers?.Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
		if (headers is null) {
			_logger.LogError("CSV resource has no header record: {ResourceName}", file.FileUri);
			throw new InvalidOperationException($"CSV resource has no header record: {file.FileUri}");
		}

		var headerCount = headers.Length;
		headers = headers.Distinct().ToArray();
		var distinctHeaderCount = headers.Length;
		if (headerCount != distinctHeaderCount) {
			_logger.LogError("CSV resource has duplicate headers: {ResourceName}. Original count: {HeaderCount}, Distinct count: {DistinctHeaderCount}", file.FileUri, headerCount, distinctHeaderCount);
			throw new InvalidOperationException($"CSV resource has duplicate headers: {file.FileUri}. Original count: {headerCount}, Distinct count: {distinctHeaderCount}");
		}

		headers = headers.Where(h => _configuration.SupportedCultures.Any(c => c.Name.Equals(h, StringComparison.OrdinalIgnoreCase)))
		                 .ToArray();

		var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
		for (var i = 0; i < headers.Length; i++) {
			var header = headers[i];
			if (string.IsNullOrEmpty(header)) continue;

			if (header.Equals(_configuration.KeyColumnName, StringComparison.OrdinalIgnoreCase)) {
				continue;
			}

			try {
				var headerCulture = CultureInfo.GetCultureInfo(header);
				if (!_configuration.SupportedCultures.Any(c => c.Name.Equals(headerCulture.Name, StringComparison.OrdinalIgnoreCase))) {
					continue;
				}

				result.TryAdd(header, i);
			}
			catch (CultureNotFoundException ex) {
				_logger.LogWarning(ex, "Invalid culture code '{Header}' found in CSV header for resource {ResourceName}. Column skipped", header, file.FileUri);
			}

			result[header] = i;
		}

		if (result.Count == 0) {
			_logger.LogError("CSV resource has no header record: {ResourceName}", file.FileUri);
			throw new InvalidOperationException($"CSV resource has no header record: {file.FileUri}");
		}

		return result;
	}
}