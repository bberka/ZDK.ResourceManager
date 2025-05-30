# ZDK Resource Management

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Abstractions](https://img.shields.io/nuget/v/ZDK.ResourceManager.Abstractions.svg)](https://www.nuget.org/packages/ZDK.ResourceManager.Abstractions/)

A flexible and extensible set of .NET libraries for managing application resources. This project
provides a core framework with abstractions and allows for different provider implementations (e.g., File System).

## Overview

The ZDK Resource Management libraries are designed to provide a clean, configurable, and testable way to handle
external resources in your .NET applications. **"ZDK" is the prefix for our company's open-source projects, indicating that these libraries are developed and maintained by ZDK Network**
**Key Features:**

*   **Abstraction-First Design:** Core interfaces define the contracts, allowing for different underlying implementations.
*   **Resource Management:** Load and access various types of resource files (e.g., configuration, assets).
*   **Provider Model:** Easily extend the system with custom providers for different data sources (e.g., File System, FTP,
    AWS S3, Databases).
*   **Optional File Watching:** Built-in support for watching file system changes and automatically reloading
    resources (configurable).
*   **Dependency Injection Friendly:** Includes extension methods for easy integration with
    `Microsoft.Extensions.DependencyInjection`.
*   **Configurable Behavior:** Options for handling missing files, etc.

## Packages

This solution is structured into several NuGet packages:

`ZDK.ResourceManager.Abstractions`

Provides the core interfaces and enums for the resource file management system. This package is essential for defining
the contracts that other resource providers will implement.

`ZDK.ResourceManager.FileSystem`
Provides implementations for loading resource files from the local file system and watching for changes. This package
includes DI extension methods for easy integration into your application.

### Resource Management

*   **`ZDK.ResourceManager.Abstractions`**: Contains the core interfaces and enums for the resource file management
    system (`IZDKResourceFileManager`, `IZDKResourceFileProvider`, `IZDKResourceFileWatcher`, `IZDKResourceFile`,
    `IZDKResourceConfiguration`, etc.).
*   **`ZDK.ResourceManager.FileSystem`**: Provides implementations for loading resource files from the local file system
    and watching for changes. Includes DI extension methods (`AddZDKFileSystemResourceManager`).

## Getting Started

### Prerequisites

*   .NET 9 SDK

### Installation

This project is currently in early development. Nightly builds are published as NuGet packages and are available for early testing. Please be aware that these versions are not stable and may introduce breaking changes.

To install a nightly build, you'll need to configure your NuGet sources to include the nightly feed (details will be provided when the feed is established). Once configured, you can install the packages using the .NET CLI:

```bash
  dotnet add package ZDK.ResourceManager.Abstractions --version <nightly-version>
  dotnet add package ZDK.ResourceManager.FileSystem --version <nightly-version>
```

Replace `<nightly-version>` with the specific version number of the nightly build you wish to use. Stable releases will be made available on NuGet.org once the project reaches a mature state.

### Basic Usage (File System Resource Management)

1.  **Place your resource files in a directory.**

2.  **Configure services in your `Program.cs`:**

    ```csharp
    using Microsoft.Extensions.DependencyInjection;
    using ZDK.ResourceManager.Abstractions;
    using ZDK.ResourceManager.FileSystem; // For extension methods and concrete config

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
    ```

3.  **Inject and use `IZDKResourceFileManager`:**

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

*   `ZDKFileSystemResourceConfiguration`

## Extending the System

### Creating Custom Providers

To support other data sources (e.g., databases, cloud storage):

1.  Implement the relevant interfaces from `ZDK.ResourceManager.Abstractions` (e.g.,
    `IZDKResourceFileProvider`).
2.  Create a corresponding configuration class if specific settings are needed.
3.  Create DI extension methods for easy registration of your custom provider.

## Contributing

Contributions are welcome! If you'd like to contribute, please follow these steps:

1.  Fork the repository.
2.  Create a new branch (`git checkout -b feature/your-feature-name`).
3.  Make your changes.
4.  Commit your changes (`git commit -m 'Add some feature'`).
5.  Push to the branch (`git push origin feature/your-feature-name`).
6.  Open a pull request.

Please make sure to update tests as appropriate.

## Building the Project

This project targets .NET 9. To build the solution, ensure you have the .NET 9 SDK installed on your machine.

1.  **Clone the repository:**
```bash
    git clone https://github.com/bberka/ZDK.ResourceManager
    cd ZDK.ResourceManager
```

2.  **Restore dependencies:**
```bash
    dotnet restore
```

3**Build the solution:**
```bash
    dotnet build
```

You can also open the `.sln` file in Visual Studio 2022 (with .NET 9 SDK installed) and build from there.
## Running Tests

Due to early development, tests are not yet implemented. Once the project stabilizes, unit tests will be added to ensure reliability and correctness.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## About ZDK.Localization package
ZDK.Localization project is removed from this repository. It is mosty likely won't be reintroduced in the future. 

You either have to write your own localization provider or use existing ones like `Microsoft.Extensions.Localization` or `Localization.AspNetCore` packages and import files from this project as needed.