// For Stream

// For IDisposable?

namespace ZDK.ResourceManager.Abstractions;

/// <summary>
/// Defines the contract for a resource file, abstracting its source location.
/// </summary>
public interface IZDKResourceFile
{
	/// <summary>
	/// Gets the unique identifier or URI of the resource file.
	/// The value of this property depends on the provider (e.g., file system path, URL, cloud storage key).
	/// </summary>
	string FileUri { get; } // Renamed from FilePath for better generality

	/// <summary>
	/// Gets the name of the resource file including its extension.
	/// </summary>
	string FileName { get; } // Good

	/// <summary>
	/// Gets the file extension of the resource file.
	/// </summary>
	string FileExtension { get; } // Good

	/// <summary>
	/// Gets the file name without extension.
	/// </summary>
	string FileNameWithoutExtension { get; } // Good

	/// <summary>
	/// Gets a read-only stream for the resource file's content.
	/// It is the responsibility of the caller to dispose of the stream.
	/// </summary>
	/// <returns>A read-only stream of the resource file's content.</returns>
	/// <exception cref="ResourceFileAccessException">Thrown if there's an error accessing the resource content.</exception> // Custom exception for access issues
	Stream GetStream();


	/// <summary>
	///  Gets the size of the resource file in bytes.
	/// </summary>
	long Size { get; }

	/// <summary>
	///  Gets the last modified date and time of the resource file.
	/// </summary>
	DateTimeOffset LastModified { get; }
	
	/// <summary>
	///  Gets the metadata associated with the resource file.
	/// </summary>
	IReadOnlyDictionary<string, string?> Metadata { get; }
}