# Blue Sky Tool

Blue Sky Tool is developed as an advanced version of OMSI Map Tools. Blue Sky allows users to scan for missing objects, splines, AI Vehicles, parked cars, humans and drivers.

## Download

Download the lastest release [here](https://github.com/cdlbt-co/Blue-Sky-Tool-OMSI/releases/latest) on GitHub

### System requirements

- Windows 10 1809 or later
- .NET Desktop Runtime 10 (get it [here](https://builds.dotnet.microsoft.com/dotnet/WindowsDesktop/10.0.0/windowsdesktop-runtime-10.0.0-win-x64.exe))

## How to scan for missing files

In the main screen, click on the `Open Map...` button. In the file selection screen, navigate to the map folder of choice and select the global.cfg file.

Blue Sky Tool will now start scanning for files required by the selected map.

After scanning, all of the essential information will show up in the main screen. You will be able to see the map name, description, preview picture, number of tiles, objects, splines, AI vehicles and humans.

**Having missing tiles missing usually means the map maker has disabled some tiles when releasing the map, which is almost never the reason why a map appears empty.**

If there are missing objects, splines, AI vehicles or humans missing, you can go to the corresponding tabs to see the list of missing files.

## How to use the logfile viewer

To use the logfile viewer, first click on the `Logfile` tab.

Then, click on the `Open log file...` button. In the file selection screen, navigate to your OMSI directory and select `logfile.txt`.

The tool will then scan the logfile and divide the entries to three sections - information, warnings, and errors.

## About this project

This program is written in C# with WPF with the .NET 10 using Visual Studio 2026. 

## License
[AGPL 3.0](https://choosealicense.com/licenses/agpl-3.0/ "AGPL 3.0 License")
