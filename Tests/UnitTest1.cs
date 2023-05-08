using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using cli_life;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var cell = new Cell { IsAlive = true };
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.DetermineNextLiveState();
            Assert.IsFalse(cell.IsAliveNext);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var cell = new Cell { IsAlive = true };
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.DetermineNextLiveState();
            Assert.IsTrue(cell.IsAliveNext);
        }

        [TestMethod]
        public void TestMethod3()
        {
            var cell = new Cell { IsAlive = true };
            cell.neighbors.Add(new Cell { IsAlive = false });
            cell.DetermineNextLiveState();
            Assert.IsFalse(cell.IsAliveNext);
        }
    }

    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            var board = new Board(50, 20, 1);
            var count = board.TotalCells;
            Assert.AreEqual(1000, count);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var board = new Board(50, 20, 1);
            for (int x = 0; x < 50; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    board.Cells[x, y].IsAlive = true;
                }
            }
            var count = board.GetAliveCellsCount();
            Assert.AreEqual(1000, count);
        }
    }
}
