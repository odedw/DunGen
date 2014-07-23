﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using DunGen.Engine.Contracts;
using DunGen.Engine.Models;

namespace DunGen.Engine.Implementations
{
    public class MazeGenerator : IMapProcessor
    {
        private readonly IRandomizer mRandomizer;
        public event MapChangedDelegate MapChanged;
        public string ActionString { get { return "Generating maze"; } }

        public MazeGenerator(IRandomizer randomizer)
        {
            mRandomizer = randomizer;
        }

        public void ProcessMap(Map map, DungeonConfiguration configuration)
        {
            //Start with a rectangular grid, x units wide and y units tall. Mark each cell in the grid unvisited
            var visitedCells = new HashSet<Cell>();
            var deadEndCells = new HashSet<Cell>();
            Direction? previousDirection = null;

            //Pick a random cell in the grid and mark it visited. This is the current cell. 
            var currentCell = mRandomizer.GetRandomCell(map);
            currentCell.Terrain = TerrainType.Floor;
            while (visitedCells.Count < map.Width*map.Height)
            {
                var oldCell = currentCell;
                var changed = false;
                visitedCells.Add(currentCell);

                //From the current cell, pick a random direction (north, south, east, or west). 
                //If (1) there is no cell adjacent to the current cell in that direction, or (2) if 
                //the adjacent cell in that direction has been visited, then that direction 
                //is invalid, and you must pick a different random direction. 
                var direction = GetRandomValidDirection(map, currentCell, visitedCells, configuration.Randomness, previousDirection);
                if (direction.HasValue)
                {
                    //Let's call the cell in the chosen direction C. Create a corridor between the 
                    //current cell and C, and then make C the current cell. Mark C visited.
                    changed = currentCell.Sides[direction.Value] != SideType.Open;
                    currentCell = map.GetAdjacentCell(currentCell, direction.Value);
                    currentCell.Sides[direction.Value.Opposite()] = oldCell.Sides[direction.Value] = SideType.Open;
                    previousDirection = direction;
                }
                else
                {
                    //If all directions are invalid, pick a different random visited cell in the grid and start this step over again.
                    deadEndCells.Add(currentCell);
                    currentCell = mRandomizer.GetRandomItem(visitedCells, deadEndCells);
                }
                if (currentCell.Terrain == TerrainType.Floor && !changed) continue;

                currentCell.Terrain = TerrainType.Floor;
                if (MapChanged != null)
                {
                    MapChanged(this, new MapChangedDelegateArgs(){Map = map, CellsChanged = new Cell[]{currentCell, oldCell}});
                }

                //Repeat until all cells in the grid have been visited.
            }
        }

        private Direction? GetRandomValidDirection(Map map, Cell cell, ICollection<Cell> visitedCells, double randomness, Direction? previousDirection)
        {
            //Randomness determines how often the direction of a corridor changes
            if (previousDirection.HasValue &&
                randomness < 1 && 
                mRandomizer.GetRandomDouble() > randomness &&
                IsDirectionValid(map, cell, previousDirection.Value, visitedCells))
            {
                return previousDirection;
            }

            var invalidDirections = new List<Direction>();
            while (invalidDirections.Count < Enum.GetValues(typeof (Direction)).Length)
            {
                var direction = mRandomizer.GetRandomEnumValue(invalidDirections);
                if (IsDirectionValid(map, cell, direction, visitedCells))
                    return direction;
                invalidDirections.Add(direction);
            }
            return null;
        }

        private bool IsDirectionValid(Map map, Cell cell, Direction direction, ICollection<Cell> visitedCells)
        {
            var adjacentCell = map.GetAdjacentCell(cell, direction);
            return adjacentCell != null && !visitedCells.Contains(adjacentCell);

        }   
    }
}
