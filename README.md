Ironstag
========

<p align="center">
  <img src="https://raw.githubusercontent.com/RochesterIndiesAnonymous/Ironstag/master/WesternSpaceShared/Content/Textures/Cutscenes/Intro7.png" />
</p>

XNA/MonoGame-based, megamanesque platformer with a space-western theme.

Created by students at RIT for a beginner's game development class.

## TODO

- Build instructions
- Conquer the world
- Update [author info](https://help.github.com/articles/changing-author-info) with the correct github profiles (where applicable)

## Available Platforms
- Windows
- MacOS Monterey (Intel) (Works in VS Code, Binary doesnt work yet)

## How to dowload the latest version of this application

Its available on github release page

https://github.com/RochesterIndiesAnonymous/Ironstag/releases

## How to build this application

Right now this project is buildable within Visual Studio Code 2022 and directly from the command line/terminal with Cake Build (https://cakebuild.net/). We also have automated build pipelines with build/release github action(s) the post the built project on the github releases page. We are still working on symversioning our releases.

### Visual Studio 2022

- NET Core 6.0 (DesktopGL)
- Monogame 3.8
- MGCB - Builds Content Files
- MGCBFX - Builds Shaders

### Cake Build

dotnet cake

## What are the dependencies for the current version

- Monogame 3.8
- NET Core 6.0
