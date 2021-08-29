using System;
using System.Collections.Generic;

namespace Negamax_Framework
{
    /// <summary>
    /// A class to repersent a move in game
    /// </summary>
    class Move
    {

        private Dictionary<String, Object> moveData;

        /// <summary>
        /// A constructor to create a new move
        /// </summary>
        public Move()
        {
            this.moveData = new Dictionary<string, object>();
        }

        /// <summary>
        /// A method to add data to the move
        /// </summary>
        /// <typeparam name="T"> the type of the data to be added </typeparam>
        /// <param name="key"> the key for the data </param>
        /// <param name="data"> the data to be added </param>
        public void addData<T>(String key, T data)
        {
            this.moveData.Add(key, data);
        }

        /// <summary>
        /// A method to get data about the move
        /// </summary>
        /// <typeparam name="T"> the type of data were trying to get </typeparam>
        /// <param name="key"> the key of the data were trying to get </param>
        /// <returns> the data corresponding to the key, if no data for that key return null </returns>
        public T getData<T>(String key)
        {
            Object output;
            return this.moveData.TryGetValue(key, out output) ? (T)output : default(T);
        }

    }
}
