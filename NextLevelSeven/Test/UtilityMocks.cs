﻿using System;
using NextLevelSeven.Utility;

namespace NextLevelSeven.Test
{
    /// <summary>
    ///     Contains mock classes for the Utility namespace for use in testing.
    /// </summary>
    public static class UtilityMocks
    {
        /// <summary>
        ///     Create an indexed cache, using the specified factory to lazily create values that don't already exist.
        /// </summary>
        /// <typeparam name="TKey">Type of key.</typeparam>
        /// <typeparam name="TValue">Type of value.</typeparam>
        /// <param name="factoryMethod">Method that will create new values.</param>
        /// <returns>Indexed cache of the specified types and the specified factory.</returns>
        public static IIndexedCache<TKey, TValue> GetIndexedCache<TKey, TValue>(Func<TKey, TValue> factoryMethod)
            where TValue : class
        {
            return new IndexedCache<TKey, TValue>(new ProxyFactory<TKey, TValue>(factoryMethod));
        }
    }
}