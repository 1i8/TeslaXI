# TeslaXI
Component-based farming bot for Growtopia.

**Work in progress!**

## Overview
TeslaXI is two things:
 - Simple (but limited) farming bot
 - Underlying component-based class library for use in other projects

The goal of the project is to allow developers to create **memory-based** trainers/bots/helpers for Growtopia without having to spend time finding game data and constantly updating the offsets. The latter is achieved with **dynamic offset generation** - see [`GameAddresses.cs`](https://github.com/TheLeftExit/TeslaXI/blob/master/TheLeftExit.Growtopia/GameAddresses.cs) and [`PointerQuery.cs`](https://github.com/TheLeftExit/TeslaXI/blob/master/TheLeftExit.Memory/PointerQuery.cs) for more details.

## Credits
 - Huge thanks to the [**TeslaX Discord Server**](https://discord.gg/Px457A4fh4) for its efforts to decode game memory. Feel free to join if you're interested in helping or learning.
 - Thanks to [**0xD3F**](https://github.com/DefaultO) for continued cooperation in reversing the game.
 - Decoder for items.dat transcribed from [GrowtopiaNoobs/Growtopia_ItemsDecoder](https://github.com/GrowtopiaNoobs/Growtopia_ItemsDecoder).
 - Decoder for RTTEX files transcribed from [Nenkai/RTPackConverter](https://github.com/Nenkai/RTPackConverter).
 
This project is [unlicensed](https://unlicense.org/). Feel free to use any parts of it in your own projects - that's mostly what it's for.

## Current progress
Done:
 - Memory reading/querying API (`TheLeftExit.Memory`)
 - Proof-of-concept version of a dynamic offset generator (`TheLeftExit.Growtopia.GameAddresses`)
 - `RTPACK` and `items.dat` decoder API
 
To do:
 - API encapsulating `GameAddresses` to provide game information directly
 - The entirety of the farmbot (WPF interface, resource caching, bot logic)
 - User-friendly items.dat explorer (property display, filtering)