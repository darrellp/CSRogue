using CSRogue.GameControl;
using CSRogue.GameControl.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSRogue.Map_Generation;

namespace RogueTests
{
    [TestClass]
    public class MovementCommandTest
    {
        [TestMethod]
        public void TestMovement()
        {
            const string mapString =
@"@..............
...............
...............
...............";
            var game = new Game(Initialization.ItemFactory);
            var map = new GameMap(10, game, mapString);
            var levelCmd = new NewLevelCommand(0, map);
            game.EnqueueAndProcess(levelCmd);
            var moveCmd = new MovementCommand(MoveDirection.MoveLowerRight);
            game.EnqueueAndProcess(moveCmd);
            Assert.AreEqual(new MapCoordinates(1,1), map.Player.Location);
        }
    }
}
