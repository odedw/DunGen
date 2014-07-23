﻿using System;
using System.Collections.Generic;
using System.Linq;
using DunGen.Engine.Contracts;
using DunGen.Engine.Models;

namespace DunGen.Engine.Implementations
{
    public class Randomizer : IRandomizer
    {
        private readonly Random mRandom = new Random();
        public Cell GetRandomCell(Map map)
        {
            if (map.Height == 0 || map.Width == 0) return null;
            return map.GetCell(mRandom.Next(map.Width), mRandom.Next(map.Height));
        }

        public T GetRandomEnumValue<T>(IEnumerable<T> excluded = null)
        {
            excluded = excluded ?? new List<T>();
            var values = Enum.GetValues(typeof(T)).OfType<T>().Except(excluded).ToList();
            return values[mRandom.Next(values.Count)];
        }

        public T GetRandomItem<T>(IEnumerable<T> collection, IEnumerable<T> excluded = null) 
        {
            excluded = excluded ?? new List<T>();
            var cleanCollection = collection.Except(excluded).ToList();
            return !cleanCollection.Any() ? default(T) : cleanCollection[(mRandom.Next(cleanCollection.Count))];
        }

        public double GetRandomDouble()
        {
            return mRandom.NextDouble();
        }
    }
}