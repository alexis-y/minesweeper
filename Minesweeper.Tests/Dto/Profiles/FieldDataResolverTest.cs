using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minesweeper.Dto.Profiles;
using Minesweeper.Model;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Minesweeper.Tests.Dto.Profiles
{
    [TestClass]
    public class FieldDataResolverTest
    {

        [TestMethod]
        public void Resolve_CorrectFieldSize()
        {
            // The output grid has the expected size

            var source = new Game(new Size(5, 7), Enumerable.Empty<Point>());
            var data = new Dictionary<Point, byte>();
            var resolver = new FieldDataResolver();

            var output = resolver.Resolve(source, null, data, null, null);

            Assert.IsNotNull(output);
            var lines = output.Split("\r\n");
            Assert.AreEqual(7, lines.Length);
            for (var ix = 0; ix < lines.Length; ix++)
            {
                Assert.AreEqual(5, lines[ix].Length);
                Assert.AreEqual(".....", lines[ix]);
            }
        }

        [TestMethod]
        public void Resolve_ProximityValues()
        {
            // The values for each tile is a single digit

            var source = new Game(new Size(9, 1), Enumerable.Empty<Point>());
            var data = Enumerable.Range(0, 9).ToDictionary(n => new Point(n, 0), n => (byte)n); // a single line of 0-8
            var resolver = new FieldDataResolver();

            var output = resolver.Resolve(source, null, data, null, null);

            Assert.AreEqual("012345678", output);
        }

        [TestMethod]
        public void Resolve_ExposedMine()
        {
            // Exposed mines are shown with an X

            var source = new Game(new Size(9, 1), Enumerable.Empty<Point>());
            var data = Enumerable.Range(4, 4).ToDictionary(n => new Point(n, 0), n => Game.Mine); // 4 mines
            var resolver = new FieldDataResolver();

            var output = resolver.Resolve(source, null, data, null, null);

            Assert.AreEqual("....XXXX.", output);
        }

    }
}
