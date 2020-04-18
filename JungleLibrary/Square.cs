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

        public int Row { get; set; }
        public int Col { get; set; }

        public string Name { get; set; }
        public string ImageFile
        {
            get => Name;
        }
        public Piece Piece { get; set; }

        public Square(int row, int col)
        {
            Name = "grass";
            Piece = null;
            Row = row;
            Col = col;
        }
        public Square(string name, Piece piece, int row, int col)
        {
            Name = name;
            Piece = piece;
            Row = row;
            Col = col;
        }
    }
}
