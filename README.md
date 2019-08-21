# StreamNotifier
-----------------------------

This program can help you to know if your favorite broadcaster is online.

## Features

* Periodically or manually checks if a stream is online using its API.

* Display a Steam-like notification 

* Open the streaming url directly in your favorite web browser or launch livestreamer to watch it (livestreamer must be installed)

* Plugin support : write your own to handle another streaming API !


## Suported services

* Twitch

* Youtube

* Hitbox

* Dailymotion

* Chaturbate 

* More to come....

## TODO (order not relevant)

- Create a GUI for the config

- Better quality support (can be tricky)

- Add more streaming services

- Better cache management

- Move the configuration file to the user directory. I like my application "portables" but this is necessary for the users.

- Find/make an icon ?

- Write extended documentation on how to write a plugin


## About & Technical details

I wrote this program because its useful for me, and because i wanted to try some .Net features.

* C#

* Plugins support use the Reflection API & interface.
Since for streaming websites "Channels", "Streams" and "Users" are similar but somewhat DIFFERENT concepts the plugin interface facilitate
the coding. The main program is manipulating "LiveStreams" and doesnt have to worry about the inner working of each streaming API.

* HTTPS, JSON

* Regex
