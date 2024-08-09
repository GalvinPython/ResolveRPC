# ResolveRPC
A Discord RPC client for DaVinci Resolve, built using C#

# How it works
This doesn't use the Resolve API, mainly because I wanted this to work on free versions of Resolve, so instead it does two things
- Checks if DaVinci Resolve is running
- If it is, get the project name from the window title using Windows

And that's it! It's not the best in detail because I can't use the built-in Python/Lua libraries but at least it shows what project you're editing

# System Requirements
- .NET 4.7.2 or 4.8 required (If you are running the latest Windows 10 update or Windows 11, you will be fine)
- 64-bit CPU (Well you need it anyway to run Resolve)
- Windows only

# Things to note
- Runs in background so will require you to kill it through Task Manager or vice versa
- Doesn't run on start-up
