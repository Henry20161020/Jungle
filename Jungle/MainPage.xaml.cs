using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Timers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Gaming.UI;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Jungle.Dal;
using JungleLibrary;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Jungle
{
    /// <summary>
    /// The main page and the only page to play the game
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private int _dragStartRow, _dragStartColumn, _dragEndRow, _dragEndColumn;   // To keep records of row/column numbers for a drag
        private Border[,] _boardMatrix = new Border[9, 7];                          // The matrix of borders. Each border contains an image.
        private Border _startSquare, _endSquare;                                    // To keep records of start square and end square for a drag
        private Game _game=new Game();                                              // The game object
        private List<Move> _moveList=new List<Move>();                              // A move list for loading the game with correct location info but wrong square name or pieces
        private DispatcherTimer _timer;                                             // A timer for replaying game

        /// <summary>
        /// Initialize
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initiate the board when the page is loaded
        /// </summary>
        /// <param name="sender">the object triggering the event</param>
        /// <param name="e">the event parameter</param>
        private void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            InitiateBoard();
        }

        /// <summary>
        /// Initiate the board
        /// </summary>
        private void InitiateBoard()
        {
            for (int row = 0; row < 9; row++)
            for (int col = 0; col < 7; col++)
            {
                _boardMatrix[row, col] = new Border();
                _boardMatrix[row, col].SetValue(Grid.RowProperty, row);
                _boardMatrix[row, col].SetValue(Grid.ColumnProperty, col);
                _boardMatrix[row, col].AllowDrop = true;
                _boardMatrix[row, col].DragStarting += OnDragStartHandler;
                _boardMatrix[row, col].DragOver += OnDragEnterHandler;
                _boardMatrix[row, col].DropCompleted += OnDropCompletedHandler;
                _boardMatrix[row, col].Background = new ImageBrush
                {
                    ImageSource =
                        new BitmapImage(new Uri($"ms-appx:///Assets/{_game.GetSquare(row, col).ImageFile}.jpg"))
                };
                Update(row,col);
                GrdMain.Children.Add(_boardMatrix[row, col]);
            }
        }

        /// <summary>
        /// Record the row/column numbers of the start square When the drag starts
        /// </summary>
        /// <param name="sender">the object triggering the event</param>
        /// <param name="e">the event parameter</param>
        private void OnDragStartHandler(UIElement sender, DragStartingEventArgs args)
        {
            _startSquare = sender as Border;
            _dragStartRow = Grid.GetRow(_startSquare);
            _dragStartColumn = Grid.GetColumn(_startSquare);
        }

        /// <summary>
        /// Record the row/column numbers of a potential end square When the drag enters a square
        /// </summary>
        /// <param name="sender">the object triggering the event</param>
        /// <param name="e">the event parameter</param>
        private void OnDragEnterHandler(object sender, DragEventArgs e)
        {
            if (_startSquare != null)
            {
                e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
                _endSquare = sender as Border;
                _dragEndRow = Grid.GetRow(_endSquare);
                _dragEndColumn = Grid.GetColumn(_endSquare);
            }
        }

        /// <summary>
        /// Handle the event when the drop is completed
        /// </summary>
        /// <param name="sender">the object triggering the event</param>
        /// <param name="e">the event parameter</param>
        private void OnDropCompletedHandler(UIElement sender, DropCompletedEventArgs args)
        {
            try // If it's a valid move, make move and update the squares
            {
                if (_startSquare != null && _endSquare != null && _startSquare!=_endSquare &&
                    _game.IsValidMove(_dragStartRow, _dragStartColumn, _dragEndRow, _dragEndColumn))
                {
                    _game.MakeMove(new Move(_game.GetSquare(_dragStartRow, _dragStartColumn),
                        _game.GetSquare(_dragEndRow, _dragEndColumn)));
                    Update(_dragStartRow, _dragStartColumn);
                    Update(_dragEndRow, _dragEndColumn);
                    ResetDragState();
                }
            }
            catch (Exception ex)    // Otherwise, tell the player why it's an invalid move
            {
                ShowExceptionDialog(ex);
            }
            ChechGameEnd();         // Check whether the game ends
        }

        /// <summary>
        /// Save the move list to the game data file
        /// </summary>
        /// <param name="sender">the object triggering the event</param>
        /// <param name="e">the event parameter</param>
        private void OnSaveClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                DataHandler dao = new DataHandler();
                dao.SaveGame(_game.MoveList);
            }
            catch (Exception ex)
            {
                ShowExceptionDialog(ex);
            }
        }

        /// <summary>
        /// Load or replay the game by the move list extracted from the game data file
        /// </summary>
        /// <param name="sender">the object triggering the event</param>
        /// <param name="e">the event parameter</param>
        private async void OnLoadClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                DataHandler dao = new DataHandler();
                _moveList = await dao.LoadGame();
                if (_moveList.Count > 0)
                {
                    OnNewClicked(sender, e);    // Call the new game method
                    if (sender == BtnReplay)    // For replaying game
                    {
                        enableButtons(false);   // Disable buttons to avoid new/load/save game when replaying
                        _timer = new DispatcherTimer();
                        _timer.Interval = new TimeSpan(0, 0, 1);    // 1 second per move
                        _timer.Tick += OnTimerTick;
                        _timer.Start();
                    }
                    else
                    {
                        foreach (Move move in _moveList)
                            MoveAndUpdate(move);
                        ChechGameEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowExceptionDialog(ex);
            }
        }

        /// <summary>
        /// New game
        /// </summary>
        /// <param name="sender">the object triggering the event</param>
        /// <param name="e">the event parameter</param>
        private void OnNewClicked(object sender, RoutedEventArgs e)
        {
            _game.Reset();
            for (int row = 0; row < 9; row++)
            for (int col = 0; col < 7; col++)
                Update(row, col);
            TxtPlayer.Text = "Current Player";
        }

        /// <summary>
        /// Reset drag state after each drag
        /// </summary>
        private void ResetDragState()
        {
            _startSquare = null;
            _endSquare = null;
            _dragStartRow = -1;
            _dragStartColumn = -1;
            _dragEndRow = -1;
            _dragEndColumn = -1;
        }

        /// <summary>
        /// Update square
        /// </summary>
        /// <param name="row">the row number of the square</param>
        /// <param name="col">the column number of the square</param>
        private void Update(int row, int col)
        {
            Piece piece= _game.GetSquare(row, col).Piece;
            if (_game.GetSquare(row, col).Piece == null)
            {
                _boardMatrix[row, col].Child = null;
                _boardMatrix[row, col].BorderBrush = new SolidColorBrush(Colors.Black);
                _boardMatrix[row, col].BorderThickness = new Thickness(1) ;
                _boardMatrix[row, col].CanDrag = false;     // The square without a piece cannot be dragged
            }
            else
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri($"ms-appx:///Assets/{piece.ImageFile}.jpg"));
                _boardMatrix[row, col].Child = img;
                _boardMatrix[row, col].BorderThickness = new Thickness(5);
                _boardMatrix[row, col].CanDrag = true;
                if (piece.Color == "red")
                    _boardMatrix[row, col].BorderBrush = new SolidColorBrush(Colors.Red);
                else
                    _boardMatrix[row, col].BorderBrush = new SolidColorBrush(Colors.Blue);
            }
        }

        /// <summary>
        /// Show exception dialog
        /// </summary>
        /// <param name="ex"></param>
        private async void ShowExceptionDialog(Exception ex)
        {
            MessageDialog dialog = new MessageDialog(ex.Message);
            await dialog.ShowAsync();
        }

        /// <summary>
        /// For each timer click, make one move and update squares
        /// </summary>
        /// <param name="sender">the object triggering the event</param>
        /// <param name="e">the event parameter</param>
        private void OnTimerTick(object sender, object e)
        {
            if (_game.CurrentMove < _moveList.Count)
                MoveAndUpdate(_moveList[_game.CurrentMove++]);
            else
            {
                _timer.Stop();
                ChechGameEnd();
                enableButtons(true);
            }
        }

        /// <summary>
        /// Make one move and update squares
        /// </summary>
        /// <param name="move">the move</param>
        private void MoveAndUpdate(Move move)
        {
            _game.MakeMove(_game.UpdateSquare(move));   // Update the correct square info and make move
            Update(move.StartSquare.Row, move.StartSquare.Col);
            Update(move.EndSquare.Row, move.EndSquare.Col);
        }

        /// <summary>
        /// Enable or disable three buttons
        /// </summary>
        /// <param name="enabled">true for enable, false for disable</param>
        private void enableButtons(bool enabled)
        {
            BtnNew.IsEnabled = enabled;
            BtnSave.IsEnabled = enabled;
            BtnLoad.IsEnabled = enabled;
        }

        /// <summary>
        /// Check whether the game ends. 
        /// </summary>
        private void ChechGameEnd()
        {
            if (_game.isGameEnded()) // If the game ends, no square can be dragged anymore
            {
                TxtPlayer.Text = "Game Over";
                for (int row = 0; row < 9; row++)
                for (int col = 0; col < 7; col++)
                    _boardMatrix[row, col].CanDrag = false;
            }
        }



    }
}
