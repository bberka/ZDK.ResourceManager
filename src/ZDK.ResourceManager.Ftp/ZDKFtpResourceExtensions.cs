using Microsoft.Extensions.DependencyInjection;
using ZDK.ResourceManager.Abstractions;

namespace ZDK.ResourceManager.Ftp;

public static class ZDKFtpResourceExtensions
{
	/// <summary>
	/// Adds the ZDK FTP Resource Manager components to the IServiceCollection.
	/// Registers ZDKFtpResourceConfiguration, ZDKFtpResourceProvider, and ZDKFtpResourceManager.
	/// Note: FTP resource management does not support file watching in the same way as file system.
	/// </summary>
	/// <param name="services">The IServiceCollection to add the services to.</param>
	/// <param name="configuration">The ZDKFtpResourceConfiguration instance to use.</param>
	/// <returns>The IServiceCollection for chaining.</returns>
	public static IServiceCollection AddZDKFtpResourceManager(
		this IServiceCollection services,
		ZDKFtpResourceConfiguration configuration) {
		ArgumentNullException.ThrowIfNull(configuration);

		services.AddSingleton(configuration);
		services.AddSingleton<IZDKResourceConfiguration>(configuration);

		services.AddSingleton<IZDKResourceFileProvider, ZDKFtpResourceProvider>();

		services.AddSingleton<IZDKResourceFileManager, ZDKFtpResourceManager>();
		return services;
	}

	/// <summary>
	/// Adds the ZDK FTP Resource Manager components as keyed services to the IServiceCollection.
	/// Registers ZDKFtpResourceConfiguration, ZDKFtpResourceProvider, and ZDKFtpResourceManager with a specified key.
	/// This allows for multiple resource managers to be registered and resolved by key.
	/// Note: FTP resource management does not support file watching in the same way as file system.
	/// </summary>
	/// <param name="services">The IServiceCollection to add the services to.</param>
	/// <param name="key">The key to use for registering the resource manager components.</param>
	/// <param name="configuration">The ZDKFtpResourceConfiguration instance to use.</param>
	/// <returns>The IServiceCollection for chaining.</returns>
	public static IServiceCollection AddZDKFtpResourceManagerAsKeyed(
		this IServiceCollection services,
		object key,
		ZDKFtpResourceConfiguration configuration) {
		ArgumentNullException.ThrowIfNull(key);
		ArgumentNullException.ThrowIfNull(configuration);

		services.AddKeyedSingleton(key, configuration);
		services.AddKeyedSingleton<IZDKResourceConfiguration>(key, (_, _) => configuration);

		services.AddKeyedSingleton<IZDKResourceFileProvider, ZDKFtpResourceProvider>(key);

		services.AddKeyedSingleton<IZDKResourceFileManager, ZDKFtpResourceManager>(key);
		return services;
	}
}