﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Karcero.Engine.Models;
using Karcero.Engine.Processors;
using NUnit.Framework;
using Randomizer = Karcero.Engine.Helpers.Randomizer;

namespace Karcero.Tests
{
    [TestFixture]
    public class DeadendsRemoverTests
    {
        private const int SOME_WIDTH = 30;
        private const int SOME_HEIGHT = 30;
        private int mSeed;
        private readonly Randomizer mRandomizer = new Randomizer();
        private readonly DungeonConfiguration mConfiguration = 
            new DungeonConfiguration() { Height = SOME_HEIGHT, Width = SOME_WIDTH, ChanceToRemoveDeadends = 1, Sparseness = 0.2, Randomness = 1};

        [SetUp]
        public void SetUp()
        {
            mSeed = Guid.NewGuid().GetHashCode();
            mRandomizer.SetSeed(mSeed);    
        }

        [TearDown]
        public void TearDown()
        {
            Console.WriteLine("Seed = {0}", mSeed);            
        }

        [Test]
        public void ProcessMap_RemoveAllDeadEnds_AllDeadEndsRemoved()
        {
            var map = new Map<BinaryCell>(SOME_WIDTH, SOME_HEIGHT);

            var mazeGenerator = new MazeGenerator<BinaryCell>();
            mazeGenerator.ProcessMap(map, mConfiguration, mRandomizer);
            var sparsenessReducer = new SparsenessReducer<BinaryCell>();
            sparsenessReducer.ProcessMap(map, mConfiguration, mRandomizer);

            var remover = new DeadendsRemover<BinaryCell>();
            remover.ProcessMap(map, mConfiguration, mRandomizer);

            Assert.AreEqual(0, map.AllCells.Count(cell => cell.Sides.Values.Count(type => type) == 1));
        }
    }
}
