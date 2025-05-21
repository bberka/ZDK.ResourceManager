using System.Collections.Immutable;
using Microsoft.Extensions.Logging;

namespace ZDK.ResourceManager.Abstractions;

/// <summary>
/// Base class for ZDK resource file managers, providing common logic for loading, storing, and accessing resource files.
/// </summary>
public abstract class ZDKResourceManagerBase : IZDKResourceFileManager
{
	protected readonly IZDKResourceFileProvider ResourceProvider; // Dependency on the provider
	protected readonly IZDKResourceConfiguration Configuration; // Dependency on the configuration
	protected readonly IZDKResourceFileWatcher? Watcher; // Optional watcher dependency
	protected readonly ILogger Logger; // Logger dependency

	// Internal storage for resource files
	private IImmutableSet<IZDKResourceFile> _files = ImmutableHashSet<IZDKResourceFile>.Empty;

	// ReaderWriterLockSlim for thread-safe access to the files collection
	private readonly ReaderWriterLockSlim _filesLock = new();

	// Property for thread-safe access to the files collection
	public IImmutableSet<IZDKResourceFile> Files {
		get {
			_filesLock.EnterReadLock();
			try {
				return _files;
			}
			finally {
				_filesLock.ExitReadLock();
			}
		}
		// Protected setter for derived classes to update the data safely
		protected set {
			_filesLock.EnterWriteLock();
			try {
				_files = value ?? throw new ArgumentNullException(nameof(value));
				Logger.LogInformation("Resource file data updated"); // Log the update
			}
			finally {
				_filesLock.ExitWriteLock();
			}
		}
	}


	protected ZDKResourceManagerBase(
		IZDKResourceFileProvider resourceProvider,
		IZDKResourceConfiguration configuration,
		IZDKResourceFileWatcher? watcher,
		ILogger logger) 
	{
		ResourceProvider = resourceProvider ?? throw new ArgumentNullException(nameof(resourceProvider));
		Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		Watcher = watcher; 
		Logger = logger ?? throw new ArgumentNullException(nameof(logger));

		if (Watcher != null) {
			Watcher.ResourceChanged += OnResourceChangedEvent;
			Watcher.ResourceRenamed += OnResourceRenamedEvent;
			Watcher.WatcherError += OnWatcherError;
		}
		else {
			Logger.LogInformation("No resource file watcher provided. Automatic resource file reloading is disabled");
		}
	}
	protected void ReloadResourceFiles(string source) // Needs the source to load from
	{
		Logger.LogInformation("Initiating resource file data reload from source: {Source}", source);
		try {
			var loadedFiles = ResourceProvider.LoadFiles(source, Configuration);
			Files = loadedFiles.ToImmutableHashSet(); // Update the base class's data property
			Logger.LogInformation("Resource file data reloaded successfully. Loaded {FileCount} files", Files.Count);
		}
		catch (Exception ex) {
			Logger.LogError(ex, "Error reloading resource file data from provider for source: {Source}", source);
		}
	}

	protected virtual void OnResourceChangedEvent(object? sender, FileSystemEventArgs e) {
		Logger.LogInformation("Resource source changed: {FullPath}. Initiating reload", e.FullPath);
	}

	protected virtual void OnResourceRenamedEvent(object? sender, RenamedEventArgs e) {
		Logger.LogInformation("Resource source renamed from {OldFullPath} to {FullPath}. Initiating reload", e.OldFullPath, e.FullPath);
	}

	protected virtual void OnWatcherError(object? sender, ErrorEventArgs e) {
		Logger.LogError(e.GetException(), "Resource Watcher error");
	}

	private IZDKResourceFile? HandleMissingFile(string fileName) {
		var handlingMethod = Configuration.MissingResourceFileHandleMethod;

		switch (handlingMethod) {
			case ZDKMissingResourceFileHandleMethod.Ignore:
				Logger.LogWarning("Resource file '{FileName}' not found. Handling method: Ignore", fileName);
				return null;

			case ZDKMissingResourceFileHandleMethod.ThrowException:
				Logger.LogError("Resource file '{FileName}' not found. Handling method: ThrowException", fileName);
				throw new ZDKMissingResourceFileException(fileName); // Use your custom exception

			default:
				Logger.LogError("Unknown MissingResourceFileHandleMethod: {Method}. Falling back to ThrowException", handlingMethod);
				throw new ZDKMissingResourceFileException(fileName);
		}
	}

	public IZDKResourceFile? GetFile(string fileName) {
		var file = Files.FirstOrDefault(f => f.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase));
		if (file == null) {
			return HandleMissingFile(fileName);
		}

		return file;
	}

	public IZDKResourceFile GetFileOrThrow(string fileName) {
		var file = GetFile(fileName);
		if (file == null && Configuration.MissingResourceFileHandleMethod != ZDKMissingResourceFileHandleMethod.Ignore) {
			Logger.LogError("GetFile returned null for '{FileName}' but handling method was not Ignore", fileName);
			throw new ZDKMissingResourceFileException(fileName);
		}

		if (file == null && Configuration.MissingResourceFileHandleMethod == ZDKMissingResourceFileHandleMethod.Ignore) {
			Logger.LogError("GetFileOrThrow called for '{FileName}' but file was not found (handled by Ignore). Throwing exception as per GetFileOrThrow contract", fileName);
			throw new ZDKMissingResourceFileException(fileName);
		}

		if (file != null) {
			return file;
		}

		Logger.LogError("Unexpected scenario in GetFileOrThrow for '{FileName}'. GetFile returned null", fileName);
		throw new ZDKMissingResourceFileException(fileName);
	}

	public IZDKResourceFile? this[string fileName] => GetFile(fileName);

	public IImmutableSet<IZDKResourceFile> GetFiles() {
		return Files;
	}


	public void Dispose() {
		_filesLock.Dispose();
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected abstract void Dispose(bool disposing);
}