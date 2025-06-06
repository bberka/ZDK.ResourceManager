using Microsoft.Extensions.Logging;
using ZDK.ResourceManager.Abstractions;

namespace ZDK.ResourceManager.FileSystem;

/// <summary>
/// Provides resource files from the file system.
/// </summary>
public class ZDKFileSystemResourceProvider : IZDKResourceFileProvider
{
	private readonly ILogger<ZDKFileSystemResourceProvider> _logger;

	// This provider doesn't need to store the source or configuration as
	// LoadFiles takes them as parameters.

	public ZDKFileSystemResourceProvider(ILogger<ZDKFileSystemResourceProvider> logger) {
		_logger = logger;
	}

	/// <summary>
	/// Loads resource files from the specified directory path using the provided configuration.
	/// </summary>
	/// <param name="source">The root directory path to load files from.</param>
	/// <param name="configuration">The resource configuration (expected to be ZDKFileSystemResourceConfiguration).</param>
	/// <returns>A collection of resource files.</returns>
	public HashSet<IZDKResourceFile> LoadFiles(string source, IZDKResourceConfiguration configuration) {
		var resourceFiles = new HashSet<IZDKResourceFile>();
		var directoryPath = source; // The source is the directory path

		_logger.LogInformation("Attempting to load resource files from directory: {DirectoryPath}", directoryPath);

		if (!Directory.Exists(directoryPath)) {
			_logger.LogWarning("Resource directory not found for loading: {DirectoryPath}", directoryPath);
			return resourceFiles;
		}

		try {
			var allFiles = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories)
			                        .ToList();

			if (allFiles.Count <= 0) {
				_logger.LogWarning("No files found in resource path: {DirectoryPath}", directoryPath);
				return resourceFiles;
			}

			_logger.LogInformation("Found {FileCount} files in resource path", allFiles.Count);

			foreach (var file in allFiles) {
				var fileInfo = new FileInfo(file);
				if (fileInfo.Length != 0)
					resourceFiles.Add(new ZDKFileSystemResourceFile(file,source));
			}
		}
		catch (Exception ex) {
			_logger.LogError(ex, "Error loading resource files from {DirectoryPath}: {ErrorMessage}", directoryPath, ex.Message);
		}

		return resourceFiles;
	}
}