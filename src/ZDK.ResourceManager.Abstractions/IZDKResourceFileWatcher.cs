namespace ZDK.ResourceManager.Abstractions;

/// <summary>
/// Defines the contract for a component that watches for changes in a resource source.
/// </summary>
public interface IZDKResourceFileWatcher : IDisposable
{
	/// <summary>
	/// Event fired when a resource in the watched source is created, deleted, or changed.
	/// </summary>
	event EventHandler<FileSystemEventArgs> ResourceChanged; // Generic event name

	/// <summary>
	/// Event fired when a resource in the watched source is renamed.
	/// </summary>
	event EventHandler<RenamedEventArgs> ResourceRenamed; // Generic event name

	/// <summary>
	/// Event fired when an error occurs during the watching process.
	/// </summary>
	event EventHandler<ErrorEventArgs> WatcherError; // Good to keep

	/// <summary>
	/// Starts watching the specified source for changes.
	/// </summary>
	/// <param name="source">The source string to watch (e.g., directory path, connection string).</param>
	void StartWatching(string source);

	/// <summary>
	/// Stops watching the resource source.
	/// </summary>
	void StopWatching();
}