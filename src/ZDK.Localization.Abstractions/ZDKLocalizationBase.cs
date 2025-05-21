using System.Globalization;
using Microsoft.Extensions.Logging;

namespace ZDK.Localization.Abstractions;
// Assuming ZDKLocalizationBase is in the Core package

/// <summary>
/// Base class for ZDK localization managers, providing common logic.
/// </summary>
public abstract class ZDKLocalizationBase : IZDKLocalization
{
	protected ILogger<ZDKLocalizationBase> Logger { get; }
	protected readonly IZDKLocalizationConfiguration Configuration;

	private IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> _localizationData = new Dictionary<string, IReadOnlyDictionary<string, string>>();
	private readonly ReaderWriterLockSlim _dataLock = new();

	public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> LocalizationData {
		get {
			_dataLock.EnterReadLock();
			try {
				return _localizationData;
			}
			finally {
				_dataLock.ExitReadLock();
			}
		}
		protected set {
			_dataLock.EnterWriteLock();
			try {
				_localizationData = value ?? throw new ArgumentNullException(nameof(value));
				Logger.LogInformation("Localization data updated"); // Log the update
			}
			finally {
				_dataLock.ExitWriteLock();
			}
		}
	}


	public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> GetLocalizationData() {
		return LocalizationData;
	}

	protected ZDKLocalizationBase(IZDKLocalizationConfiguration localizationConfiguration,
	                              ILogger<ZDKLocalizationBase> logger) {
		Logger = logger;
		Configuration = localizationConfiguration ?? throw new ArgumentNullException(nameof(localizationConfiguration));
	}

	private string HandleMissingKey(string key, CultureInfo cultureInfo) {
		var handlingMethod = Configuration.MissingLocalizationKeyHandleMethod;

		switch (handlingMethod) {
			case ZDKMissingLocalizationKeyHandleMethod.ReturnEmptyString:
				Logger.LogWarning("Localization key '{Key}' not found for culture '{CultureName}'. Handling method: ReturnEmptyString", key, cultureInfo.Name);
				return string.Empty;

			case ZDKMissingLocalizationKeyHandleMethod.ReturnKey:
				Logger.LogWarning("Localization key '{Key}' not found for culture '{CultureName}'. Handling method: ReturnKey", key, cultureInfo.Name);
				return key;

			case ZDKMissingLocalizationKeyHandleMethod.ThrowException:
				Logger.LogError("Localization key '{Key}' not found for culture '{CultureName}'. Handling method: ThrowException", key, cultureInfo.Name);
				throw new ZDKMissingLocalizationKeyException(key, cultureInfo.Name);

			default:
				Logger.LogError("Unknown MissingLocalizationKeyHandleMethod: {Method}. Falling back to ReturnKey", handlingMethod);
				return key;
		}
	}

	private string? GetRawStringWithFallback(string key, CultureInfo cultureInfo) {
		_dataLock.EnterReadLock();
		try {
			if (_localizationData.TryGetValue(key, out var localizedStrings)) {
				if (localizedStrings.TryGetValue(cultureInfo.Name, out var localizedString)) return localizedString;

				// Try neutral culture if available and different from specific
				if (!string.IsNullOrEmpty(cultureInfo.Parent.Name) &&
				    !cultureInfo.Parent.Equals(CultureInfo.InvariantCulture) && // Avoid fallback to invariant culture if not explicitly defined
				    localizedStrings.TryGetValue(cultureInfo.Parent.Name, out localizedString))
					return localizedString;

				if (localizedStrings.TryGetValue(Configuration.DefaultCulture.Name, out localizedString))
					return localizedString;

				Logger.LogWarning("Localization key '{Key}' found, but no translation available for culture '{CultureName}', parent culture '{ParentCultureName}', or default culture '{DefaultCultureName}'",
				                  key, cultureInfo.Name, cultureInfo.Parent.Name, Configuration.DefaultCulture?.Name ?? "None");
				return null;
			}
		}
		finally {
			_dataLock.ExitReadLock();
		}

		Logger.LogWarning("Localization key '{Key}' not found in localization data", key);
		return null;
	}


	public string GetString(string key) {
		var culture = CultureInfo.CurrentUICulture;
		return GetString(key, culture);
	}

	public string GetString(string key, CultureInfo cultureInfo) {
		var localizedString = GetRawStringWithFallback(key, cultureInfo);

		if (localizedString == null) return HandleMissingKey(key, cultureInfo);

		return localizedString;
	}

	protected object[] ResolveLocalizationArgs(object[] args, CultureInfo? cultureInfo = null) {
		if (args.Length == 0) return args;

		var culture = cultureInfo ?? CultureInfo.CurrentUICulture; //TODO: Make sure this is the correct culture

		var resolvedArgs = new object[args.Length];
		for (var i = 0; i < args.Length; i++)
			if (args[i] is string argString) {
				if (argString.StartsWith("$L.")) {
					var argKey = argString[3..];
					resolvedArgs[i] = GetString(argKey, culture);
				}
				else {
					resolvedArgs[i] = args[i];
				}
			}
			else {
				resolvedArgs[i] = args[i];
			}

		return resolvedArgs;
	}


	public string GetString(string key, params object[] args) {
		var localizedString = GetString(key);
		var resolvedArgs = ResolveLocalizationArgs(args);
		try {
			return string.Format(localizedString, resolvedArgs);
		}
		catch (FormatException ex) {
			Logger.LogError(ex, "Error formatting localized string for key '{Key}' with culture '{CultureName}'. Format string: '{FormatString}'. Arguments: {Args}",
			                key, CultureInfo.CurrentUICulture.Name, localizedString, resolvedArgs);
			return localizedString;
		}
	}

	public string GetString(string key, CultureInfo cultureInfo, params object[] args) {
		var localizedString = GetString(key, cultureInfo);
		var resolvedArgs = ResolveLocalizationArgs(args);
		try {
			return string.Format(localizedString, resolvedArgs);
		}
		catch (FormatException ex) {
			Logger.LogError(ex, "Error formatting localized string for key '{Key}' with culture '{CultureName}'. Format string: '{FormatString}'. Arguments: {Args}",
			                key, cultureInfo.Name, localizedString, resolvedArgs);
			return localizedString;
		}
	}

	public string GetString<T>(T keyEnum) where T : Enum {
		var key = keyEnum.ToString();
		return GetString(key);
	}

	public string GetString<T>(T keyEnum, CultureInfo cultureInfo) where T : Enum {
		var key = keyEnum.ToString();
		return GetString(key, cultureInfo);
	}

	public string GetString<T>(T keyEnum, params object[] args) where T : Enum {
		var key = keyEnum.ToString();
		return GetString(key, args);
	}

	public string GetString<T>(T keyEnum, CultureInfo cultureInfo, params object[] args) where T : Enum {
		var key = keyEnum.ToString();
		return GetString(key, cultureInfo, args);
	}

	public string this[string key] => GetString(key);

	public string this[string key, CultureInfo cultureInfo] => GetString(key, cultureInfo);

	public void Dispose() {
		_dataLock.Dispose();
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected abstract void Dispose(bool disposing);
}