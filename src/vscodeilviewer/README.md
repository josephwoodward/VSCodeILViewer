# IL Viewer for Visual Studio Code

IL (Intermediate Language) Viewer for Visual Studio Code allows you to rapidly inspect the IL output of any given C# file.

**How does it work?**  
Using the power of Roslyn, IL Viewer compiles your chosen file in memory (along with any dependencies it has) and produces an in memory DLL containing the IL. 

## Features

**Easy IL Inspection:**

Simply open a .cs file which you'd like to inspect, right-click and select `Inspect IL`.

![Easy IL inspection](https://raw.githubusercontent.com/JosephWoodward/VSCodeILViewer/master/images/demo.gif)

More features coming soon.


## Requirements

Currently only works on .NET Core apps using project.json. Support for .csproj is coming very soon.

## Known Issues

- In order to refresh IL output you need to close the split pane and request IL again.
- Delay in first de-compilation start up, this will be improved soon.
- Project.json support only (.csproj coming soon)

## Release Notes

### 0.0.1

Initial release of IL Viewer. Please report any issues you find to [https://github.com/JosephWoodward/VSCodeILViewer/](https://github.com/JosephWoodward/VSCodeILViewer/)