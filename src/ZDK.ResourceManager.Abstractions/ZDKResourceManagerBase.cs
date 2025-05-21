using System.Collections.Immutable;

namespace ZDK.ResourceManager.Abstractions;

public abstract class ZDKResourceManagerBase : IZDKResourceFileManager
{
	public IImmutableSet<IZDKResourceFile> Files { get; protected set; } = ImmutableHashSet<IZDKResourceFile>.Empty;

	public IZDKResourceFile? GetFile(string fileName) {
		return Files.FirstOrDefault(f => f.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase));
	}

	public IZDKResourceFile GetFileOrThrow(string fileName) {
		return GetFile(fileName) ?? throw new FileNotFoundException($"Resource file '{fileName}' not found.");
	}
}