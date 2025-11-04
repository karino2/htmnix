#!/bin/sh

# dotnet publish -c release -r osx-x64 --self-contained false /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:IncludeAllContentForSelfExtract=true
dotnet publish -c release -r osx-arm64 --self-contained false /p:PublishSingleFile=true /p:IncludeAllContentForSelfExtract=true
cp bin/release/net9.0/osx-arm64/publish/htmnix ~/bin
