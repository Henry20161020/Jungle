using System;
using System.Collections.Generic;
using System.Text;

namespace JungleLibrary
{
    /// <summary>
    /// This class reprensents a piece in the game
    /// </summary>
    public class Piece : IComparable<Piece>
    {
        public string Name { get; set; }        // Piece name
        public string ImageFile { get; set; }   // Piece image file name
        public string Color { get; set; }       // Piece color

        /// <summary>
        /// The constructor to create a piece
        /// </summary>
        /// <param name="name">piece name</param>
        /// <param name="fileName">piece image file name</param>
        /// <param name="color">piece color</param>
        public Piece(string name, string fileName, string color)
        {
            Name = name;
            ImageFile = fileName;
            Color = color;
        }

        /// <summary>
        /// Compare the power of two pieces
        /// </summary>
        /// <param name="otherPiece">the other piece</param>
        /// <returns>1 for win, -1 for lose</returns>
        public int CompareTo(Piece otherPiece)
        {
            // Higher integer means higher power, but rat can win elephant
            Dictionary<string, int> power = new Dictionary<string, int>
            {
                {"Rat",0},
                {"Cat",1},
                {"Dog",2},
                {"Wolf",3},
                {"Leopard",4},
                {"Tiger",5},
                {"Lion",6},
                {"Elephant",7}
            };
            int powerDifference = power[this.Name] - power[otherPiece.Name];
            if ((powerDifference >= 0 && powerDifference <= 6) || powerDifference == -7)
                return 1;
            else
                return -1;
        }
    }
}
