using System.Collections.Generic;

namespace Negamax_Framework
{
    /// <summary>
    /// A interface that a game needes to implement so a minimax agent could play it
    /// </summary>
    interface MinimaxPlayable
    {

        /// <summary>
        /// A method to evaluate the current state of the game
        /// </summary>
        /// <returns> a numbric value represening the current state of the game </returns>
        double evaluate();

        /// <summary>
        /// A method to preform a one way hash on the current game state
        /// </summary>
        /// <returns> a hash on the current game state </returns>
        int hashBoardState();

        /// <summary>
        /// A method to return all possiable moves from the current game states
        /// </summary>
        /// <returns> a list of all possiable moves from the current game state </returns>
        List<Move> generateAllMoves();

        /// <summary>
        /// A method to prefrom a given move
        /// </summary>
        /// <param name="move"> the move to be preformed </param>
        void makeMove(Move move);

        /// <summary>
        /// A method to undo a given move
        /// </summary>
        /// <param name="move"> the move to be undone </param>
        void unmakeMove(Move move);

    }
}
