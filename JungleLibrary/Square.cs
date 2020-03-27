using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace JungleLibrary
{
    public class Square
    {
        private string _name;
        private string _imageFile;
        private Piece _piece;

        public string Name { get; set; }
        public string ImageFile { get; set; }
        public Piece Piece { get; set; }

        public Square()
        {
            Name = "battle ground";
            ImageFile = "grass";
            Piece = null;
        }
        public Square(string name, string fileName, Piece piece)
        {
            Name = name;
            ImageFile = fileName;
            Piece = piece;
        }
    }
}
