using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;

namespace JungleLibrary
{
    /// <summary>
    /// This class controls the game rules
    /// </summary>
    public class Game : INotifyPropertyChanged
    {
        
        private const int HEIGHT = 9;   // 9 rows
        private const int WIDTH = 7;    // 7 columns
        private Square[,] _board = new Square[HEIGHT, WIDTH];   // A matrix of squares of the game, organized in 9 rows and 7 columns
        private int[] _pieceCount=new int[2];    // The array to count the pieces of two players. The first element is to count blue and the second element is to count red.
        private string _status;         // Maintains the game status such as who's turn or who wins

        /// <summary>
        /// The Status property change will trigger an event for UI layer to update the binding UI
        /// </summary>
        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status"));
                }
            }
        }
        public List<Move> MoveList { get; set; }    // Contains the list of moves from the game start
        public int CurrentMove { get; set; }        // The index of the current move, used for replaying games.
        public event PropertyChangedEventHandler PropertyChanged;   // For triggering property change event
        
        /// <summary>
        /// Game constructor. The Reset method is also used to new a game
        /// </summary>
        public Game()
        {
            Reset();
        }

        /// <summary>
        /// Return a square object by its row number and column number in the matrix
        /// </summary>
        /// <param name="row">the row number of the square</param>
        /// <param name="column">the column number of the square</param>
        /// <returns>a square object</returns>
        public Square GetSquare(int row, int column)
        {
            return _board[row, column];
        }

        /// <summary>
        /// Make a valid move
        /// </summary>
        /// <param name="move">a validated move</param>
        public void MakeMove(Move move)
        {
            Piece piece = move.EndSquare.Piece;
            // When an animal piece is defeated, the corresponding piece count decrement.
            if (piece!=null)
                if (piece.Color == "blue")
                    _pieceCount[0]--;
                else
                    _pieceCount[1]--;
            // The moving piece moves from start square to end square
            move.EndSquare.Piece = move.StartSquare.Piece;
            move.StartSquare.Piece = null;
            // Change player
            Status = Status == "red" ? "blue" : "red";
            // Add move into the move list
            MoveList.Add(move);
        }

        /// <summary>
        /// Judge whether the destination square is safe for the moving animal
        /// </summary>
        /// <param name="start">the start square</param>
        /// <param name="end">the end square</param>
        /// <returns>true for safe, false for unsafe</returns>
        private bool IsSafeDestination(Square start, Square end)
        {
            if (start.Piece.Name == "Rat")  // Rat can go anywhere
                return true;
            if (end.Name == "water")        // No animals can enter water except rat
                throw new Exception("The animal cannot swim.");
            else
                return true;
        }

        /// <summary>
        /// Judge whether a square is adjacent to river (water)
        /// </summary>
        /// <param name="row">the row number of the square</param>
        /// <param name="col">the column number of the square</param>
        /// <returns>true for adjacent to river, false for not adjacent to river</returns>
        private bool IsRiverSide(int row, int col)
        {
            // Check the north, south, east and west square whether it's water square
            // Row 0, row 8, column 0 and column 6 are not water tile, so the algorithem can be simplified as below
            if (_board[Math.Min(8, row + 1), col].Name == "water") return true;
            if (_board[Math.Max(0, row - 1), col].Name == "water") return true;
            if (_board[row, Math.Min(6,col+1)].Name == "water") return true;
            if (_board[row, Math.Max(0, col - 1)].Name == "water") return true;
            return false;
        }

        /// <summary>
        /// Judge whether the move distance is valid
        /// </summary>
        /// <param name="startRow">the row number of start square</param>
        /// <param name="startCol">the column number of start square</param>
        /// <param name="endRow">the row number of end square</param>
        /// <param name="endCol">the column number of end square</param>
        /// <returns>true for valid distance, false for invalid distance</returns>
        private bool IsDistanceValid(int startRow, int startCol, int endRow, int endCol)
        {
            // Moving 1 square vertically is valid for all animals
            if (Math.Abs(startRow - endRow) == 1 && Math.Abs(startCol - endCol) == 0) return true;
            // Moving 1 square horizontally is valid for all animals
            if (Math.Abs(startRow - endRow) == 0 && Math.Abs(startCol - endCol) == 1) return true;
            Square startSquare = _board[startRow, startCol];
            Square endSquare = _board[endRow, endCol];
            // For lion and tiger, they can jump over the river (water)
            if ((startSquare.Piece.Name == "Lion" || startSquare.Piece.Name == "Tiger") && IsRiverSide(startRow, startCol) && IsRiverSide(endRow,endCol)
                && ((Math.Abs(startRow - endRow) == 0 && Math.Abs(startCol - endCol) == 3) || (Math.Abs(startRow - endRow) == 4 && Math.Abs(startCol - endCol) == 0)))
            {
                if (Math.Abs(startCol - endCol) == 0)   // Jump horizontally
                {
                    // Check whether there is rat in the way. Lions and tigers can't jump if there is rat in the jumping path.
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
                else    // Jump vertically
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
            // All other moves are invalid
            throw new Exception("The animal cannot move like this.");
        }

        /// <summary>
        /// Judge whether a move is valid
        /// </summary>
        /// <param name="startRow">the row number of start square</param>
        /// <param name="startCol">the column number of start square</param>
        /// <param name="endRow">the row number of end square</param>
        /// <param name="endCol">the column number of end square</param>
        /// <returns></returns>
        public bool IsValidMove(int startRow, int startCol, int endRow, int endCol)
        {
            Square startSquare = _board[startRow, startCol];
            Square endSquare = _board[endRow, endCol];
            // Player cannot move the opponent piece
            if (startSquare.Piece?.Color!=Status) throw new Exception("This is not your turn");
            if (IsSafeDestination(startSquare, endSquare) && IsDistanceValid(startRow, startCol, endRow, endCol))
            {
                // No animal is in the destination square 
                if (endSquare.Piece == null)
                    return true;
                // Player cannot move one animal to a square occupied by its teammate animal
                if (startSquare.Piece.Color == endSquare.Piece.Color) throw new Exception("Two animals can not be in the same square");
                // Animal with higher power can win the battle and eat the defeated animal
                if (startSquare.Piece.CompareTo(endSquare.Piece) == 1)
                    return true;
                // When an animal is in the trap, any animal can eat it.
                if (endSquare.Name == "trap")
                    return true;
                throw new Exception("The animal cannot win the battle.");
            }
            return false;
        }

        /// <summary>
        /// Reset the game
        /// </summary>
        public void Reset()
        {
            // Initial all squares
            for (int row = 0; row < HEIGHT; row++)
            for (int col = 0; col < WIDTH; col++)
            {
                _board[row, col] = new Square(row, col);
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

            // Inital all pieces
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

            // Initial game settings
            Status = "blue";
            MoveList = new List<Move>();
            CurrentMove = 0;
            _pieceCount[0] = 8;
            _pieceCount[1] = 8;
        }

        /// <summary>
        /// Update the squares for a move. This may happen when a move list is loaded from a file with limited information.
        /// </summary>
        /// <param name="move">one move with correct square row/column number but wrong square name or piece</param>
        /// <returns>a move with all correct attributes</returns>
        public Move UpdateSquare(Move move)
        {
            int startRow = move.StartSquare.Row;
            int startCol = move.StartSquare.Col;
            int endRow = move.EndSquare.Row;
            int endCol = move.EndSquare.Col;

            // Update move
            move.StartSquare.Name = _board[startRow, startCol].Name;
            move.StartSquare.Piece = _board[startRow, startCol].Piece;
            move.EndSquare.Name = _board[endRow, endCol].Name;
            move.EndSquare.Piece = _board[endRow, endCol].Piece;

            // Update the board accordingly
            _board[startRow, startCol] = move.StartSquare;
            _board[endRow, endCol] = move.EndSquare;

            return move;
        }

        /// <summary>
        /// Judge whether the game ends
        /// </summary>
        /// <returns>true for game end, false for not</returns>
        public bool isGameEnded()
        {
            // If an animal conquer the opponent's den, its team wins
            Piece denPiece = _board[0, 3].Piece;
            if (denPiece!=null)
                if (denPiece.Color == "blue")
                {
                    Status = "Blue win";
                    return true;
                }
            denPiece = _board[8, 3].Piece;
            if (denPiece != null)
                if (denPiece.Color == "red")
                {
                    Status = "Red win";
                    return true;
                }

            // If all pieces of a player are defeated, the player loses
            if (_pieceCount[0] == 0)
            {
                Status = "Red win";
                return true;
            }
            if (_pieceCount[1] == 0)
            {
                Status = "Blue win";
                return true;
            }

            return false;
        }
    }
}
