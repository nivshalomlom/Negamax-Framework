using System;
using System.Collections.Generic;

namespace Negamax_Framework
{
    /// <summary>
    /// A implementation of a minimax agent capable of playing any game implementing the MiniMaxPlayable interface correctly
    /// </summary>
    class MinimaxAgent
    {

        // Enum for cache entry flags
        private enum flag
        {
            EXACT, 
            LOWERBOUND,
            UPPERBOUND
        }

        // The game the agent is playing
        private MinimaxPlayable game;

        // The transposition table(cache)
        private Dictionary<int, EvaluationEntry> evaluation_cache;

        /// <summary>
        /// A constructor to create a new minimax agent
        /// </summary>
        /// <param name="game"> the game the agent is going to play </param>
        public MinimaxAgent(MinimaxPlayable game)
        {
            this.game = game;
            this.evaluation_cache = new Dictionary<int, EvaluationEntry>();
        }

        /// <summary>
        /// A method to try a move and return it's shallow evaluation
        /// </summary>
        /// <param name="move"> the move to be tried </param>
        /// <returns> the shallow evaluation of the game after the move </returns>
        private double computeMoveEvaluation(Move move)
        {
            this.game.makeMove(move);
            double evaluation = this.game.evaluate();
            this.game.unmakeMove(move);
            return evaluation;
        }

        /// <summary>
        /// A method to evaluate all possiable moves and find the best one
        /// </summary>
        /// <param name="depth"> the depth limit of the search (how many recursive calls) </param>
        /// <param name="alpha"> pruning parameter, keeps the minimum score of the maximizing player </param>
        /// <param name="beta"> pruning parameter, keeps the maximum score of the minimizing player </param>
        /// <param name="prespective"> 1 if maximizing player, -1 if minimizing player </param>
        /// <returns> A EvaluationEntry containing the depth that was searched, 
        /// the result of the search, a flag to signify result accuracy and the move chosen as optimal</returns>
        private EvaluationEntry evaluatePlay(int depth, double alpha, double beta, int prespective)
        {
            double alphaBackup = alpha;

            // Hash the current board state
            int board_state_hash = this.game.hashBoardState();
            // Check the cache for a evaluation from this exact board position to
            // a depth equal or greater then this one
            EvaluationEntry cache_entry;
            if (this.evaluation_cache.TryGetValue(board_state_hash, out cache_entry) && cache_entry.depth >= depth)
            {
                // Process cache entry according to flag
                if (cache_entry.flag == flag.EXACT)
                    return cache_entry;
                else if (cache_entry.flag == flag.LOWERBOUND)
                    alpha = Math.Max(alpha, cache_entry.result);
                else if (cache_entry.flag == flag.UPPERBOUND)
                    beta = Math.Min(beta, cache_entry.result);
                // If cache entry is a prefect match return its value
                if (alpha >= beta)
                    return cache_entry;
            }

            // Check if we reached target depth or bottom of tree (no more options)
            List<Move> moves = this.game.generateAllMoves();
            if (depth == 0 || moves.Count == 0)
                return new EvaluationEntry(depth, prespective * this.game.evaluate(), flag.EXACT, null);

            // Sort all possiable moves according to shallow evaluation
            moves.Sort(delegate (Move m1, Move m2)
            {
                double m1_value = this.computeMoveEvaluation(m1);
                double m2_value = this.computeMoveEvaluation(m2);
                return m1_value.CompareTo(m2_value);
            });

            // Iterate over all possiable moves
            double best_value = Double.NegativeInfinity;
            Move best_move = null;
            foreach (Move move in moves)
            {
                // Evaluate the move
                this.game.makeMove(move);
                double result = -evaluatePlay(depth - 1, -beta, -alpha, -prespective).result;
                if (best_value < result || best_move == null)
                {
                    best_value = result;
                    best_move = move;
                }
                this.game.unmakeMove(move);

                // Check if branch can be pruned
                alpha = Math.Max(alpha, best_value);
                if (alpha >= beta)
                    break;
            }

            // Store current evaluation in cache
            EvaluationEntry entry = new EvaluationEntry(depth, best_value, flag.EXACT, best_move);
            if (best_value <= alphaBackup)
                entry.flag = flag.UPPERBOUND;
            else if (best_value >= beta)
                entry.flag = flag.LOWERBOUND;
            this.evaluation_cache.Add(this.game.hashBoardState(), entry);

            return entry;
        }

        /// <summary>
        /// A method to let the minimax agent find the play the optimal move for the given parameters
        /// </summary>
        /// <param name="depth"> the depth limit of the search (how many recursive calls) </param>
        /// <param name="prespective"> 1 if maximizing player, -1 if minimizing player </param>
        public void play(int depth, int prespective)
        {
            // Choose starting alpha and beta according to prespective
            double alpha = Double.NegativeInfinity;
            double beta = Double.PositiveInfinity;
            if (prespective == -1)
            {
                alpha = Double.PositiveInfinity;
                beta = Double.NegativeInfinity;
            }
            // Find and preform the optimal move for the current parameters
            EvaluationEntry result = this.evaluatePlay(depth, alpha, beta, prespective);
            this.game.makeMove(result.move);
        }

        /// <summary>
        /// A method to clear the search cache
        /// </summary>
        public void clearCache()
        {
            this.evaluation_cache.Clear();
        }

        /// <summary>
        /// A class to handle cache entries for our transposition table(cache)
        /// </summary>
        private struct EvaluationEntry
        {

            public int depth;
            public double result;
            public Move move;

            public flag flag;

            /// <summary>
            /// Creates a new evaluation entry
            /// </summary>
            /// <param name="depth"> the depth this evaluation took in place in </param>
            /// <param name="result"> the result of this evaluation </param>
            /// <param name="flag"> a flag to indicate if the result is the true value or the pruned value </param>
            /// <param name="move"> the move that took us to this board state </param>
            public EvaluationEntry(int depth, double result, flag flag, Move move)
            {
                this.depth = depth;
                this.result = result;
                this.flag = flag;
                this.move = move;
            }

        }

    }
}
