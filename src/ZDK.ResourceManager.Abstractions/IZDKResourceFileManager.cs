using System.Collections.Immutable; // Added for GetFiles return type

namespace ZDK.ResourceManager.Abstractions;

/// <summary>
/// Defines an interface for managing and accessing resource files.
/// </summary>
public interface IZDKResourceFileManager // Consider IDisposable if the manager itself needs disposal
{
	/// <summary>
	/// Gets a snapshot of all resource files available through this manager.
	/// </summary>
	/// <returns>An immutable collection of resource files.</returns>
	IImmutableSet<IZDKResourceFile> GetFiles(); // Added GetFiles back here

	/// <summary>
	/// Gets the resource file with the specified name.
	/// </summary>
	/// <param name="fileName">The name of the file to look up (case-insensitive recommended).</param>
	/// <returns>The resource file if found; otherwise, null.</returns>
	IZDKResourceFile? GetFile(string fileName);

	/// <summary>
	/// Gets the resource file with the specified name or throws an exception if not found.
	/// </summary>
	/// <param name="fileName">The name of the file to look up (case-insensitive recommended).</param>
	/// <returns>The resource file.</returns>
	/// <exception cref="ZDKMissingResourceFileException">Thrown when the file with the specified name is not found by the manager.</exception> // Custom exception
	IZDKResourceFile GetFileOrThrow(string fileName);

	// Indexers for convenience
	/// <summary>
	/// Gets the resource file with the specified name.
	/// </summary>
	/// <param name="fileName">The name of the file to look up (case-insensitive recommended).</param>
	/// <returns>The resource file if found; otherwise, null.</returns>
	IZDKResourceFile? this[string fileName] { get; }
}