using ZDK.ResourceManager.Abstractions;

namespace ZDK.ResourceManager.FileSystem;

/// <summary>
/// Represents a resource file located on the file system.
/// </summary>
public sealed record ZDKFileSystemResourceFile : IZDKResourceFile
{
	public string FileUri { get; }
	public string FileName { get; }
	public string FileExtension { get; }
	public string FileNameWithoutExtension { get; }

	public ZDKFileSystemResourceFile(string filePath, string? basePath = null) {
		if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("File path cannot be null or whitespace.", nameof(filePath));

		if (!File.Exists(filePath)) {
			throw new FileNotFoundException($"The file '{filePath}' does not exist.", filePath);
		}

		FileUri = filePath;
		FileName = string.IsNullOrEmpty(basePath) 
			? Path.GetFileName(filePath) 
			: Path.GetRelativePath(basePath, filePath);
		FileExtension = Path.GetExtension(filePath);
		FileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
	}

	/// <summary>
	/// Gets a read-only stream for the resource file's content from the file system.
	/// It is the responsibility of the caller to dispose of the stream.
	/// </summary>
	/// <returns>A read-only stream of the resource file's content.</returns>
	/// <exception cref="FileNotFoundException">Thrown if the file does not exist at the time of access.</exception>
	/// <exception cref="IOException">Thrown if an I/O error occurs during file access.</exception>
	/// <exception cref="UnauthorizedAccessException">Thrown if the caller does not have the required permission.</exception>
	public Stream GetStream() {
		try {
			return new FileStream(FileUri, FileMode.Open, FileAccess.Read, FileShare.Read);
		}
		catch (FileNotFoundException) {
			throw;
		}
		catch (IOException ex) {
			throw new ResourceFileAccessException($"Error accessing file stream for '{FileUri}'.", ex);
		}
		catch (UnauthorizedAccessException ex) {
			throw new ResourceFileAccessException($"Unauthorized access to file stream for '{FileUri}'.", ex);
		}
		catch (Exception ex) {
			throw new ResourceFileAccessException($"An unexpected error occurred while accessing file stream for '{FileUri}'.", ex);
		}
	}

	public string GetContentAsString() {
		using var stream = GetStream();
		using var reader = new StreamReader(stream);
		return reader.ReadToEnd();
	}
	public string[] GetContentAsLines() {
		using var stream = GetStream();
		using var reader = new StreamReader(stream);
		var lines = new List<string>();
		while (!reader.EndOfStream) {
			var line = reader.ReadLine()?.Trim();
			if (!string.IsNullOrEmpty(line)) {
				lines.Add(line);
			}
		}
		return lines.ToArray();
	}
}