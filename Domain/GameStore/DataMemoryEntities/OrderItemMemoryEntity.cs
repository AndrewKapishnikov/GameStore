﻿using System;

namespace GameStore
{
    public class OrderItemMemoryEntity
    {
        private int count;
        public int GameId { get; }
        public decimal Price { get; }

        public int Count
        {
            get { return count; }
            set
            {
                ThrowIfInvalidCount(value);
                count = value;
            }
        }
        public OrderItemMemoryEntity(int gameId, int count, decimal price)
        {
            ThrowIfInvalidCount(count);
            GameId = gameId;
            Count = count;
            Price = price;
        }
        private static void ThrowIfInvalidCount(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException("The quantity must not be less than one!");
        }
    }
}
