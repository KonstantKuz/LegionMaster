using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using LegionMaster.Location.GridArena.Model;
using LegionMaster.NavMap.Model;
using LegionMaster.NavMap.Service;
using LegionMaster.Units.Component;
using NUnit.Framework;
using UnityEngine;
using Zenject;

namespace Tests.NavMap
{
    [TestFixture]
    public class NavMapTest : ZenjectUnitTestFixture
    {
        
        private static NavMapService CreateService() => new NavMapService();
        private static UnitType TargetUnit => UnitType.AI;
     
        [NotNull]
        private static readonly string[] DATA_GRID = new string[]
        {
                " EP",
                "P P",
                "   "
        };
 
        [Test]
        public void TestFindPath()
        {
            var service = CreateService();
            service.CreateMap(ConvertGridToMap(DATA_GRID));
            
            var startCell = new CellId(0, 0);
            var path = service.FindPath(startCell, TargetUnit, 1);
            var expectedPathCells = new[] {new CellId(0, 1), new CellId(1, 1)};
            var expectedTargetCell = new CellId(2, 1);
            AssertSuccessPath(path, expectedPathCells, expectedTargetCell, service.NavMap.Map);
            
            path = service.FindPath(startCell, TargetUnit, 2);
            Assert.That(path.NoPath, Is.True);
            Assert.That(path.TargetCell, Is.Not.Null);
        } 
        private void AssertSuccessPath(TargetSearchResult path, CellId[] expectedPathCells, CellId expectedTargetCell, Cell[,] map)
        { 
            Assert.That(path.Path, Is.EqualTo(expectedPathCells));
            Assert.That(path.TargetCell.CellId, Is.EqualTo(expectedTargetCell));
            LogPathAndPrintGrid(path, map);
        }
        private static readonly string[] DATA_GRID_1 = new string[]
        {
                "  E ",
                "    ",
                " E E",
                "    ",
                " PP ",
                "   P",
                "P   ",
                "    "
        };
        [Test]
        public void TestFindFinishCells1()
        {
            var service = CreateService();
            service.CreateMap(ConvertGridToMap(DATA_GRID_1));
            
            var startCell = new CellId(0, 0);
            var path = service.FindPath(startCell, TargetUnit, 1);
            var expectedFinishCells = new[] {new CellId(4, 0), new CellId(4, 1), new CellId(4, 2)};
            var expectedTargetCell = new CellId(5, 1);
            AssertSuccessFinishCells(path, expectedFinishCells, expectedTargetCell, service.NavMap.Map);;

            startCell = new CellId(0, 3);
            path = service.FindPath(startCell, TargetUnit, 2);
            expectedFinishCells = new[] {new CellId(3, 0), new CellId(3, 3)}; 
            expectedTargetCell = new CellId(5, 1);
            AssertSuccessFinishCells(path, expectedFinishCells, expectedTargetCell, service.NavMap.Map);
        }    
        private static readonly string[] DATA_GRID_2 = new string[]
        {
                "  E ",
                "PPPP",
                "    ",
                " P  ",
                " PP ",
                "   P",
                "PP  ",
                "    "
        };
        [Test]
        public void TestFindFinishCells2()
        {
            var service = CreateService();
            service.CreateMap(ConvertGridToMap(DATA_GRID_2));
            
            var startCell = new CellId(0, 0);
            var path = service.FindPath(startCell, TargetUnit, 2);
            var expectedFinishCells = new[] {new CellId(5, 0), new CellId(5, 1), new CellId(5, 2), new CellId(5, 3)};
            var expectedTargetCell = new CellId(7, 2);
            
            AssertSuccessFinishCells(path, expectedFinishCells, expectedTargetCell, service.NavMap.Map);
            
        } 
        [Test]
        public void TestInvalidPath()
        {
            var service = CreateService();
            service.CreateMap(ConvertGridToMap(DATA_GRID_2));
            
            var startCell = new CellId(0, 0);
            var path = service.FindPath(startCell, TargetUnit, 1);
            
            Assert.That(path.Path, Is.EqualTo(null));  
            Assert.That(path.TargetCell, Is.EqualTo(null));

        }  
        private static readonly string[] DATA_GRID_3 = new string[]
        {
                "    ",
                "    ",
                "    ",
                "    ",
                "    ",
                "    ",
                "PP  ",
                " E  "
        }; 
        [Test]
        public void TestPathToCurrentCell()
        {
            var service = CreateService();
            service.CreateMap(ConvertGridToMap(DATA_GRID_3));
            
            var startCell = new CellId(0, 0);
            var path = service.FindPath(startCell, TargetUnit, 1);
            Assert.That(path.NoPath, Is.True);
            Assert.That(path.TargetCell.CellId, Is.EqualTo(new CellId(0, 1)));
            
            service.CreateMap(ConvertGridToMap(DATA_GRID_2));
            startCell = new CellId(4, 0);
            path = service.FindPath(startCell, TargetUnit, 3);
            Assert.That(path.NoPath, Is.True);
            Assert.That(path.TargetCell.CellId, Is.EqualTo(new CellId(7, 2)));

            service.CreateMap(ConvertGridToMap(DATA_GRID_2));
            startCell = new CellId(0, 3);
            path = service.FindPath(startCell, TargetUnit, 9);
            Assert.That(path.NoPath, Is.True);
            Assert.That(path.TargetCell.CellId, Is.EqualTo(new CellId(7, 2)));
        }

          
        private static readonly string[] DATA_GRID_4 = new string[]
        {
                "    ",
                "P   ",
                " E  ",
                " PPP",
                "    ",
                " P  ",
                "PP  ",
                "    "
        };
        [Test]
        public void TestMoveUnit()
        {
            var service = CreateService();
            service.CreateMap(ConvertGridToMap(DATA_GRID_4));
            
            var startCell = new CellId(0, 0);
            var path = service.FindPath(startCell, TargetUnit, 1);
            var expectedFinishCells = new[] {new CellId(4, 0)};
            var expectedTargetCell = new CellId(5, 1);
            AssertSuccessFinishCells(path, expectedFinishCells, expectedTargetCell, service.NavMap.Map);

            var nextTargetCell = new CellId(6, 1);
            service.MoveUnit(expectedTargetCell, nextTargetCell, UnitType.AI, "testUnit");
            
            path = service.FindPath(startCell, TargetUnit, 1);
            expectedFinishCells = new[] {new CellId(5, 0)};
            expectedTargetCell = nextTargetCell;
            AssertSuccessFinishCells(path, expectedFinishCells, expectedTargetCell, service.NavMap.Map);
            
            nextTargetCell = new CellId(7, 1);
            service.MoveUnit(expectedTargetCell, nextTargetCell, UnitType.AI, "testUnit");
            
            path = service.FindPath(startCell, TargetUnit, 1);
            expectedFinishCells = new[] {new CellId(6, 1)};
            expectedTargetCell = nextTargetCell;
            AssertSuccessFinishCells(path, expectedFinishCells, expectedTargetCell, service.NavMap.Map);

        }
        private void AssertCurrentCell(TargetSearchResult path, CellId startCell, CellId expectedTargetCell, Cell[,] map)
        {
            Assert.That(path.Path.Count, Is.EqualTo(1)); 
            Assert.That(path.StartCell, Is.EqualTo(startCell));   
            Assert.That(path.FinishCell, Is.EqualTo(startCell));    
            Assert.That(path.TargetCell.CellId, Is.EqualTo(expectedTargetCell));
            LogPathAndPrintGrid(path, map);
        }
        private void AssertSuccessFinishCells(TargetSearchResult path, CellId[] expectedFinishCells, CellId expectedTargetCell, Cell[,] map)
        {
            CollectionAssert.Contains(expectedFinishCells, path.FinishCell);       
            Assert.That(path.TargetCell.CellId, Is.EqualTo(expectedTargetCell));
            LogPathAndPrintGrid(path, map);
        }
        
