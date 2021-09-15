using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minesweeper.Model;
using System;
using System.Drawing;
using System.Linq;

namespace Minesweeper.Tests.Model
{
    [TestClass]
    public class GameTest
    {

        [TestMethod]
        public void Create()
        {
            // A simple and easy field that should have unique positions for each mine

            var result = Game.Create(new Size(10, 15), 50);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Field);
            Assert.AreEqual(10, result.Field.Width);
            Assert.AreEqual(15, result.Field.Height);
            Assert.IsNotNull(result.Mines);
            Assert.AreEqual(50, result.Mines.Count());
            Assert.IsTrue(result.Mines.All(p => p.X >= 0 && p.X < result.Field.Width && p.Y >= 0 && p.Y < result.Field.Height), "The mines are within the field");
            Assert.AreEqual(50, result.Mines.Distinct().Count(), "The mines' positions are not repeated");
        }

        [TestMethod]
        public void Create_HighSaturation()
        {
            // The deployment doesn't break under high saturation (mine count is close to field size)

            var result = Game.Create(new Size(100, 100), 9999);

            Assert.IsNotNull(result);
            Assert.AreEqual(9999, result.Mines.Count());
            Assert.AreEqual(9999, result.Mines.Distinct().Count(), "The mines' positions are not repeated");
        }

        [TestMethod]
        public void Create_Invalid_FieldSize()
        {
            // The field is too small/big

            Assert.ThrowsException<ArgumentException>(() => Game.Create(new Size(0, 10), 3));
            Assert.ThrowsException<ArgumentException>(() => Game.Create(new Size(10, 0), 3));
            Assert.ThrowsException<ArgumentException>(() => Game.Create(new Size(256, 10), 3));
            Assert.ThrowsException<ArgumentException>(() => Game.Create(new Size(10, 256), 3));
        }

        [TestMethod]
        public void Create_Invalid_Supersaturated()
        {
            // There are more mines than positions in the field

            Assert.ThrowsException<ArgumentException>(() => Game.Create(new Size(5, 5), 25));
            Assert.ThrowsException<ArgumentException>(() => Game.Create(new Size(255, 255), ushort.MaxValue));
        }

        [TestMethod]
        public void GetWarningCount()
        {
            // TODO: Make this private, test thru Move()

            var game = new Game(new Size(5, 5), new[] { new Point(0, 0), new Point(3, 1), new Point(4, 1), new Point(4, 2), new Point(3, 3), new Point(4, 3) });
            /* 
             *  |01234
             * -+-----
             * 0|*1122
             * 1|111**
             * 2|0025*
             * 3|001**
             * 4|00122
             */

            Assert.AreEqual(1, game.GetWarningCount(new Point(1, 0)));
            Assert.AreEqual(1, game.GetWarningCount(new Point(2, 0)));
            Assert.AreEqual(2, game.GetWarningCount(new Point(3, 0)));
            Assert.AreEqual(2, game.GetWarningCount(new Point(4, 0)));
            Assert.AreEqual(1, game.GetWarningCount(new Point(0, 1)));
            Assert.AreEqual(1, game.GetWarningCount(new Point(1, 1)));
            Assert.AreEqual(1, game.GetWarningCount(new Point(2, 1)));
            Assert.AreEqual(0, game.GetWarningCount(new Point(0, 2)));
            Assert.AreEqual(0, game.GetWarningCount(new Point(1, 2)));
            Assert.AreEqual(2, game.GetWarningCount(new Point(2, 2)));
            Assert.AreEqual(5, game.GetWarningCount(new Point(3, 2)));
            Assert.AreEqual(0, game.GetWarningCount(new Point(0, 3)));
            Assert.AreEqual(0, game.GetWarningCount(new Point(1, 3)));
            Assert.AreEqual(1, game.GetWarningCount(new Point(2, 3)));
            Assert.AreEqual(0, game.GetWarningCount(new Point(0, 4)));
            Assert.AreEqual(0, game.GetWarningCount(new Point(1, 4)));
            Assert.AreEqual(1, game.GetWarningCount(new Point(2, 4)));
            Assert.AreEqual(2, game.GetWarningCount(new Point(3, 4)));
            Assert.AreEqual(2, game.GetWarningCount(new Point(4, 4)));
        }

