# Tomb Raider Gameflow Editor
TRGE lets you edit the script file for Tomb Raider II, Tomb Raider II Gold, Tomb Raider III and Tomb Raider III Gold. The features described below can be modified using this tool.

# Thanks
First things first, big thanks to:

* DanzaG - for [TR2LevelReader](https://github.com/DanzaG/TR2-Rando), an invaluable tool for reading and editing TR2 level files. Check out TR2-Rando - a fantastic randomizer for TR2.
* chreden - for [TRView](https://github.com/chreden/trview), another invaluable tool which was used to pick default pistol locations for unarmed levels.
* Paolone, Popov - for [TRViewer](https://www.aspidetr.com/en/tools/tr-viewer/trviewer/), which was used to amend Home Sweet Home to support all weapons, weapon animations, moveables and sprites.

# Usage

This application uses a MIT license as described in the LICENSE file. Follow the steps below to download and use the application.

_Prerequisites_
* Windows 7 SP1, Windows 8.1, Windows 10
* .NET Framework 4.7.2

_Install Steps_
* Download the latest release from https://github.com/lahm86/TRGameflowEditor/releases
  * You only need to download the main TRGE zip file (e.g. TRGE_0.9.3-beta.zip) to be able to run the application normally.
  * The .wad files shown on the release page are downloaded from within the software if they are required (see the note in [Audio Tracks](#audio-tracks) below).
  * Feel free to download the source files and build the solution in Visual Studio. Use package manager to resolve missing dependencies.
* Extract the zip file to any location on your PC.
* Run TRGE.exe

Once TRGE is open, you can select the data folder that contains the relevent files to edit. As a minimum, this folder must contain the TOMBPC.dat (or TOMBPSX.dat) file, but to allow level file editing for TR2 and TR2G, the associated .TR2 level files must also be in this folder. The simplest method is to point TRGE to your default Tomb Raider installation folder, and select the data folder within. For example, if you have the Steam version of TR2 installed, the following location should be selected.

C:\Program Files (x86)\Steam\steamapps\common\Tomb Raider (II)\data

TRGE will backup the files when you open the folder and so you can restore at any point by choosing **Edit** > **Restore To Default**. TRGE also keeps a history of the folders you open, so the next time you launch it, you can re-select previous folders easily from the displayed list and menu items.

![TRGE Main Window](https://github.com/lahm86/TRGameflowEditor/blob/main/Resources/TRGE.png)

## Title Screen & Passport Options

* Edit whether or not the title screen is displayed when the game starts. If it's disabled, launching the game will take you straight to the first level. **Exit to Title** is then replaced in the in-game passport with **New Game**. Use Alt+F4 to exit the game.
* You can choose to have the list of levels displayed in the passport on the **New Game** page. Note that by default this will also list any demo levels.
* You can choose to disable saving and loading, both via the passport and F5/F6.
* You can choose to disable the option ring, so it will not be possible to reach the sound and controls options other than from the title screen (if that's enabled).
* You can choose to disable the demo levels that show normally in the title screen.
* You can change the length of time the title screen waits before showing the demo levels.
* You can choose to remove Lara's Home from the title screen.

## Interruptions & Cheats

* You can disable FMVs globally, which will remove the script commands from each level and before the title screen.
* You can disable all cutscenes across all levels.
* You can disable start animations across all levels - so for example, in Offshore Rig, you don't need to wait for Lara to wake up.
* You can have the game ignore any cheat inputs.
* For the PSX Beta version, you can turn on DOZY.

## Level Sequencing

You can manually organise the sequence that levels are played. Required pickups will always be available in TR2, for example, if Barkhang Monastry is picked as the first level, The Seraph will be in Lara's backpack by default.

## Unarmed Levels

You can manually choose which levels Lara starts with all weapons removed. For TR2 and TR2G, the pistols will be added to the levels for Lara to find. Thanks to [DanzaG's TR2LevelReader](https://github.com/DanzaG/TR2-Rando) for making this possible. 

## Ammoless Levels

You can manually choose which levels Lara starts with all ammo, medi-packs and flares removed (Home Sweet Home in TR2 is the default). Note that no extra items will be added to these levels.

## Secret Rewards

For TR2 and TR2G, you can manually allocate the items rewarded to Lara for collecting all secrets. Note that while you can select flares to be rewarded, the sprite does not show on screen because the scripting allocates a single flare rather than a box. The flare does get added to inventory regardless. This issue is being investigated further.

## Sunsets

For TR2 and TR2G, you can choose to have the lighting dim in particular rooms over a period of 20 minutes. This happens by default in Bartoli's Hideout. It only applies to specific rooms per level.

## Audio Tracks

You can change the title screen music and the ambient track that plays in each level. You can also change the sound played when a secret is found, but this is a global setting and can't be configured per level. When selecting which audio tracks to use in the game, you can choose to play the track first in the UI and/or to export it to a .WAV file. The first time you do this, TRGE will download an additional resource file from the GitHub release page for this application. This action only needs to happen once per game version (there is currently one file for both TR2 and TR2G, and another for TR3 and TR3G).

![TRGE Audio Download](https://github.com/lahm86/TRGameflowEditor/blob/main/Resources/audiodownload.png)
