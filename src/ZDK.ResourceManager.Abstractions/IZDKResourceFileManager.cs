using System.Collections.Immutable;

namespace ZDK.ResourceManager.Abstractions;

public interface IZDKResourceFileManager
{
	/// <summary>
	///  Gets the resource files with specified file name.
	/// </summary>
	/// <param name="fileName"> The file name to look up.</param>
	/// <returns> A collection of resource files.</returns>
	IZDKResourceFile? GetFile(string fileName);

	/// <summary>
	///  Gets the resource file with specified file name or throws an exception if not found.
	/// </summary>
	/// <param name="fileName"> The file name to look up.</param>
	/// <returns> The resource file.</returns>
	IZDKResourceFile GetFileOrThrow(string fileName);
}