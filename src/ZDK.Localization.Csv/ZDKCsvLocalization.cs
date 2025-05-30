using Microsoft.Extensions.Logging;
using ZDK.Localization.Abstractions;
using ZDK.ResourceManager.Abstractions;

namespace ZDK.Localization.Csv;

public class ZDKCsvLocalization : ZDKLocalizationBase
{
	private readonly IZDKLocalizationProvider _localizationProvider;
	private readonly IZDKResourceFileManager _resourceFileManager;
	private readonly IZDKResourceFileWatcher? _fileWatcher;
	private readonly ZDKCsvLocalizationConfiguration _csvConfiguration;

	public ZDKCsvLocalization(
		ZDKCsvLocalizationConfiguration localizationConfiguration,
		IZDKLocalizationProvider localizationProvider,
		IZDKResourceFileManager resourceFileManager,
		IZDKResourceFileWatcher? fileWatcher,
		ILogger<ZDKCsvLocalization> logger)
		: base(localizationConfiguration, logger) {
		_csvConfiguration = localizationConfiguration;
		_localizationProvider = localizationProvider;
		_resourceFileManager = resourceFileManager;
		_fileWatcher = fileWatcher;

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
			var newData = _localizationProvider.LoadLocalizationData(_resourceFileManager.GetFiles());
			LocalizationData = newData;
			Logger.LogInformation("Localization data reloaded successfully");
		}
		catch (Exception ex) {
			Logger.LogError(ex, "Error reloading localization data from provider");
		}
	}


	private void SetupCsvWatcher() {
		if (_fileWatcher is null) {
			Logger.LogWarning("File watcher is not available. Automatic reloading of localization CSV is disabled");
			return;
		}
		
		Logger.LogInformation("Setting up file watcher for localization CSV");
		
	}

	private void OnCsvFileChanged(object sender, FileSystemEventArgs e) {
		if (e.ChangeType != WatcherChangeTypes.Changed && e.ChangeType != WatcherChangeTypes.Renamed) {
			Logger.LogInformation("Ignoring non-change event for localization CSV: {ChangeType}", e.ChangeType);
			return;
		}
		
		var isLocalizationFile 

		
		
		Logger.LogInformation("Localization CSV file changed: {FullPath}. Initiating reload in short delay", e.FullPath);
		Thread.Sleep(100); 

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