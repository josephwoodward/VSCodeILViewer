# IL Viewer for Visual Studio Code

IL (Intermediate Language) Viewer for Visual Studio Code allows you to rapidly inspect the IL output of any given C# file.

**How does it work?**  
Using the power of Roslyn, IL Viewer compiles your chosen file in memory (along with any dependencies it has) and produces an in memory DLL containing the IL. 

## Features

**Easy IL Inspection:**  
Simply open a file you'd like to inspect the IL for, right-click and select `View IL`.

More features coming soon.


## Requirements

Currently only works on .NET Core apps using project.json. Support for .csproj is coming very soon.

## Known Issues

None as of yet.

## Release Notes

### 0.0.1

Initial release of IL Viewer. Please report any issues you find to [https://github.com/JosephWoodward/VSCodeILViewer/](https://github.com/JosephWoodward/VSCodeILViewer/)

-----------------------------------------------------------------------------------------------------------