using Microsoft.Extensions.Logging;
using ZDK.Localization.Abstractions;

namespace ZDK.Localization.Csv;

public class ZDKCsvLocalization : ZDKLocalizationBase
{
	private readonly IZDKLocalizationProvider _localizationProvider;
	private readonly ZDKCsvLocalizationConfiguration _csvConfiguration;
	private FileSystemWatcher? _csvWatcher;

	public ZDKCsvLocalization(
		ZDKCsvLocalizationConfiguration localizationConfiguration,
		IZDKLocalizationProvider localizationProvider,
		ILogger<ZDKCsvLocalization> logger) 
		: base(localizationConfiguration, logger)
	{
		_csvConfiguration = localizationConfiguration; 
		_localizationProvider = localizationProvider;

		ReloadLocalizationData();

		if (_csvConfiguration.ReloadOnFileChange) {
			SetupCsvWatcher();
		}
		else {
			Logger.LogInformation("Reload on file change is disabled for CSV localization");
		}
	}

	// Method to reload localization data using the provider
	private void ReloadLocalizationData() {
		Logger.LogInformation("Initiating localization data reload");
		try {
			var newData = _localizationProvider.LoadLocalizationData();
			LocalizationData = newData;
			Logger.LogInformation("Localization data reloaded successfully");
		}
		catch (Exception ex) {
			Logger.LogError(ex, "Error reloading localization data from provider");
		}
	}


	private void SetupCsvWatcher() {
		var csvFilePath = _csvConfiguration.SourcePath;
		var directoryPath = Path.GetDirectoryName(csvFilePath);
		var fileName = Path.GetFileName(csvFilePath);

		if (string.IsNullOrEmpty(directoryPath) || string.IsNullOrEmpty(fileName)) {
			Logger.LogError("Could not set up file watcher for localization CSV: invalid path or filename '{CsvFilePath}'", csvFilePath);
			return;
		}

		try {
			_csvWatcher = new FileSystemWatcher(directoryPath, fileName) {
				NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
				EnableRaisingEvents = true 
			};

			_csvWatcher.Changed += OnCsvFileChanged;
			_csvWatcher.Renamed += OnCsvFileChanged;
			_csvWatcher.Error += OnWatcherError;

			Logger.LogInformation("Watching for changes to localization CSV: {CsvFilePath}", csvFilePath);
		}
		catch (Exception ex) {
			Logger.LogError(ex, "Error setting up file watcher for localization CSV: {CsvFilePath}. Automatic reloading disabled", csvFilePath);
			_csvWatcher = null;
		}
	}

	private void OnCsvFileChanged(object sender, FileSystemEventArgs e) {
		// Debounce the event - FileSystemWatcher can fire multiple times for a single save
		// A simple debounce with Thread.Sleep is often sufficient for resource files
		// In a more complex system, consider a more robust debouncing mechanism (timer, TPL Dataflow)
		Logger.LogInformation("Localization CSV file changed: {FullPath}. Initiating reload in short delay", e.FullPath);
		Thread.Sleep(100); // Small delay to ensure file is fully written

		ReloadLocalizationData();
	}

	private void OnWatcherError(object sender, ErrorEventArgs e) {
		Logger.LogError(e.GetException(), "Localization FileSystemWatcher error");
	}


	protected override void Dispose(bool disposing) {
		Logger.LogInformation("Disposing ZDKCsvLocalization and its FileSystemWatcher");
		_csvWatcher?.Dispose();
	}
}