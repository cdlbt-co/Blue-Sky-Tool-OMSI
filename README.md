# Blue Sky Tool

Blue Sky Tool is developed as an advanced version of OMSI Map Tools. Blue Sky allows users to scan for missing objects, splines, AI Vehicles, parked cars, humans and drivers.

## How to scan for missing files

In the main screen, click on the `Open Map...` button. In the file selection screen, navigate to the mapfolder of choice and select the global.cfg file.

Blue Sky Tool will now start scanning for files required by the selected map. Please wait patiently.

After scanning, all of the essential information will show up in the main screen. You will be able to see the map name, description, preview picture, number of tiles, objects, splines, AI vehicles and humans.

**Having missing tiles missing usually means the map maker has disabled some tiles when releasing the map, which is almost never the reason why a map appears empty.**

If there are missing objects, splines, AI vehicles or humans missing, you can go to the corresponding tabs to see the list of missing files.

## How to use the logfile viewer

To use the logfile viewer, first click on the `Logfile` tab.

Then, click on the `Open log file...` button. In the file selection screen, navigate to your OMSI directory and select `logfile.txt`. If you have made a copy of your logfile and renamed the file or placed elsewhere, click on the dropdown box `OMSI Log File (logfile.txt)`, and select `All Files (*.*)`, then select your log file.

The tool will then scan the logfile and divide the entries to three sections - information, warnings, and errors.

## About this project

This program is written in C# with WPF using Visual Studio 2019. 

## License
[AGPL 3.0](https://choosealicense.com/licenses/agpl-3.0/ "AGPL 3.0 License")
