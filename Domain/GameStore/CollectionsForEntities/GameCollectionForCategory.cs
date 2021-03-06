﻿using GameStore.DataEF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GameStore
{
    public class GameCollectionForCategory: IReadOnlyCollection<Game>
    {
        private readonly IReadOnlyCollection<Game> items;

        public GameCollectionForCategory(IEnumerable<GameDTO> gameDto)
        {
            if (gameDto == null)
                throw new ArgumentNullException(nameof(gameDto));

            items = gameDto.Select(Game.Mapper.Map).ToList();
        }

        public int Count => items.Count;

        public IEnumerator<Game> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (items as IEnumerable).GetEnumerator();
        }
    }
}
