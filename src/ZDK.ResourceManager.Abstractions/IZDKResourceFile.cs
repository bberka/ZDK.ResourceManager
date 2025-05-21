namespace ZDK.ResourceManager.Abstractions;

/// <summary>
///  Defines the contract for a resource file.
/// </summary>
public interface IZDKResourceFile
{
	/// <summary>
	/// Gets the  file path of the resource file. The value of this property is depends on the provider (e.g. it can be file system path or an url).
	/// </summary>
	public string FileUri { get; }

	/// <summary>
	///  Gets the file extension of the resource file.
	///  </summary>
	///  <returns>The file extension.</returns>
	public string FileExtension { get; }

	/// <summary>
	///  Gets the file name without extension.
	///  </summary>
	///  <returns>The file name without extension.</returns>
	public string FileNameWithoutExtension { get; }

	/// <summary>
	///  Gets the file name with extension.
	///  </summary>
	///  <returns>The file name with extension.</returns>
	public string FileName { get; }

	/// <summary>
	///  Gets the stream of the resource file.
	///  </summary>
	///  <returns>The stream of the resource file.</returns>
	public Stream GetStream();
}