using System;
using System.Collections.Generic;
using System.Text;

namespace JungleLibrary
{
    /// <summary>
    /// This class represent a move in the game
    /// </summary>
    public class Move
    {
        public Square StartSquare { get; set; } // The start square of the move
        public Square EndSquare { get; set; }   // The end square of the move

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="start">start square</param>
        /// <param name="end">end square</param>
        public Move(Square start, Square end)
        {
            StartSquare = start;
            EndSquare = end;
        }

        /// <summary>
        /// Overriden method for writing the move into a file
        /// </summary>
        /// <returns>a string containing the key move info</returns>
        public override string ToString()
        {
            return $"{StartSquare.Row},{StartSquare.Col},{EndSquare.Row},{EndSquare.Col}";
        }

        /// <summary>
        /// The method will convert a string into a move with correct location info, but wrong square name and piece
        /// </summary>
        /// <param name="moveAsString">a string read from game data file</param>
        /// <returns>a move with correct location info, but wrong square name and piece</returns>
        public static Move Parse(string moveAsString)
        {
            string[] indexList = moveAsString.Split(',');
            return new Move(new Square(int.Parse(indexList[0]),int.Parse(indexList[1])), new Square(int.Parse(indexList[2]), int.Parse(indexList[3])));
        }
    }
}
