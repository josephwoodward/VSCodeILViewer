# Visual Studio Code C# IL (Intermediate Language) Viewer

IL (Intermediate Language) Viewer for Visual Studio Code allows you to rapidly inspect the IL output of any given C# file.

**Downloading IL Viewer for Visual Studio Code**  
[Download C# IL Viewer for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=josephwoodward.vscodeilviewer) or install it directly within Visual Studio Code by launching Quick Open (`CMD + P` for Mac or `CTRL + P` for Windows) and pasting in the follow command and press enter.

`ext install vscodeilviewer`

**How does it work?**  
Using the power of Roslyn, IL Viewer compiles your chosen file in memory (along with any dependencies it has) and produces an in memory DLL containing the IL. 

## Features

**Easy IL Inspection:**  
Simply open a .cs file which you'd like to inspect, right-click and select `Inspect IL`.

![Easy IL inspection](./images/demo.gif)

More features coming soon.


## Reporting Issues

As this is an early version there will likely be issues. Please feel free to raise any issues you run into in the [issues section](https://github.com/JosephWoodward/VSCodeILViewer/issues).

## Contributions

I would love contributions on IL Viewer and welcome contributions of all shapes and sizes. If you're interested in contributing then please feel free to let me know if you need any help getting started.
