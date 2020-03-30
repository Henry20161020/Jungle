using System;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;

namespace JungleLibrary
{
    public class Game
    {
        private const int HEIGHT = 9;
        private const int WIDTH = 7;
        private Square[,] _board = new Square[HEIGHT, WIDTH];
        public string Status { get; set; }



        public Game()
        {
            for (int row=0; row<HEIGHT; row++)
            for (int col = 0; col < WIDTH; col++)
            {
                _board[row,col]=new Square();
            }

            _board[0, 3].Name = "den";
            _board[8, 3].Name = "den";
            _board[0, 2].Name = "trap";
            _board[0, 4].Name = "trap";
            _board[1, 3].Name = "trap";
            _board[8, 2].Name = "trap";
            _board[8, 4].Name = "trap";
            _board[7, 3].Name = "trap";
            for (int row = 3; row < 6; row++)
            {
                _board[row, 1].Name = "water";
                _board[row, 2].Name = "water";
                _board[row, 4].Name = "water";
                _board[row, 5].Name = "water";
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

            Status = "blue";


        }

        public Square GetSquare(int row, int column)
        {
            return _board[row, column];
        }

        public void MakeMove(Move move)
        {
            move.EndSquare.Piece = move.StartSquare.Piece;
            move.StartSquare.Piece = null;
            Status = Status == "red" ? "blue" : "red";

        }

        private bool isSafeDestination(Square start, Square end)
        {
            if (start.Piece.Name == "Rat")
                return true;
            if (end.Name == "water")
            {
                throw new Exception("The animal cannot swim.");
            }
            else
                return true;
        }

        private bool isRiverSide(int row, int col)
        {
            if (_board[Math.Min(8, row + 1), col].Name == "water") return true;
            if (_board[Math.Max(0, row - 1), col].Name == "water") return true;
            if (_board[row, Math.Min(6,col+1)].Name == "water") return true;
            if (_board[row, Math.Max(0, col - 1)].Name == "water") return true;
            return false;
        }



        private bool isDistanceValid(int startRow, int startCol, int endRow, int endCol)
        {
            if (Math.Abs(startRow - endRow) == 1 && Math.Abs(startCol - endCol) == 0) return true;
            if (Math.Abs(startRow - endRow) == 0 && Math.Abs(startCol - endCol) == 1) return true;
            Square startSquare = _board[startRow, startCol];
            Square endSquare = _board[endRow, endCol];
            if ((startSquare.Piece.Name == "Lion" || startSquare.Piece.Name == "Tiger") && isRiverSide(startRow, startCol) && isRiverSide(endRow,endCol)
                && ((Math.Abs(startRow - endRow) == 0 && Math.Abs(startCol - endCol) == 3) || (Math.Abs(startRow - endRow) == 4 && Math.Abs(startCol - endCol) == 0)))
            {
                if (Math.Abs(startCol - endCol) == 0)
                {
                    int step = (endRow - startRow) / Math.Abs(startRow - endRow);
                    for (int row = startRow; Math.Abs(row - endRow)>0; row += step)
                    {
                        if (_board[row, startCol].Piece?.Name == "Rat" && _board[row, startCol].Piece?.Color!=Status)
                        {
                            throw new Exception("There is Rat. The animal cannot jump.");
                        }

                    }


                    return true;
                }
                else
                {
                    int step = (endCol - startCol) / Math.Abs(startCol - endCol);
                    for (int col = startCol; Math.Abs(col - endCol)>0; col += step)
                    {
                        if (_board[startRow, col].Piece?.Name == "Rat" && _board[startRow, col].Piece?.Color != Status)
                        {
                            throw new Exception("There is Rat. The animal cannot jump.");
                        }
                    }



                    return true;
                }


            }
            throw new Exception("The animal cannot move like this.");
        }


        


        public bool IsValidMove(int startRow, int startCol, int endRow, int endCol)
        {
            Square startSquare = _board[startRow, startCol];
            Square endSquare = _board[endRow, endCol];
            if (startSquare.Piece?.Color!=Status) throw new Exception("This is not your turn");
            
            if (isSafeDestination(startSquare, endSquare) && isDistanceValid(startRow, startCol, endRow, endCol))
            {
                if (endSquare.Piece == null)
                    return true;
                if (startSquare.Piece.Color == endSquare.Piece.Color) throw new Exception("Two animals can not be in the same square");
                if (startSquare.Piece.CompareTo(endSquare.Piece) == 1)
                    return true;
                if (endSquare.Name == "trap")
                    return true;
                throw new Exception("The animal cannot win the battle.");
            }
            return false;

        }


    }
}
