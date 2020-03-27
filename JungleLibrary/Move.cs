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
    }
}
