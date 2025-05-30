using ZDK.ResourceManager.Abstractions;
using Microsoft.Extensions.Logging;
using FluentFTP;
using System.Net;
using FluentFTP.Exceptions;


namespace ZDK.ResourceManager.Ftp;

/// <summary>
/// Provides resource files from an FTP server using FluentFTP library.
/// </summary>
public class ZDKFtpResourceProvider : IZDKResourceFileProvider
{
	private readonly ILogger<ZDKFtpResourceProvider> _logger;

	protected string TempDirectory => Path.GetTempPath();

	public ZDKFtpResourceProvider(ILogger<ZDKFtpResourceProvider> logger) {
		_logger = logger;
	}

	/// <summary>
	/// Loads resource files from the specified FTP directory using the provided configuration.
	/// </summary>
	/// <param name="source">The root directory path on the FTP server to load files from.</param>
	/// <param name="configuration">The resource configuration (expected to be ZDKFtpResourceConfiguration).</param>
	/// <returns>A collection of resource files.</returns>
	public HashSet<IZDKResourceFile> LoadFiles(string source, IZDKResourceConfiguration configuration) {
		if (configuration is not ZDKFtpResourceConfiguration ftpConfig) {
			throw new ArgumentException("Configuration must be of type ZDKFtpResourceConfiguration for FTP provider.", nameof(configuration));
		}

		var resourceFiles = new HashSet<IZDKResourceFile>();
		var ftpRootPath = source.TrimEnd('/');
		if (string.IsNullOrEmpty(ftpRootPath)) ftpRootPath = "/";

		_logger.LogInformation("Attempting to load resource files from FTP directory: {FtpRootPath} on {FtpHost}:{FtpPort}",
		                       ftpRootPath, ftpConfig.FtpHost, ftpConfig.FtpPort);

		try {
			using (var ftpClient = CreateFtpClient(ftpConfig)) {
				ftpClient.Connect();

				if (!ftpClient.IsConnected) {
					_logger.LogError("Failed to connect to FTP server: {Host}", ftpConfig.FtpHost);
					return resourceFiles;
				}

				_logger.LogDebug("Successfully connected to FTP server");

				var ftpItems = ftpClient.GetListing(ftpRootPath, FtpListOption.Recursive | FtpListOption.AllFiles);

				_logger.LogInformation("Found {FileCount} items on FTP server matching listing options", ftpItems.Length);

				foreach (var ftpListItem in ftpItems) {
					if (ftpListItem.Type != FtpObjectType.File) continue;
					var fullFtpPath = ftpListItem.FullName;
					string relativeFilePath;

					if (fullFtpPath.StartsWith(ftpRootPath, StringComparison.OrdinalIgnoreCase)) {
						relativeFilePath = Path.GetRelativePath(ftpRootPath, fullFtpPath).Replace('\\', '/');
					}
					else {
						_logger.LogWarning("File '{FullFtpPath}' does not seem to be relative to configured root '{RootPath}'. Using full path as relative", fullFtpPath, ftpRootPath);
						relativeFilePath = fullFtpPath.TrimStart('/'); // Remove leading slash if it exists
					}

					var ftpFileUri = $"{fullFtpPath}"; //ftp{(ftpConfig.UseSsl ? "s" : ")}://{ftpConfig.FtpHost}:{ftpConfig.FtpPort}

					resourceFiles.Add(new ZDKFtpResourceFile(ftpFileUri, relativeFilePath, StreamFactory));
					continue;

					Stream StreamFactory() {
						_logger.LogDebug("Attempting to download file {FtpPath} from FTP server", fullFtpPath);
						using var downloadClient = CreateFtpClient(ftpConfig);
						downloadClient.Connect();

						if (!downloadClient.IsConnected) {
							throw new ResourceFileAccessException($"Could not connect to FTP to download '{fullFtpPath}'.");
						}

						var memoryStream = new MemoryStream();
						if (downloadClient.DownloadStream(memoryStream, fullFtpPath)) {
							memoryStream.Position = 0;
							_logger.LogDebug("Successfully downloaded file {FtpPath}", fullFtpPath);
							return memoryStream;
						}

						memoryStream.Dispose();
						throw new ResourceFileAccessException($"Failed to download file '{fullFtpPath}' from FTP.");
					}
				}
			}

			_logger.LogInformation("Successfully loaded {FileCount} resource files from FTP", resourceFiles.Count);
		}
		catch (FtpSecurityNotAvailableException ex) {
			_logger.LogError(ex, "FTP security (SSL/TLS) not available or failed for {Host}:{Port}. Check UseSsl configuration", ftpConfig.FtpHost, ftpConfig.FtpPort);
			throw new ResourceFileAccessException($"FTP security error connecting to {ftpConfig.FtpHost}:{ftpConfig.FtpPort}.", ex);
		}
		catch (FtpAuthenticationException ex) {
			_logger.LogError(ex, "FTP authentication failed for user '{Username}' on {Host}:{Port}. Check credentials", ftpConfig.FtpUsername, ftpConfig.FtpHost, ftpConfig.FtpPort);
			throw new ResourceFileAccessException($"FTP authentication failed for {ftpConfig.FtpUsername} on {ftpConfig.FtpHost}:{ftpConfig.FtpPort}.", ex);
		}
		catch (TimeoutException ex) {
			_logger.LogError(ex, "FTP connection timed out to {Host}:{Port}. Check host, port, and network connectivity", ftpConfig.FtpHost, ftpConfig.FtpPort);
			throw new ResourceFileAccessException($"FTP connection timed out to {ftpConfig.FtpHost}:{ftpConfig.FtpPort}.", ex);
		}
		catch (IOException ex) {
			_logger.LogError(ex, "FTP I/O error during operation to {Host}:{Port}{Path}: {ErrorMessage}",
			                 ftpConfig.FtpHost, ftpConfig.FtpPort, ftpRootPath, ex.Message);
			throw new ResourceFileAccessException($"FTP I/O error during operation to {ftpConfig.FtpHost}:{ftpConfig.FtpPort}.", ex);
		}
		catch (Exception ex) {
			_logger.LogError(ex, "Unexpected error loading resource files from FTP {FtpHost}:{FtpPort}{FtpRootPath}: {ErrorMessage}",
			                 ftpConfig.FtpHost, ftpConfig.FtpPort, ftpRootPath, ex.Message);
			throw new ResourceFileAccessException($"An unexpected error occurred during FTP resource loading from {ftpConfig.FtpHost}:{ftpConfig.FtpPort}.", ex);
		}

		return resourceFiles;
	}

	/// <summary>
	/// Creates and configures an FtpClient instance based on the provided configuration.
	/// </summary>
	private static FtpClient CreateFtpClient(ZDKFtpResourceConfiguration config) {
		var client = new FtpClient(config.FtpHost, new NetworkCredential(config.FtpUsername, config.FtpPassword), config.FtpPort, config.FtpConfig);

		if (config.SkipCertificateValidation) {
			client.ValidateCertificate += (_, e) => { e.Accept = true; };
		}

		return client;
	}
}