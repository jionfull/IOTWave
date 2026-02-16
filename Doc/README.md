# IOTWave

An IoT framework library for .NET 8.0 and .NET 10.

## Features

- Multi-framework support (.NET 8.0 and .NET 10.0)
- Easy-to-use IoT device management
- Extensible architecture
- Modern C# features

## Installation

Install via NuGet:

```bash
dotnet add package IOTWave
```

## Quick Start

```csharp
using IOTWave;

// Initialize the framework
var iotFramework = new IOTFramework();

// Connect to device
await iotFramework.ConnectDeviceAsync("device-id");

// Send command
await iotFramework.SendCommandAsync("device-id", "your-command");
```

## License

This project is licensed under the LGPL-2.1-or-later License - see the [LICENSE](../LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Contact

For questions and support, please open an issue on GitHub.
