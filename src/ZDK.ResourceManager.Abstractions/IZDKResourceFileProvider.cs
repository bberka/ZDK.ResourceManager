namespace ZDK.ResourceManager.Abstractions;

/// <summary>
/// Defines an interface for loading resource files from a source.
/// </summary>
public interface IZDKResourceFileProvider : IDisposable
{
	/// <summary>
	/// Loads all resource files from the specified source using the provided configuration.
	/// </summary>
	/// <param name="source">The source string for the provider (e.g., directory path, connection string, bucket name).</param>
	/// <param name="configuration">The resource configuration.</param>
	/// <returns>A collection of resource files.</returns>
	HashSet<IZDKResourceFile> LoadFiles(string source, IZDKResourceConfiguration configuration); // Changed method name to LoadFiles and parameters
}