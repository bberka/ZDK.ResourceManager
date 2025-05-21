using System.Collections.Immutable;

namespace ZDK.ResourceManager.Abstractions;

/// <summary>
/// Defines an interface for providing resource files and monitoring changes to those resources.
/// </summary>
public interface IZDKResourceFileProvider : IDisposable
{
	/// <summary>
	///  Gets the resource files
	/// </summary>
	/// <returns> A collection of resource files.</returns>
	IImmutableSet<IZDKResourceFile> GetFiles();
}