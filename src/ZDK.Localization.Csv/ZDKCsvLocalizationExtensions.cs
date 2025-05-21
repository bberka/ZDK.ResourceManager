using Microsoft.Extensions.DependencyInjection;
using ZDK.Localization.Abstractions;

namespace ZDK.Localization.Csv;

public static class ZDKCsvLocalizationExtensions
{
	/// <summary>
	/// Adds the ZDK CSV Localization components to the IServiceCollection.
	/// Registers ZDKCsvLocalizationConfiguration, ZDKCsvLocalizationProvider, and ZDKCsvLocalizationManager.
	/// </summary>
	/// <param name="services">The IServiceCollection to add the services to.</param>
	/// <param name="configuration">An action to configure the ZDKCsvLocalizationConfiguration.</param>
	/// <returns>The IServiceCollection for chaining.</returns>
	public static IServiceCollection AddZDKCsvLocalization(
		this IServiceCollection services,
		ZDKCsvLocalizationConfiguration configuration) {
		if (configuration == null) throw new ArgumentNullException(nameof(configuration));

		services.AddSingleton<IZDKLocalizationConfiguration>(configuration);
		services.AddSingleton<IZDKLocalizationProvider, ZDKCsvLocalizationProvider>();
		services.AddSingleton<IZDKLocalization, ZDKCsvLocalization>();
		return services;
	}


	/// <summary>
	/// Adds the ZDK CSV Localization components to the IServiceCollection.
	/// Registers ZDKCsvLocalizationConfiguration, ZDKCsvLocalizationProvider, and ZDKCsvLocalizationManager.
	/// </summary>
	/// <param name="services">The IServiceCollection to add the services to.</param>
	/// <param name="key"> The key to register the localization provider and manager.</param>
	/// <param name="configuration">An action to configure the ZDKCsvLocalizationConfiguration.</param>
	/// <returns>The IServiceCollection for chaining.</returns>
	public static IServiceCollection AddZDKCsvLocalizationAsKeyed(
		this IServiceCollection services,
		object key,
		ZDKCsvLocalizationConfiguration configuration) {
		if (key == null) throw new ArgumentNullException(nameof(key));
		if (configuration == null) throw new ArgumentNullException(nameof(configuration));

		services.AddKeyedSingleton<IZDKLocalizationConfiguration>(configuration);
		services.AddKeyedSingleton<IZDKLocalizationProvider, ZDKCsvLocalizationProvider>(key);
		services.AddKeyedSingleton<IZDKLocalization, ZDKCsvLocalization>(key);
		return services;
	}
}