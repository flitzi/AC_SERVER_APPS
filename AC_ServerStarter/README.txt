AC_TrackCycle
=============

Features:
---------
- track cycles automatically after race end
- track cycle via admin command
- complete cfg template cycle (new since 2.0)
- server message to inform players of incoming track change (new since 2.0)
- session control tab (new since 2.4)
- connected drivers tab (new since 2.4)
- track graph tab (new since 2.4)
- result logging into json, including all players (not only the last connected players), collision events, laps (new since 2.0) (optional upload to database and safety rating based player access will come soon)
- server chat via server GUI or server command line (new since 2.0)
- server chat via admin command (new since 2.0)
- server private message via admin command (new since 2.2)
- results displayed as server chat messages including best lap and incidents (new since 2.0)
- near real-time incident report as server chat message including involved cars and impact speed (new since 2.0)
- live fastest lap in session broadcast (new since 2.1)
- driver welcome message (new since 2.1)
- complete C# source code on https://github.com/flitzi/AC_SERVER_APPS


Installation:
-------------
- Extract the files into your AC/server directory, next to acServer executable or extract anywhere else and set ac_server_directory in exe.config to folder where acServer executable is located
- If not already installed you need .NET 4.0


Configuration:
--------------
Plugin for result, report and server chat functionality
- put these lines in the server_cfg.ini (you may adjust the ports, the ports are read from the server_cfg.ini)

UDP_PLUGIN_ADDRESS=127.0.0.1:12000
UDP_PLUGIN_LOCAL_PORT=11000


Track Cycle (optional):

- put this section in your server_cfg.ini
[TRACK_CYCLE]
TRACKS=spa,5;ks_nordschleife,endurance,2;mugello,6;nurburgring,6;silverstone,6;imola,6

syntax is like this <trackname>,<lapcount>;<trackname>,<lapcount>,<layout>;<trackname>,<lapcount>


Presets Cycle (optional):

- fill the name of the preset directory that includes a server_cfg.ini and/or entry_list.ini in the exe.config key presets_cycle, separated with ;
e.g.
<add key="presets_cycle" value="spa_gt3;nurburgring_street_mix"/>

the default directory stucture for this then should be:

/acserver.exe
/presets/spa_gt3/server_cfg.ini
/presets/spa_gt3/server_cfg.ini
/presets/nurburgring_street_mix/server_cfg.ini
/presets/nurburgring_street_mix/server_cfg.ini

you can change the presets directory name in the exe.config with the key ac_presets_directory ( note: before 2.7.8 this was always the cfg directory, but since acServerManager.exe creates a templates directory why not use that as default. If you want the 2.7.7 behaviour use <add key="ac_presets_directory" value="cfg"/> )

note: If you have the [TRACK_CYCLE] block in the default cfg/server_cfg.ini then this will take priority over the presets cycle.


In the exe.config you can also specify which incidents are broadcasted, 0=off, 1=only car with car, 2=all
<add key="broadcast_incidents" value="2"/>

You can specify the number of result broadcast positions
<add key="broadcast_results" value="10"/>

You can specify whether fastest lap set in session should be broadcasted live, 0=off, 1=on, 2=every lap
<add key="broadcast_fastest_lap" value="1"/>

You can specify the welcome message. For multiple lines use the | for line breaks. To disable set to empty string.
<add key="welcome_message" value="Welcome $DriverName$ to $ServerName$!|Please drive respectfully!"/>

- if you uncheck "Change track after race" the track will not be cycled after the race, you can flip this option while the server is running, so if you like to stay on the track for some races just uncheck, if you have enough, check.

- the "Next Track" button switches the track immediately

- you can also use admin commands to change the track while you are in the game, just type in the chat after you authorized yourself as admin with /admin myPassword

/next_track
switches to the next track in list

/change_track spa
/change_track ks_nordschleife,endurance    (separated with comma, but no space)
the track in needs to be in the TRACKS=... list
this also works with the id of the track that is shown with /list_tracks, e.g. /change_track 1

/queue_track spa
/queue_track ks_nordschleife,endurance    (separated with comma, but no space)
the track in needs to be in the TRACKS=... list
this also works with the id of the track that is shown with /list_tracks, e.g. /queue_track 1


- everyone can use these commands in the chat

/list_tracks
lists the all the tracks in the cycle

/vote_track spa
/vote_track ks_nordschleife,endurance    (separated with comma, but no space)
the track in needs to be in the TRACKS=... list
this also works with the id of the track that is shown with /list_tracks, e.g. /vote_track 1
you can only vote once per cycle, it doesn't matter when you vote (practice/qualify/race)
the track with the most votes will be the next track, unless the server admin chooses something else
if multiple tracks have the same amount of votes, none of them is used

- admin command for broadcasting a message from the server
/broadcast blablabla

- admin command for sending a message from the server to a specific player
/send_chat carId blablabla


