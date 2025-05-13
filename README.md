# READ ME

Hello and welcome to our development environment for Cold Bargain.

We made this game as a team of 5 students at Reykjavik University for a final project for our Computer Science BSc. degree.
Over the course, the project is overseen by Steingerður Lóa and Kristín Bestla Þórsdóttir, our instructors, Ingólfur Halldórsson, our examiner, and Jonathan Pierce, our project owner.

The project was submitted on May 16th, 2025.

The team consists of:
Ágúst Máni Þorsteinsson 
Daði Rúnarsson
Hermann Helgi Þrastarson
Hugi Freyr Álfgeirsson
Júlía Ósk Tómasdóttir


This ReadMe includes the following:

* Basic Information
* System Requirements
* Installation and Set-up Instructions 
* Operations Manual
* Game Manual

## Basic Information

Game Description: The project is a survival-horror game set in a cold, dark environment where you as the player have to survive the elements as well as the monster that hunts you. You must explore and collect special items located around the map to use them to bargain with a Witch for her to help you escape.

Core Mechanics: Exploration, warmth and hunger management, item collection, enemy avoidance, bargaining, storing, time management, environmental storytelling.

Target Platform: PC, macOS

Engine and Tools Used: Unity 6, Visual Studio Code, GitHub, Git LFS, Krita, Blender, Audacity.

## System Requirements

Minimum: 
OS: Windows 10 / MacOS 10.13 High Sierra
RAM: 4 GB
Storage Space: 500 MB 
CPU: Dual-core 2.0 GHz
GPU: Integrated (e.g. AMD or Intel)
Resolution: 1280x720 minimum
DirectX: Version 11 (Windows)

Recommended:
RAM: 8 GB
CPU: Quad-core 2.5 GHz
GPU: Dedicated GPU (e.g., NVIDIA GTX 1050)
Resolution: 1920x1080

Input: Keyboard & mouse


## Installation and Set-up Instructions

Start by downloading the Unity Hub ( https://unity.com/download ) and install the Unity Editor version 6 ( 6000.0.36f1 )  

![image.png](attachment:5e20bbee-41a8-4060-a897-02c0a4d2aada:image.png)

![image.png](attachment:bf4141e2-22a0-4d6c-9463-37b9ee9ba319:image.png)

Open a terminal and navigate to a folder where you'd like to store the repository.
Use the command [git clone https://github.com/HermannHelgi/making-a-game-G13] 

Once you have cloned the repo, open Unity Hub and Add Project from Disk. From there select the location of the repo.

![image.png](attachment:90f081c3-db43-48ee-9a75-8338954df878:image.png)

![Choose add project from disk](attachment:9365f349-09c7-46e3-8a92-b0f97c4c72b7:image.png)

Launch the project from the Editor (Hub).
Once you have the project open, navigate to X folder and open the X scene. Then you can hit Play the top of the scene window.

Run through Itch.io:

Open this link and follow the instructions on how to run the game through an application file:
itch.io/krummigames/ColdBargain


## Operations Manual v.2

### Technical Architecture

**Git flow**

We will be following Git flow, a workflow structure for git: https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow

The structure is as follows: A Main branch which is only updated with stable release versions of the game throughout the development process; A development branch, which is updated once a feature is stable and completed and is ready to be implemented into the game to be integrated with other features. The development branch is used to prepare and test before pushing onto the Main branch; Feature branches that are created for each feature that will be implemented for the game, i.e. the inventory system.

When creating a feature branch the branch name should be prefixed with feature/*. Example in command line “git checkout -b feature/player-controls” or “git branch feature/storage-system”

If a name of a branch is more then one word then words should be separated by a dash -

<aside>
⚠️
Remember to periodically commit to prevent loss of data
</aside>

#### Script Overview: List of core scripts and their functions.

Scene Structure: High-level layout of your Unity scenes

**Unity File Structure**

Within the Assets folder are three folders which help with git flow. 

There is the features folder. In here are all the features created from each feature branch. When a developer creates a new feature branch the first thing they should do is to create a new folder within the feature folder in unity and name the folder the same thing as the branch or the feature. All work that is done on that feature should be within that folder. That is to say prefabs, levels, scripts, scenes, etc. If for whatever reason anything outside of this folder is changed the team should be let known via discord.

Then there is the development folder. This folder will contain features which are ready for to be used for development use. This folder may be updated when updating the develop branch.

Then there is the versions folder. This is the same as the main branch. Here finished demos or versions of the game are contained.

![image.png](attachment:f9b08c94-9ada-4818-9d76-0b9275619c18:image.png)

**AI Architecture**
State machines, NavMesh usage, predator-prey logic.

**Animation/Rigging**
How characters and animals are animated.

#### Art and Audio

Visual Style: Brief description of your aesthetic choices (e.g., low-poly, hand-painted, stylized lighting).

Assets: Mention if models, textures, or sounds were custom-made or sourced (include credits).

https://assetstore.unity.com/packages/3d/environments/low-poly-snowy-lands-132172

https://assetstore.unity.com/packages/3d/environments/fantasy/low-poly-ice-world-83909

https://assetstore.unity.com/packages/3d/environments/low-poly-trees-and-vegetation-pack-265300

https://assetstore.unity.com/packages/3d/environments/low-poly-woods-lifestyle-65306

https://assetstore.unity.com/packages/3d/environments/landscapes/cross-plains-lowpoly-environment-by-unvik-3d-203644

https://assetstore.unity.com/packages/3d/environments/fantasy/tent-pack-19370

https://assetstore.unity.com/packages/2d/textures-materials/blood-splatter-decal-package-7518

https://assetstore.unity.com/packages/3d/props/item-pack-survival-131598

https://sketchfab.com/3d-models/stylized-sack-coinpurse-419a105395644219afea37b78d5c906b

https://freesound.org/people/Gerent/sounds/558397/

https://freesound.org/people/Benboncan/sounds/116663/

https://freesound.org/people/13GPanska_Taborsky_Radovan/sounds/378134/

https://freesound.org/people/straget/sounds/402809/

https://freesound.org/people/Benboncan/sounds/63220/

https://sketchfab.com/3d-models/low-poly-chest-ede8d988e3724ed395bc20419040d2da#download

https://sketchfab.com/3d-models/free-pack-rocks-stylized-7c60b4d1b8ab4187965f30c5e0212fc0

https://sketchfab.com/3d-models/low-poly-beer-bottle-d6a3307927254388a06b8d53aa847fcd

https://sketchfab.com/3d-models/low-poly-vegetable-soup-d83903da681843128b4f8d9e92592be4

https://poly.pizza/m/btWmPNVSKUc

__**MISSING MODELS? skull/bones, hand? etc**__

**Audio Systems:**
Sound Manager

### Testing and Debugging

Testing Methods: How you tested gameplay, performance, and bug fixing.
Known Issues: Any bugs or incomplete features.
Playtesting Feedback: (If applicable) summaries or insights gained.

## Future Improvements: What you'd add/fix with more time
