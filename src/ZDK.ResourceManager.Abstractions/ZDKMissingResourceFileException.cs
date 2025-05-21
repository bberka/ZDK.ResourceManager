namespace ZDK.ResourceManager.Abstractions;

public sealed class ZDKMissingResourceFileException : Exception
{
	public ZDKMissingResourceFileException(string fileName)
		: base($"The resource file '{fileName}' was not found.") {
		FileName = fileName;
	}

	public string FileName { get; }
}