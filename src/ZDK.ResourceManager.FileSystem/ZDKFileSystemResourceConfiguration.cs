using ZDK.ResourceManager.Abstractions;

namespace ZDK.ResourceManager.FileSystem;

/// <summary>
/// Represents configuration settings specific to the file system resource provider.
/// </summary>
public class ZDKFileSystemResourceConfiguration : IZDKResourceConfiguration
{
	/// <summary>
	/// Gets or sets the root directory path for resource files.
	/// </summary>
	public required string ResourceDirectoryPath { get; init; }

	/// <summary>
	/// Gets or sets the method for handling requests for missing resource files. Default is ThrowException.
	/// </summary>
	public ZDKMissingResourceFileHandleMethod MissingResourceFileHandleMethod { get; init; } = ZDKMissingResourceFileHandleMethod.ThrowException;

	/// <summary>
	/// Gets or sets a value indicating whether to reload resource files automatically when changes are detected in the file system. Default is true.
	/// </summary>
	public bool ReloadOnFileChange { get; init; } = true;
}