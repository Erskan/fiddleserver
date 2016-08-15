# fiddleserver
A simple server made in C# for my fiddle project.

Functionality
---
Runs an HTTPListener on a given port, listening for upgrade calls to WebSocket.
When found it spawns a WebSocket in a thread and goes right back to listening.
The sockets read and to some extent modify the GameState which is synced with the clients.

Disclaimer
---
This is by no means a best practice approach. I'm just quickly learning about sockets, threading and simple game programming.
