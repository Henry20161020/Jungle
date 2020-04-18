using System;
using System.Collections.Generic;
using System.Text;

namespace JungleLibrary
{
    public class Move
    {
        public Square StartSquare { get; set; }

        public Square EndSquare { get; set; }

        public Move(Square start, Square end)
        {
            StartSquare = start;
            EndSquare = end;
        }

        public override string ToString()
        {
            return $"{StartSquare.Row},{StartSquare.Col},{EndSquare.Row},{EndSquare.Col}";
        }

        public static Move Parse(string moveAsString)
        {
            string[] indexList = moveAsString.Split(',');
            return new Move(new Square(int.Parse(indexList[0]),int.Parse(indexList[1])), new Square(int.Parse(indexList[2]), int.Parse(indexList[3])));
        }
    }
}
