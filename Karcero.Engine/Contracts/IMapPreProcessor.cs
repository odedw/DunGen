﻿using Karcero.Engine.Models;

namespace Karcero.Engine.Contracts
{
    internal interface IMapPreProcessor<T> where T : class, IBinaryCell, new()
    {
        void ProcessMap(Map<T> map, DungeonConfiguration configuration, IRandomizer randomizer);

    }
}
