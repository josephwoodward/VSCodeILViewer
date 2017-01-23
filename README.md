# IL Viewer for Visual Studio Code

IL (Intermediate Language) Viewer for Visual Studio Code allows you to rapidly inspect the IL output of any given C# file.

**How does it work?**  
Using the power of Roslyn, IL Viewer compiles your chosen file in memory (along with any dependencies it has) and produces an in memory DLL containing the IL. 

## Features

**Easy IL Inspection:**

Simply open a .cs file which you'd like to inspect, right-click and select `Inspect IL`.

![Easy IL inspection](./images/demo.gif)

More features coming soon.


## Requirements

Currently only works on .NET Core apps using project.json. Support for .csproj is coming very soon.

## Reporting Issues

As this is an early version there will likely be issues. Please feel free to raise any issues you run into in the [issues section](https://github.com/JosephWoodward/VSCodeILViewer/issues).

## Contributions

Contributions of all shapes and sizes are welcome and very much encouraged.