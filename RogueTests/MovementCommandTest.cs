using System;
using CSRogue.GameControl;
using CSRogue.GameControl.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSRogue.Map_Generation;
using CSRogue.RogueEventArgs;

namespace RogueTests
{
    [TestClass]
    public class MovementCommandTest
    {
        private CreatureMoveEventArgs moveArgs;

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
            var moveCmd = new MovementCommand(new MapCoordinates(1,1));
            game.HeroMoveEvent += GameOnHeroMoveEvent;
            game.EnqueueAndProcess(moveCmd);
            Assert.AreEqual(new MapCoordinates(1,1), map.Player.Location);
            Assert.IsNotNull(moveArgs);
            Assert.IsTrue(moveArgs.IsPlayer);
            Assert.AreEqual(new MapCoordinates(0, 0), moveArgs.PreviousCreatureLocation);
            Assert.AreEqual(new MapCoordinates(1, 1), moveArgs.CreatureDestination);
            Assert.IsFalse(moveArgs.IsBlocked);
            Assert.IsFalse(moveArgs.IsRunning);
            Assert.IsFalse(moveArgs.IsFirstTimePlacement);
        }

        private void GameOnHeroMoveEvent(object sender, CreatureMoveEventArgs creatureMoveEventArgs)
        {
            moveArgs = creatureMoveEventArgs;
        }
    }
}
