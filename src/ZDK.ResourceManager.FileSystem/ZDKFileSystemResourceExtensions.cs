using Microsoft.Extensions.DependencyInjection;
using ZDK.ResourceManager.Abstractions;

namespace ZDK.ResourceManager.FileSystem;

public static class ZDKFileSystemResourceExtensions
{
	/// <summary>
	/// Adds the ZDK File System Resource Manager components to the IServiceCollection.
	/// Registers ZDKFileSystemResourceConfiguration, ZDKFileSystemResourceProvider, ZDKFileSystemResourceWatcher (optionally), and ZDKFileSystemResourceManager.
	/// </summary>
	/// <param name="services">The IServiceCollection to add the services to.</param>
	/// <param name="configuration">The ZDKFileSystemResourceConfiguration instance to use.</param>
	/// <returns>The IServiceCollection for chaining.</returns>
	public static IServiceCollection AddZDKFileSystemResourceManager(
		this IServiceCollection services,
		ZDKFileSystemResourceConfiguration configuration) {
		if (configuration == null) {
			throw new ArgumentNullException(nameof(configuration));
		}

		services.AddSingleton(configuration);
		services.AddSingleton<IZDKResourceFileProvider, ZDKFileSystemResourceProvider>();
		if (configuration.ReloadOnFileChange) {
			services.AddSingleton<IZDKResourceFileWatcher, ZDKFileSystemResourceWatcher>();
		}
		services.AddSingleton<IZDKResourceFileManager, ZDKFileSystemResourceManager>(); // Use IZDKResourceFileManager here
		return services;
	}

	/// <summary>
	/// Adds the ZDK File System Resource Manager components as keyed services to the IServiceCollection.
	/// Registers ZDKFileSystemResourceConfiguration, ZDKFileSystemResourceProvider, ZDKFileSystemResourceWatcher (optionally), and ZDKFileSystemResourceManager with a specified key.
	/// This allows for multiple resource managers to be registered and resolved by key.
	/// </summary>
	/// <param name="services">The IServiceCollection to add the services to.</param>
	/// <param name="key">The key to use for registering the resource manager components.</param>
	/// <param name="configuration">The ZDKFileSystemResourceConfiguration instance to use.</param>
	/// <returns>The IServiceCollection for chaining.</returns>
	public static IServiceCollection AddZDKFileSystemResourceManagerAsKeyed(
		this IServiceCollection services,
		object key,
		ZDKFileSystemResourceConfiguration configuration) {
		if (key == null) {
			throw new ArgumentNullException(nameof(key));
		}

		if (configuration == null) {
			throw new ArgumentNullException(nameof(configuration));
		}

		services.AddKeyedSingleton(typeof(ZDKFileSystemResourceConfiguration), key, configuration);
		services.AddSingleton(configuration); 

		services.AddKeyedSingleton<IZDKResourceFileProvider, ZDKFileSystemResourceProvider>(key);

		if (configuration.ReloadOnFileChange) {
			services.AddKeyedSingleton<IZDKResourceFileWatcher, ZDKFileSystemResourceWatcher>(key);
		}

		services.AddKeyedSingleton<IZDKResourceFileManager, ZDKFileSystemResourceManager>(key);
		return services;
	}
}