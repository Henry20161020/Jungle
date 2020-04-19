using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace JungleLibrary
{
    /// <summary>
    /// This class represents a square in the game
    /// </summary>
    public class Square
    {
        public int Row { get; set; }        // The row number of the square
        public int Col { get; set; }        // The column number of the square
        public string Name { get; set; }    // Square name
        public string ImageFile             // Square image file name, the same as square name
        {
            get => Name;
        }
        public Piece Piece { get; set; }    // The piece on the square

        /// <summary>
        /// The constructor will create a default grass square and there's no animal piece on the square
        /// </summary>
        /// <param name="row">the row number of the square</param>
        /// <param name="col">the column number of the square</param>
        public Square(int row, int col)
        {
            Name = "grass";
            Piece = null;
            Row = row;
            Col = col;
        }

        /// <summary>
        /// The constructor will create a square with all parameters provided
        /// </summary>
        /// <param name="name">square name</param>
        /// <param name="piece">the animal piece on the square</param>
        /// <param name="row">the row number of the square</param>
        /// <param name="col">the column number of the square</param>
        public Square(string name, Piece piece, int row, int col)
        {
            Name = name;
            Piece = piece;
            Row = row;
            Col = col;
        }
    }
}
