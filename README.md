# Standard-Game-Menu
Standardised Procedural Game Menu for PC and Console

Get it here: https://github.com/michael-evan-allison/Standard-Game-Menu

Tested In: Unity URP 2022.2.2f1, Unity URP 2022.2.9f1

Version History
- 16/06/2023 - Removed Text Mesh Pro Depedency
- 20/02/2023 - Initial Commit

Requirements:
- The new unity input system

Features
- Two menu types (Main Menu and In Game Menu)
- Procedurally generates the menu from Json file
- Can generate an infinite number of submenus
- Standardised design, easy to modify
- Top bar on ingame menu can be cycled through using the shoulder buttons on a controller
- Pressing 'Escape' or 'B' on an X-Box Controller or 'Circle' on a Playstation Controller will take you back up the menu tree
- Menu dynamically resizes to accomodate number of menu items
- Automatically assigns functions and variables to UI buttons using a function dictionary

Main Menu
![MainMenu](https://user-images.githubusercontent.com/67586167/219983439-fcf32d17-136d-494b-aa78-2b0b55d2ad25.jpg)

In Game Menu
![IngameMenu](https://user-images.githubusercontent.com/67586167/219983444-c154740e-0296-4776-9c41-ba5f1a445361.jpg)

How to use
- Drop the files into you assets folder
- Drag the menu you wish to use into an empty scene with a camera (you can find them in Resources/Menu/Menus)
- Press 'play', the menu should load its default layout
- Add content by editing the relevant Json menu file in Resources/MenuFiles
- You can add new functions to your menu by writing a new void function in 'MainMenuFunctions.cs' or 'InGameMenuFunctions.cs' you then need to add the function to the function dictionary at the top of script 
- You can edit the button styles or make a new one by edting the prefabs in Resources/MenuButtons

Credits
- The included icons are by Kenney.nl https://opengameart.org/content/game-icons

Apart from the icons (which are belong to Kenney.nl) the menu is under the Apache License 2.0
