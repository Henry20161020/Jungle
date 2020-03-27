using System;
using System.Collections.Generic;
using System.Text;

namespace JungleLibrary
{
    public class Piece
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


    }
}
