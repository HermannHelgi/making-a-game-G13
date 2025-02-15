Start by downloading the unity hub ( https://unity.com/download ) and install the unity editor version 6 ( 6000.0.36f1 )  

![image.png](attachment:5e20bbee-41a8-4060-a897-02c0a4d2aada:image.png)

![image.png](attachment:bf4141e2-22a0-4d6c-9463-37b9ee9ba319:image.png)

Create a folder on your computer where you would like to hold the repository and clone it from here

https://github.com/HermannHelgi/making-a-game-G13

Once you have cloned the repo and have finished installing unity 6 then go into the unity hub → projects and click add. From there select the location of the repo.

![image.png](attachment:90f081c3-db43-48ee-9a75-8338954df878:image.png)

![Choose add project from disk](attachment:9365f349-09c7-46e3-8a92-b0f97c4c72b7:image.png)

Choose add project from disk

The folder should look something like this

![image.png](attachment:92db6e8c-42dd-49a7-9697-22e5781d6df8:image.png)

Press open and launch the project. Unity should create the rest of the files needed to run the unity editor.

## Git flow

We will be following Git flow, a workflow structure for git. https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow

To do this we will have a main branch which will have a stable version of the game which is agreed upon by the group. the main branch will be used for versions we wish to use for playtests or demos.

Then we will have the develop branch, a branch from the main branch. This branch will contain things which are still in development. The develop branch is a shared branch for the developers to merge features to and polish them or use them before joining to the main branch.

Then we will have Feature branches. These are branches branched from the develop branch. A feature branch is a branch created to implement a specific feature, once the feature is complete it merges onto the develop branch. 

When creating a feature branch the branch name should be prefixed with feature/*. Example in command line “git checkout -b feature/player-controls” or “git branch feature/storage-system”

If a name of a branch is more then one word then words should be separated by a dash -

## Unity file structure

Within the Assets folder are three folders which help with git flow. 

There is the features folder. In here are all the features created from each feature branch. When a developer creates a new feature branch the first thing they should do is to create a new folder within the feature folder in unity and name the folder the same thing as the branch or the feature. All work that is done on that feature should be within that folder. That is to say prefabs, levels, scripts, scenes, etc. If for whatever reason anything outside of this folder is changed the team should be let known via discord.

Then there is the development folder. This folder will contain features which are ready for to be used for development use. This folder may be updated when updating the develop branch.

Then there is the versions folder. This is the same as the main branch. Here finished demos or versions of the game are contained.

![image.png](attachment:f9b08c94-9ada-4818-9d76-0b9275619c18:image.png)

<aside>
⚠️

Remember to periodically commit to prevent loss of data

</aside>
