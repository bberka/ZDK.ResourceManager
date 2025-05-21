using Microsoft.Extensions.Logging; // Assuming ILogger is used
using ZDK.ResourceManager.Abstractions;

namespace ZDK.ResourceManager.FileSystem;

/// <summary>
/// Watches the file system for resource file changes.
/// </summary>
public class ZDKFileSystemResourceWatcher : IZDKResourceFileWatcher,
                                            IDisposable
{
	private readonly ILogger<ZDKFileSystemResourceWatcher> _logger;
	private FileSystemWatcher? _watcher;

	public event EventHandler<FileSystemEventArgs>? ResourceChanged;
	public event EventHandler<RenamedEventArgs>? ResourceRenamed;
	public event EventHandler<ErrorEventArgs>? WatcherError;


	public ZDKFileSystemResourceWatcher(ILogger<ZDKFileSystemResourceWatcher> logger) {
		_logger = logger;
	}

	/// <summary>
	/// Starts watching the specified directory path for resource file changes.
	/// </summary>
	/// <param name="source">The directory path to watch.</param>
	public void StartWatching(string source)
	{
		if (_watcher != null) StopWatching(); 

		var directoryPath = source;

		try {
			if (!Directory.Exists(directoryPath)) {
				_logger.LogWarning("Resource directory for watching not found: {DirectoryPath}", directoryPath);
				return;
			}

			_watcher = new FileSystemWatcher(directoryPath) {
				NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size,
				IncludeSubdirectories = true, 
				EnableRaisingEvents = true 
			};

			_watcher.Created += (s, e) => ResourceChanged?.Invoke(this, e);
			_watcher.Deleted += (s, e) => ResourceChanged?.Invoke(this, e);
			_watcher.Changed += (s, e) => ResourceChanged?.Invoke(this, e);
			_watcher.Renamed += (s, e) => ResourceRenamed?.Invoke(this, e);
			_watcher.Error += (s, e) => WatcherError?.Invoke(this, e);

			_logger.LogInformation("Started watching resource directory: {DirectoryPath}", directoryPath);
		}
		catch (Exception ex) {
			_logger.LogError(ex, "Error starting file watcher for directory {DirectoryPath}. Automatic reloading disabled", directoryPath);
			_watcher = null; 
		}
	}

	/// <summary>
	/// Stops watching the resource source.
	/// </summary>
	public void StopWatching() {
		if (_watcher != null) {
			_watcher.EnableRaisingEvents = false;
			_watcher.Dispose();
			_watcher = null;
			_logger.LogInformation("Stopped watching resource directory");
		}
	}

	public void Dispose() {
		StopWatching();
	}
}