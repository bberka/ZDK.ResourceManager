using Microsoft.Extensions.Logging;
using ZDK.ResourceManager.Abstractions;

namespace ZDK.ResourceManager.FileSystem;

public class ZDKFileSystemResourceManager : ZDKResourceManagerBase
{
	private readonly string _resourceSource;

	public ZDKFileSystemResourceManager(
		ZDKFileSystemResourceConfiguration configuration,
		IZDKResourceFileProvider resourceProvider,
		IZDKResourceFileWatcher? watcher,
		ILogger<ZDKFileSystemResourceManager> logger)
		: base(resourceProvider, configuration, watcher, logger) {
		_resourceSource = configuration.ResourceDirectoryPath;

		ReloadResourceFiles(_resourceSource);
	}

	protected override void OnResourceChangedEvent(object? sender, FileSystemEventArgs e) {
		base.OnResourceChangedEvent(sender, e);

		ReloadResourceFiles(_resourceSource);
	}

	protected override void OnResourceRenamedEvent(object? sender, RenamedEventArgs e) {
		base.OnResourceRenamedEvent(sender, e);
		ReloadResourceFiles(_resourceSource);
	}

	protected override void Dispose(bool disposing) { }
}