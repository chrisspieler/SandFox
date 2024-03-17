# SandFox
SandFox is a modding and debugging tool for S&box that uses DLL injection to load a .NET assembly in to a running instance of S&box.

## Features
- Adds console commands to support debugging such as `scene_list` and `gameobject_dump`
- Adds console commands to support modifying game behavior such as `map_change` and `freecam_start`

## How to Use
1. Ensure that S&box is already running a game
2. Run ManagedClient.exe
3. In S&box, press F1 to open the S&box console
4. If "Successful SandFoxSystem init." was logged, then all the ConCmds and ConVars in ManagedPayload should be available to use.
5. After closing S&box, SandFox commands will not be useable again until after restarting S&box and running ManagedClient.exe again.

## Modifying
To add new behavior to SandFox, add a ConCmd or ConVar to the ManagedPayload C# project, build it, and copy the output DLL to same directory 
that ManagedClient.exe is in. 

Loading .NET DLL plugins is planned to be supported in the future, so you wouldn't have to modify ManagedPayload itself.

## Future Plans
- Allow SandFox to work for multiple different games without needing to restart S&box and reinject the payload each time.
- Add a plugin system to that allows different DLLs to be loaded depending on game.

## Troubleshooting
- Issue: Windows Defender thinks that Injector.exe is a virus
- Resolution: This is a false positive. Injector.exe is just a tool that forces a process to load a DLL. Because Injector.exe is fairly basic, it is probably semantically equivalent to similar tools used for malicious purposes. In the future, I might modify ManagedClient so that it no longer requires Injector.exe to function. 
