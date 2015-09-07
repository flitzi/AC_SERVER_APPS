AC_TrackCycle
=============

Features:
---------
- track cycles automatically after race end
- track cycle via admin command
- complete cfg template cycle (new since 2.0)
- server message to inform players of incoming track change (new since 2.0)
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


cfg Template Cycle (optional):

- fill the name of the cfg template folder that includes a server_cfg.ini and/or entry_list.ini in the exe.config key template_cycle, separated with ;
e.g.
<add key="template_cycle" value="spa_gt3;nurburgring_street_mix"/>


In the exe.config you can also specify which incidents are broadcasted, 0=off, 1=only car with car, 2=all
<add key="broadcast_incidents" value="2"/>

You can specify the number of result broadcast positions
<add key="broadcast_results" value="10"/>

You can specify whether fastest lap set in session should be broadcasted live, 0=off, 1=on
<add key="broadcast_fastest_lap" value="1"/>

You can specify the welcome message. For multiple lines use the | for line breaks. To disable set to empty string.
<add key="welcome_message" value="Welcome $DriverName$ to $ServerName$!|Please drive respectfully!"/>

- if you uncheck "Change track after race" the track will not be cycled after the race, you can flip this option while the server is running, so if you like to stay on the track for some races just uncheck, if you have enough, check.

- the "Next Track" button switches the track immediately

- you can also use admin commands to change the track while you are in the game, just type in the chat after you authorized yourself as admin with /admin myPassword

/next_track
/change_track spa
/change_track ks_nordschleife,endurance
the track(and config, separated with comma, but no space!) in /change_track needs to be in the TRACKS=... list

- admin command for broadcasting a message from the server
/broadcast blablabla

- admin command for sending a message from the server to a specific player
/send_chat carId blablabla

note: chat started with / is not visible to other players

- the server does NOT change the track if restart_session or next_session is voted after the race, so people can prevent the track change if they make a successful vote. Only after the RACE_OVER_TIME has passed the server will change the track

- the result json are saved in the "sessionresults" folder


Changelog:
----------
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