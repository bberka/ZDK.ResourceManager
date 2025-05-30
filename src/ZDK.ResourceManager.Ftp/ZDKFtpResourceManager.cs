using Microsoft.Extensions.Logging;
using ZDK.ResourceManager.Abstractions;

namespace ZDK.ResourceManager.Ftp;

public class ZDKFtpResourceManager : ZDKResourceManagerBase
{
	public ZDKFtpResourceManager(
		ZDKFtpResourceConfiguration configuration,
		IZDKResourceFileProvider resourceProvider,
		ILogger<ZDKFtpResourceManager> logger)
		: base(resourceProvider, configuration, null, logger) 
	{
		var ftpResourceSource = configuration.FtpRootDirectory;

		ReloadResourceFiles(ftpResourceSource);
	}

	protected override void Dispose(bool disposing) { }
}