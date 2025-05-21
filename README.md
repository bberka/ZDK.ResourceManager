# ZDK Resource & Localization Management Libraries

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Abstractions](https://img.shields.io/nuget/v/ZDK.Localization.Abstractions.svg)](https://www.nuget.org/packages/ZDK.Localization.Abstractions/)

A flexible and extensible set of .NET libraries for managing application resources and localization. This project
provides a core framework with abstractions and allows for different provider implementations (e.g., File System, CSV).

## Overview

The ZDK Resource and Localization libraries are designed to provide a clean, configurable, and testable way to handle
external resources and localized strings in your .NET applications.

**Key Features:**

* **Abstraction-First Design:** Core interfaces define the contracts, allowing for different underlying implementations.
* **Resource Management:** Load and access various types of resource files (e.g., configuration, assets).
* **Localization Management:** Load and access localized strings for multi-language applications.
* **Provider Model:** Easily extend the system with custom providers for different data sources (e.g., File System, FTP,
  AWS S3, Databases).
* **Optional File Watching:** Built-in support for watching file system changes and automatically reloading
  resources/localization data (configurable).
* **Dependency Injection Friendly:** Includes extension methods for easy integration with
  `Microsoft.Extensions.DependencyInjection`.
* **Configurable Behavior:** Options for handling missing keys/files, default cultures, etc.

## Packages

This solution is structured into several NuGet packages:

`ZDK.Localization.Abstractions`

Provides the core interfaces and enums for the localization system. This package is essential for defining the contracts
that other localization providers will implement.

`ZDK.Localization.Csv`

Provides an implementation for loading localization data from CSV files. This package includes DI extension methods for
easy integration into your application.

`ZDK.ResourceManager.Abstractions`

Provides the core interfaces and enums for the resource file management system. This package is essential for defining
the contracts that other resource providers will implement.

`ZDK.ResourceManager.FileSystem`
Provides implementations for loading resource files from the local file system and watching for changes. This package
includes DI extension methods for easy integration into your application.

### Localization

* **`ZDK.Localization.Abstractions`**: Contains the core interfaces and enums for the localization system (
  `IZDKLocalizationManager`, `IZDKLocalizationProvider`, `IZDKLocalizationConfiguration`, etc.).
* **`ZDK.Localization.Csv`**: Provides an implementation for loading localization data from CSV files. Includes DI
  extension methods (`AddZDKCsvLocalization`).
    * Supports single CSV file with cultures as columns.
    * Supports multiple CSV files (one per culture).
    * Configurable separator.

### Resource Management

* **`ZDK.ResourceManager.Abstractions`**: Contains the core interfaces and enums for the resource file management
  system (`IZDKResourceFileManager`, `IZDKResourceFileProvider`, `IZDKResourceFileWatcher`, `IZDKResourceFile`,
  `IZDKResourceConfiguration`, etc.).
* **`ZDK.ResourceManager.FileSystem`**: Provides implementations for loading resource files from the local file system
  and watching for changes. Includes DI extension methods (`AddZDKFileSystemResourceManager`).

## Getting Started

### Prerequisites

* .NET 9 SDK

### Installation

This project still in early development, there is no nuget package released yet. Will start releasing nightly builds
very soon.

### Basic Usage (Localization with CSV)

1. **Create your localization CSV file(s).**

   Example `localization.csv` (SingleFileWithAllCultures):
   ```csv
   key,en-US,fr-FR
   greeting,Hello,Bonjour
   farewell,Goodbye,Au revoir
   ```

2. **Configure services in your `Program.cs` or `Startup.cs`:**

   ```csharp
   using Microsoft.Extensions.DependencyInjection;
   using ZDK.Localization.Abstractions;
   using ZDK.Localization.Csv; // For extension methods and concrete config
   using System.Globalization;

   public class Program
   {
       public static void Main(string[] args)
       {
           var builder = WebApplication.CreateBuilder(args); // Or Host.CreateDefaultBuilder()

           // Add ZDK CSV Localization
           builder.Services.AddZDKCsvLocalization(config =>
           {
               config.CsvFilePath = "path/to/your/localization.csv"; // Or directory for multiple files
               config.DefaultCulture = new CultureInfo("en-US");
               config.SupportedCultures = new CultureInfo[]
               {
                   new CultureInfo("en-US"),
                   new CultureInfo("fr-FR")
               };
               config.MissingLocalizationKeyHandleMethod = ZDKMissingLocalizationKeyHandleMethod.ReturnKey;
               config.ReadMethod = ZDKCsvLocalizationReadMethod.SingleFileWithAllCultures;
               config.Separator = ',';
               config.ReloadOnFileChange = true; // Enable file watching
           });

           // ... other service registrations

           var app = builder.Build();

           // ... app configuration
           app.Run();
       }
   }
   ```

3. **Inject and use `IZDKLocalizationManager`:**

   ```csharp
   using ZDK.Localization.Abstractions;

   public class MyService
   {
       private readonly IZDKLocalizationManager _localizationManager;

       public MyService(IZDKLocalizationManager localizationManager)
       {
           _localizationManager = localizationManager;
       }

       public string GetGreeting()
       {
           return _localizationManager.GetString("greeting");
           // Or using the indexer: _localizationManager["greeting"];
       }

       public string GetFrenchFarewell()
       {
           return _localizationManager.GetString("farewell", new CultureInfo("fr-FR"));
       }
   }
   ```

### Basic Usage (File System Resource Management)

1. **Place your resource files in a directory.**

2. **Configure services in your `Program.cs` or `Startup.cs`:**

   ```csharp
   using Microsoft.Extensions.DependencyInjection;
   using ZDK.ResourceManager.Abstractions;
   using ZDK.ResourceManager.FileSystem; // For extension methods and concrete config

   public class Program
   {
       public static void Main(string[] args)
       {
           var builder = WebApplication.CreateBuilder(args); // Or Host.CreateDefaultBuilder()

           // Add ZDK File System Resource Manager
           builder.Services.AddZDKFileSystemResourceManager(config =>
           {
               config.ResourceDirectoryPath = "path/to/your/resources";
               config.MissingResourceFileHandleMethod = ZDKMissingResourceFileHandleMethod.ThrowException;
               config.ReloadOnFileChange = true; // Enable file watching
           });

           // ... other service registrations

           var app = builder.Build();

           // ... app configuration
           app.Run();
       }
   }
   ```

3. **Inject and use `IZDKResourceFileManager`:**

   ```csharp
   using ZDK.ResourceManager.Abstractions;
   using System.IO;

   public class MyFileService
   {
       private readonly IZDKResourceFileManager _resourceManager;

       public MyFileService(IZDKResourceFileManager resourceManager)
       {
           _resourceManager = resourceManager;
       }

       public string? ReadConfigFile()
       {
           IZDKResourceFile? configFile = _resourceManager.GetFile("config.json");
           if (configFile != null)
           {
               using (var stream = configFile.GetStream())
               using (var reader = new StreamReader(stream))
               {
                   return reader.ReadToEnd();
               }
           }
           return null;
       }
   }
   ```

## Configuration Options

Detailed configuration options are available for each provider. Please refer to the specific configuration classes:

* `ZDKCsvLocalizationConfiguration`
* `ZDKFileSystemResourceConfiguration`

## Extending the System

### Creating Custom Providers

To support other data sources (e.g., databases, cloud storage):

1. Implement the relevant interfaces from `ZDK.Localization.Abstractions` or `ZDK.ResourceManager.Abstractions` (e.g.,
   `IZDKLocalizationProvider`, `IZDKResourceFileProvider`).
2. Create a corresponding configuration class if specific settings are needed.
3. Create DI extension methods for easy registration of your custom provider.

## Contributing

Contributions are welcome! If you'd like to contribute, please follow these steps:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature/your-feature-name`).
3. Make your changes.
4. Commit your changes (`git commit -m 'Add some feature'`).
5. Push to the branch (`git push origin feature/your-feature-name`).
6. Open a pull request.

Please make sure to update tests as appropriate.

## Building the Project

WIP

## Running Tests

WIP

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgements

* [CsvHelper](https://joshclose.github.io/CsvHelper/) - For CSV parsing.
