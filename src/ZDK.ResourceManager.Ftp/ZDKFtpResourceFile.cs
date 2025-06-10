using ZDK.ResourceManager.Abstractions;
using System.IO;

namespace ZDK.ResourceManager.Ftp;

/// <summary>
/// Represents a resource file located on an FTP server.
/// </summary>
public sealed record ZDKFtpResourceFile : IZDKResourceFile
{
	private readonly Func<Stream> _streamFactory;

	public string FileUri { get; } // e.g., ftp://host/root/path/to/file.txt
	public string FileName { get; } // This should be the relative path from the FtpRootDirectory
	public string FileExtension { get; }
	public string FileNameWithoutExtension { get; }
	public string? DirectoryName { get; set; }

	// Constructor to create an instance with a factory for getting the stream
	public ZDKFtpResourceFile(string ftpFullUri, string relativeFilePath, Func<Stream> streamFactory) {
		if (string.IsNullOrWhiteSpace(ftpFullUri))
			throw new ArgumentException("FTP Full URI cannot be null or whitespace.", nameof(ftpFullUri));
		if (string.IsNullOrWhiteSpace(relativeFilePath))
			throw new ArgumentException("Relative file path cannot be null or whitespace.", nameof(relativeFilePath));
		ArgumentNullException.ThrowIfNull(streamFactory);

		FileUri = ftpFullUri;
		FileName = relativeFilePath.Replace('\\', '/').TrimStart('/'); // Ensure Unix-style path and no leading slash
		FileExtension = Path.GetExtension(relativeFilePath);
		FileNameWithoutExtension = Path.GetFileNameWithoutExtension(relativeFilePath);
		_streamFactory = streamFactory;
		DirectoryName = Path.GetDirectoryName(relativeFilePath)?.Replace('\\', '/'); // Ensure Unix-style path
	}

	/// <summary>
	/// Gets a read-only stream for the resource file's content from the FTP server.
	/// It is the responsibility of the caller to dispose of the stream.
	/// </summary>
	/// <returns>A read-only stream of the resource file's content.</returns>
	/// <exception cref="ResourceFileAccessException">Thrown if an error occurs during FTP download.</exception>
	public Stream GetStream() {
		try {
			return _streamFactory.Invoke();
		}
		catch (Exception ex) {
			throw new ResourceFileAccessException($"Error accessing file stream for '{FileUri}' from FTP.", ex);
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