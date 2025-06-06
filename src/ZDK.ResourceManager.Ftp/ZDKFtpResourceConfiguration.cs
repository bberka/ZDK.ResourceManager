using System.Net;
using FluentFTP;
using ZDK.ResourceManager.Abstractions;

namespace ZDK.ResourceManager.Ftp;

/// <summary>
/// Represents configuration settings specific to the FTP resource provider.
/// </summary>
public class ZDKFtpResourceConfiguration : IZDKResourceConfiguration
{
	/// <summary>
	/// Gets or sets the FTP host address (e.g., "ftp.example.com").
	/// </summary>
	public required string FtpHost { get; init; }

	/// <summary>
	/// Gets or sets the FTP port. Default is 21.
	/// </summary>
	public int FtpPort { get; init; } = 21;

	/// <summary>
	/// Gets or sets the FTP username.
	/// </summary>
	public required string FtpUsername { get; init; }

	/// <summary>
	/// Gets or sets the FTP password.
	/// </summary>
	public required string FtpPassword { get; init; }

	/// <summary>
	/// Gets or sets the root directory path on the FTP server to manage resources from.
	/// This should be a path relative to the FTP user's home directory or an absolute path on the server.
	/// Default is "/".
	/// </summary>
	public string FtpRootDirectory { get; init; } = "/";

	/// <summary>
	/// Gets or sets the method for handling requests for missing resource files. Default is ThrowException.
	/// </summary>
	public ZDKMissingResourceFileHandleMethod MissingResourceFileHandleMethod { get; init; } = ZDKMissingResourceFileHandleMethod.ThrowException;

	/// <summary>
	/// Gets or sets a value indicating whether to skip server certificate validation during FTP connection. This is useful for development environments with self-signed certificates.
	/// </summary>
	public bool SkipCertificateValidation { get; init; } = false;

	/// <summary>
	///  Gets or sets the FTP configuration settings used by the FluentFTP library.
	/// </summary>
	public FtpConfig? FtpConfig { get; init; }
}