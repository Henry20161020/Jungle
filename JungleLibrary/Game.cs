using System;
using System.Data;

namespace JungleLibrary
{
    public class Game
    {
        private const int HEIGHT = 9;
        private const int WIDTH = 7;
        private Square[,] _board = new Square[HEIGHT, WIDTH];

        public Game()
        {
            for (int row=0; row<HEIGHT; row++)
            for (int col = 0; col < WIDTH; col++)
            {
                _board[row,col]=new Square();
            }

            _board[0, 3].ImageFile = "den";
            _board[8, 3].ImageFile = "den";
            _board[0, 2].ImageFile = "trap";
            _board[0, 4].ImageFile = "trap";
            _board[1, 3].ImageFile = "trap";
            _board[8, 2].ImageFile = "trap";
            _board[8, 4].ImageFile = "trap";
            _board[7, 3].ImageFile = "trap";
            for (int row = 3; row < 6; row++)
            {
                _board[row, 1].ImageFile = "water";
                _board[row, 2].ImageFile = "water";
                _board[row, 4].ImageFile = "water";
                _board[row, 5].ImageFile = "water";
            }

            _board[0, 0].Piece = new Piece("Lion", "lion", "red");
            _board[0, 6].Piece = new Piece("Tiger", "tiger", "red");
            _board[1, 1].Piece = new Piece("Dog", "dog", "red");
            _board[1, 5].Piece = new Piece("Cat", "cat", "red");
            _board[2, 0].Piece = new Piece("Rat", "rat", "red");
            _board[2, 2].Piece = new Piece("Leopard", "leopard", "red");
            _board[2, 4].Piece = new Piece("Wolf", "wolf", "red");
            _board[2, 6].Piece = new Piece("Elephant", "elephant", "red");

            _board[8, 6].Piece = new Piece("Lion", "lion", "blue");
            _board[8, 0].Piece = new Piece("Tiger", "tiger", "blue");
            _board[7, 5].Piece = new Piece("Dog", "dog", "blue");
            _board[7, 1].Piece = new Piece("Cat", "cat", "blue");
            _board[6, 6].Piece = new Piece("Rat", "rat", "blue");
            _board[6, 4].Piece = new Piece("Leopard", "leopard", "blue");
            _board[6, 2].Piece = new Piece("Wolf", "wolf", "blue");
            _board[6, 0].Piece = new Piece("Elephant", "elephant", "blue");


        }

        public Square GetSquare(int row, int column)
        {
            return _board[row, column];
        }



    }
}