        [TestMethod]
        public void Move_IntoMine()
        {
            var game = new Game(new Size(5, 5), new[] { new Point(0, 0), new Point(3, 1), new Point(4, 1), new Point(4, 2), new Point(3, 3), new Point(4, 3) });
            /* 
             *  |01234
             * -+-----
             * 0|*....
             * 1|...X*
             * 2|....*
             * 3|...**
             * 4|.....
             */

            game.Move(new Point(3, 1));

            CollectionAssert.AreEqual(new[] { new Point(3, 1) }, game.Moves.ToArray(), "Stored the move");
            Assert.AreEqual(GameResult.Lose, game.Result);
            CollectionAssert.AreEquivalent(game.Mines.ToArray(), game.Uncovered.Keys.ToArray(), "All the mines were uncovered");
            Assert.IsTrue(game.Uncovered.Keys.All(pos => game.Uncovered[pos] == Game.Mine), "All the mines were uncovered");

        }

        [TestMethod]
        public void Move_IntoClear()
        {
            var game = new Game(new Size(5, 5), new[] { new Point(0, 0), new Point(3, 1), new Point(4, 1), new Point(4, 2), new Point(3, 3), new Point(4, 3) });
            /* 
             *  |01234
             * -+-----
             * 0|*..X.
             * 1|...**
             * 2|....*
             * 3|...**
             * 4|.....
             */

            game.Move(new Point(3, 0));

            CollectionAssert.AreEqual(new[] { new Point(3, 0) }, game.Moves.ToArray(), "Stored the move");
            Assert.IsNull(game.Result);
            CollectionAssert.AreEquivalent(new[] { new Point(3, 0) }, game.Uncovered.Keys.ToArray(), "The position was uncovered");
           
        }

        [TestMethod]
        public void Move_IntoClearSpan()
        {
            var game = new Game(new Size(5, 5), new[] { new Point(0, 0), new Point(3, 1), new Point(4, 1), new Point(4, 2), new Point(3, 3), new Point(4, 3) });
            /* 
             *  |01234
             * -+-----
             * 0|*....
             * 1|11.**
             * 2|0X2.*
             * 3|001**
             * 4|001..
             */

            game.Move(new Point(1, 2));

            CollectionAssert.AreEqual(new[] { new Point(1, 2) }, game.Moves.ToArray(), "Stored the move");
            Assert.IsNull(game.Result);
            CollectionAssert.AreEquivalent(new[] {
                new Point(0, 1), new Point(1, 1),
                new Point(0, 2), new Point(1, 2), new Point(2, 2),
                new Point(0, 3), new Point(1, 3), new Point(2, 3),
                new Point(0, 4), new Point(1, 4), new Point(2, 4)
            }, game.Uncovered.Keys.ToArray(), "The span was uncovered");

        }

        [TestMethod]
        public void Move_WinGame()
        {
            var game = new Game(new Size(5, 5), new[] { new Point(0, 0), new Point(3, 1), new Point(4, 1), new Point(4, 2), new Point(3, 3), new Point(4, 3) });
            /* 
             *  |01234
             * -+-----
             * 0|*....
             * 1|...**
             * 2|....*
             * 3|...**
             * 4|.....
             */

            var moves = new[] { new Point(0, 2), new Point(1, 0), new Point(2, 0), new Point(2, 1), new Point(3, 0), new Point(4, 0), new Point(3, 2), new Point(3, 4), new Point(4, 4) };
            foreach(var move in moves)
            {
                game.Move(move);
            }
            CollectionAssert.AreEqual(moves, game.Moves.ToArray());
            Assert.AreEqual(GameResult.Win, game.Result);

        }

        [TestMethod]
        public void Move_AfterGameOver()
        {
            var game = new Game(new Size(5, 5), new[] { new Point(0, 0), new Point(3, 1), new Point(4, 1), new Point(4, 2), new Point(3, 3), new Point(4, 3) });
            /* 
             *  |01234
             * -+-----
             * 0|*....
             * 1|...X*
             * 2|....*
             * 3|...**
             * 4|.....
             */

            game.Move(new Point(3, 1));
            Assert.IsNotNull(game.Result);

            Assert.ThrowsException<InvalidOperationException>(() => game.Move(new Point(0, 1)));

        }

        [TestMethod]
        public void Move_IntoUncovered()
        {
            var game = new Game(new Size(5, 5), new[] { new Point(0, 0), new Point(3, 1), new Point(4, 1), new Point(4, 2), new Point(3, 3), new Point(4, 3) });
            /* 
             *  |01234
             * -+-----
             * 0|*....
             * 1|11.**
             * 2|0X2.*
             * 3|001**
             * 4|001..
             */

            game.Move(new Point(1, 2)); // This uncovers a span
            CollectionAssert.Contains(game.Uncovered.Keys.ToArray(), new Point(0, 2));  // We already tested this
            Assert.ThrowsException<InvalidOperationException>(() => game.Move(new Point(0, 2)));

        }
    }
}
