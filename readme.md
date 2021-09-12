# TeslaXI
Component-based farming bot for Growtopia.

**Work in progress!**

## Overview
TeslaXI is two things:
 - A farming bot
 - Underlying component-based class library for use in other projects

The goal of the project is to allow developers to create **memory-based** trainers/bots/helpers for Growtopia without having to spend time finding game data and constantly updating the offsets. The latter is achieved with **dynamic offset generation** using RTTI data and other memory behavior patterns.

By the way, the library (`TheLeftExit.Growtopia`) is available as a NuGet package.  
Simply run `Install-Package TheLeftExit.Growtopia` in your package manager to add it as a project reference.

## Credits
 - Huge thanks to the [**TeslaX Discord Server**](https://discord.gg/Px457A4fh4) for its efforts to decode game memory. Feel free to join if you're interested in helping or learning.
 - Thanks to [**0xD3F**](https://github.com/DefaultO) for continued cooperation in reversing the game.
 - RTTI functionality transcribed from [ReClass.NET](https://github.com/ReClassNET/ReClass.NET/blob/0ee8a4cd6a00e2664f2ef3250a81089c32d69392/ReClass.NET/Memory/RemoteProcess.cs#L190).
 - Decoder for items.dat transcribed from [GrowtopiaNoobs/Growtopia_ItemsDecoder](https://github.com/GrowtopiaNoobs/Growtopia_ItemsDecoder).
 - Decoder for RTTEX files transcribed from [Nenkai/RTPackConverter](https://github.com/Nenkai/RTPackConverter).
 
This project is [unlicensed](https://unlicense.org/). Feel free to use any parts of it in your own projects.

## Current progress
**The project is currently being restructured (it's like the fifth time), so the stuff below is no longer relevant!**  

Done:
 - Memory reading/querying API (`TheLeftExit.Growtopia.Native`)
 - Transcription of Growtopia's object model into C# (`TheLeftExit.Growtopia.ObjectModel` namespace - all you'd have to do is create an instance of `Growtopia` and follow IntelliSense)
 - `RTPACK` and `items.dat` decoder API
 - Simple user-friendly bot implementation (`TheLeftExit.TeslaX.Headless`)
 - User-friendly items.dat explorer (property display, filtering)
 
To do:
 - Expand the object model
 - Optimize WorldTileMap to support optionally loading the entire world
 - GUI for the bot