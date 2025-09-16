# TileKit.Unity (adapter)

This folder shows how you'd hook TileKit.Core into a Unity project.

- `TileMapView.cs` renders a TileMap snapshot onto a Unity tilemap or sprites.
- `TileInputToCommands.cs` shows converting clicks to your own game orders.

Keep all simulation logic in a headless assembly. Unity draws and gathers input.