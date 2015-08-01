# AC_SERVER_APPS

This repository contains some .NET C# Assetto Corsa Server tools.

- AC_SessionReport: A simple project containing a serializable multiplayer session report and also defines an interface for handling the report in custom tools. 
- AC_SessionReportPlugin: This is the server plugin that collects information about the session and creates a SessionReport object. The plugin is derived from Minolins acPlugins4net.AcServerPlugin, currently using the fork https://github.com/flitzi/acplugins. The main advantage over the report we get from kunos in the default json is that we get the full drivers list, not only the last connected drivers (which I think is the case in the default json). It also collects driven distances (odometer).
- AC_ServerStarter: This reads the server.cfg and starts the plugin before starting the acServer.exe. It also can be configured to do a track cycle. Defines three new admin commands (/next_track, /change_track, /send_chat). Contains a SessionReport handler for writing a report.json.
- AC_TrackCycle: (executable) a simple server administration GUI. Shows what's going on, has the option to send messages from the server and to cycle to the next track. In the App.config you can specify which SessionReport handler should be loaded (currently JsonReportWriter and DBFiller, you probably want to remove the DBFiller if you don't have a MSSQL DB)
- AC_TrackCycle_Console: (executable) the console version of AC_TrackCycle.
- AC_DBFillerEF: A SessionReport handler filling a MSSQL database using EntitiyFramework6.
