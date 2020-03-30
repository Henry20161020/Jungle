using System;
using System.Collections.Generic;
using System.Text;

namespace JungleLibrary
{
    public class Piece : IComparable<Piece>
    {
        public string Name { get; set; }
        public string ImageFile { get; set; }
        public string Color { get; set; }

        public Piece(string name, string fileName, string color)
        {
            Name = name;
            ImageFile = fileName;
            Color = color;
        }

        public int CompareTo(Piece otherPiece)
        {
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
