# IOTWave

English | [中文](README.md)

An IoT framework library for .NET 8.0 and .NET 10, providing high-performance waveform display and data processing capabilities.

## Features

- Multi-framework support (.NET 8.0 and .NET 10.0)
- High-performance waveform display controls based on Avalonia UI
- Support for TimeMarker, TimeRangeMarker, YMarker and other markers
- Customizable Y-axis renderer
- Support for zoom, pan and other interactive operations
- Extensible architecture design

## Installation

Install via NuGet:

```bash
dotnet add package IOTWave
```

## Quick Start

```csharp
using IotWave.Views;
using IotWave.Models;

// Create WaveListPanel
var wavePanel = new WaveListPanel
{
    StartTime = DateTime.Now.AddHours(-1),
    EndTime = DateTime.Now,
    AutoDistributePanelHeight = true
};

// Add curve data
wavePanel.ItemsSource = curveGroups;
```

## Project Structure

```
IOTWave/
├── IOTWave/                 # Core library
│   ├── Views/              # Avalonia UI controls
│   ├── Models/             # Data models
│   └── ...
├── IOTWaveDemo/            # Demo project
├── IOTWave.Tests/          # Unit tests
└── Doc/                    # Documentation
```

## License

This project is licensed under the LGPL-2.1-or-later License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Contact

- Email: jionfull@163.com
- QQ: 2608902246

<div align="center">
  <table>
    <tr>
      <td align="center">
        <img src="Doc/qq.png" alt="QQ" width="150"/>
        <br/>
        <sub>QQ QR Code</sub>
      </td>
     
    </tr>
  </table>
</div>

## Star History

If this project helps you, please give it a ⭐️ Star!
