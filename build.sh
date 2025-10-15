#!/bin/sh

dotnet publish -c release -r osx-x64 --self-contained false /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:IncludeAllContentForSelfExtract=true
cp bin/release/net9.0/osx-x64/publish/htmnix ~/bin
