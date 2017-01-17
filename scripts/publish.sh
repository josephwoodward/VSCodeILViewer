#!/bin/bash

dotnet publish ../src/IlViewer.WebApi -c release --output "../src/vscodeilviewer/server/"
dotnet publish ../src/IlViewer -c debug --output "../src/vscodeilviewer/server/"