        private void LogPathAndPrintGrid(TargetSearchResult path, Cell[,] map)
        {
            Debug.Log($"FinishCell={path.FinishCell}");    
            Debug.Log($"TargetCell={path.TargetCell}");
            for (int i = 0; i < path.Path.Count; i++) {
                Debug.Log($"Cell_{i}:={path.Path[i]}");
            }
            PrintGridWithPath(map, path); 
        }

        private void PrintGridWithPath(Cell[,] map, TargetSearchResult path)
        {
            var printedGrid = new List<string>();
            for (int y = 0; y < map.GetLength(0); y++) {
                char[] chars = new char[map.GetLength(1)];
                for (int x = 0; x < map.GetLength(1); x++) {
                    chars[x] = ConvertStateToChar(map[y, x].State);
                    if (path.Path.Contains(new CellId(y, x))) {
                        chars[x] = '*';
                    }
                }
                var lineMap = new string(chars); 
                printedGrid.Add($"'{lineMap}'");
            }
            printedGrid.Reverse();
            printedGrid.ForEach(line => Debug.Log(line));

        }
        private Cell[,] ConvertGridToMap(string[] dataGrid)
        {
            Cell[,] map = new Cell[dataGrid.Length, dataGrid[0].Length];
            for (int y = 0; y < map.GetLength(0); y++) {
                for (int x = 0; x < map.GetLength(1); x++) {
                    var state = ConvertCharToState(dataGrid[dataGrid.Length - 1 - y][x]);
                    var unitId = state == CellState.Empty ? "" : "testUnit";
                    map[y, x] = new Cell(new CellId(y, x), state, unitId);
                }
            }
            return map;
        }
        private CellState ConvertCharToState(char cell)
        {
            return cell switch {
                    ' ' => CellState.Empty,
                    'P' => CellState.BusyPlayer,
                    'E' => CellState.BusyEnemy,
                    _ => throw new ArgumentOutOfRangeException(nameof(cell), cell, null)
            };
        }
        private char ConvertStateToChar(CellState state)
        {
            return state switch {
                    CellState.Empty => ' ',
                    CellState.BusyPlayer => 'P',
                    CellState.BusyEnemy => 'E',
                    _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };
        }
    }
}