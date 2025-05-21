namespace ZDK.ResourceManager.Abstractions;

/// <summary>
///  Defines the contract for watching resource files. The watcher detects file updates, deletions, and renames, providing events for responding to these changes.
/// </summary>
public interface IZDKResourceFileWatcher : IDisposable
{
	/// <summary>
	///  Notifies when a file system change occurs.
	/// </summary>
	event EventHandler<FileSystemEventArgs> FileSystemChanged;

	/// <summary>
	///  Notifies when a file system change occurs and the file is renamed.
	/// </summary>
	event EventHandler<RenamedEventArgs> FileSystemRenamed;

	/// <summary>
	///  Notifies when a file system change occurs and the file is deleted.
	/// </summary>
	event EventHandler<ErrorEventArgs> WatcherError;

	/// <summary>
	///  Starts watching the resource files for changes.
	/// </summary>
	void StartWatching();

	/// <summary>
	///  Stops watching the resource files for changes.
	/// </summary>
	void StopWatching();
}