note: 
- chat started with / is not visible to other players

- the result json are saved in the "sessionresults" folder

- for the track graph to work you need the official map.png and map.ini in the official folder structure. If you run acServer from a complete game steam installation, they will be found. If you are having a standalone server installation, you can put the two files per track in the content/tracks subfolders (where the surface.ini files are)

Changelog:
----------

2.8.1 (2021/02/20)
- votes now change track if auto_change_track = 0
- if several tracks get the same number of votes, the track which reached this number fist will be used


2.8.0 (2021/02/15)
- added auto_change_track config option


2.7.9 (2018/01/08)
- added log_to_console, overwrite_log_file and interactive_console keys in AC_TrackCycle_Console.exe.config


2.7.8 (2018/01/06)
- renamed template_cycle to preset_cycle in exe.config
- added ac_presets_directory in exe.config, this is the directory where the presets should be stored ( note: before 2.7.8 this was always the cfg directory, but since acServerManager.exe creates a templates directory why not use that as default. If you want the 2.7.7 behaviour use <add key="ac_presets_directory" value="cfg"/> )
- not showing the complete path of the presets
- fixed GUI not showing the current session status for the very first session


2.7.7 (2017/12/23)
- changed startup order
- using additional kill process when using create_server_window = 1


2.7.6 (2017/12/18)
- (re)starting other processes in background


2.7.5 (2017/12/18)
- added BestLap and LapCount columns to Drivers tab
- added elapsed time textbox


2.7.4 (2017/05/16)
- small changes to make AC_TrackCycle_Console.exe Linux compatible (using mono) (tested with a mono docker)


2.7.3 (2017/03/03)
- trying to write the server log messages in correct order
- allowing /change_track with index
- added /queue_track admin command
- added /list_tracks command
- added /vote_track command
- removed change_track_after_every_loop config option, now always changing track after every loop


2.7.2 (2016/08/01)
- new config options
    <add key="kick_before_change" value="0"/>
    <add key="create_server_window" value="0"/><!-- if AC_TrackCycle crashes because the acServer.exe process can not be killed on track change then set this to 1 -->


2.7.0 (2016/07/16)
- updated to AC v1.7
- fixed issue with broadcast results
- showing timestamp in chat


2.6.1 (2016/07/10)
- option to set broadcast_fastest_lap to 2 to broadcast every completed lap


2.6.0 (2016/07/09)
- option to start additional executables, they are also restarted when the server is restarted due to track change, this helps with plugins that can't handle server restarts/track changes


2.5.1 (2016/01/02)
- blocking "Smoothing" server log messages


2.5.0 (2015/10/22)
- updated to new acServer UDP protocol version (AC v1.3.4)
- added DriverGuid column to Connected Drivers tab
- added ban context menu option


2.4.0 (2015/10/21)
- fixed change_track_after_every_loop doesn't cycle twice if used with race session
- if next cycle has same track, layout and laps then the server is not restarted
- showing number of connected drivers
- added Session Control tab
- added Connected Drivers tab
- added Track Graph tab

2.3.0 (2015/10/10)
- updated to new acServer UDP protocol version (AC v1.3)

2.2.3 (2015/09/07)
- added change_track_after_every_loop config option that enables track change after "server looping" message (useful if not having race sessions)

2.2.2 (2015/09/05)
- fixed bug with broadcast_incidents = 0 not disabling broadcast of car collisions
- fixed session info in GUI not being set
- session report: warp detection improvement

2.2.1 (2015/08/13)
- restructured code
- renamed /send_chat to /broadcast
- renamed /send_pm to /send_chat
- fixed issue with DriverInfo not correctly carried over to next session
- some changes and additions to written session results (e.g. LapLength)

2.2.0 (2015/08/11)
- updated to new acServer UDP protocol version (AC v1.2.3)
- no longer needed to provide password for each admin command
- added "/send_pm carId msg" admin command

2.1.1 (2015/08/09)
- fixed problem with results broadcast sometimes screwed up (duplicate lines)
- truncating long driver and car names in results broadcast
- disabled BroadCastIncidents by default
- fixed problem with startpositions in session report

2.1.0 (2015/08/09)
- changed to .NET 4.0 (Client Profile is sufficient) instead of .NET 4.5.2
- updated to new AcServerPluginManager, allowing multiple internal and external plugins to be configured in exe.config
- added live fastest lap in session broadcast
- added driver welcome message
- added StartPosition and TopSpeed to SessionReport
- improved computation of race session position results

2.0.0 (2015/08/05)
- new AC 1.2 server plugin functionality

1.3.0 (2015/05/10)
- added console version
- flushing log file immediately

1.2.0 (2015/03/12)
- now possible to use different TRACK_CONFIG

1.1.0 (2015/03/08)
- option to create logs
- writing log after each track change and flushing log after 50000 lines to prevent high memory usage

1.0.0
- initial release