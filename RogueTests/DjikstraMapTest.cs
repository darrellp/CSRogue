using System;
using System.Collections.Generic;
using CSRogue.Map_Generation;
using CSRogue.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RogueTests
{
    [TestClass]
    public class DjikstraMapTest
    {
        [TestMethod]
        public void TestDjikstraMap()
        {
            Dictionary<int, List<MapCoordinates>> goals = new Dictionary<int, List<MapCoordinates>>()
            {
                {0, new List<MapCoordinates>() {new MapCoordinates(0, 0)}}
            };
            var djikstra = new DjikstraMap(10, 10, goals, (i, j) => true);
            var map = djikstra.CreateMap();
            for (var iCol = 0; iCol < 10; iCol++)
            {
                for (var iRow = 0; iRow < 10; iRow++)
                {
                    Assert.AreEqual(Math.Max(iRow, iCol), map[iRow][iCol]);
                }
            }
        }
}
}
