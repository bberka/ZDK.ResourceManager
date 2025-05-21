namespace ZDK.ResourceManager.Abstractions;

/// <summary>
/// Specifies the method for handling requests for missing resource files.
/// </summary>
public enum ZDKMissingResourceFileHandleMethod
{
	/// <summary>
	/// Throws a <see cref="ZDKMissingResourceFileException"/> when a resource file is not found.
	/// </summary>
	ThrowException,

	/// <summary>
	/// Ignores the request and returns null when a resource file is not found.
	/// </summary>
	Ignore,
}