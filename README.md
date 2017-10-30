# TileGame

This is a game I worked on for a few weeks. There is not much to the game in terms of gameplay, however I spent a lot of time programming the AI, workers and building.
The AI uses A* pathfinding, using a grid which is generated and updated based on tiles built.
The workers are given a building job (i.e to build a wall), they then request a path from the A* implementation and then move to the job and build.
The building mechanic currently involves concrete and walls. Walls cannot be passed through by workers and require concrete to be built on. The player can select an area
to build concrete and a 1xN area to build a  wall.

The game is made in unity.
To view the code, open Assets/Scripts/----.cs
To play the game, go to Build/TileGame.exe