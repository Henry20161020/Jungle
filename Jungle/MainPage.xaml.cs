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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private int _dragStartRow, _dragStartColumn, _dragEndRow, _dragEndColumn;
        private Border[,] boardMatrix = new Border[9, 7];
        private Border _startSquare, _endSquare;
        private Game _game=new Game();
        private List<Move> _moveList=new List<Move>();
        private DispatcherTimer _timer;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void InitiateBoard()
        {
            

            for (int row = 0; row < 9; row++)
            for (int col = 0; col < 7; col++)
            {
                boardMatrix[row, col] = new Border();
                boardMatrix[row, col].SetValue(Grid.RowProperty, row);
                boardMatrix[row, col].SetValue(Grid.ColumnProperty, col);
                boardMatrix[row, col].AllowDrop = true;
                
                boardMatrix[row, col].DragStarting += OnDragStartHandler;
                boardMatrix[row, col].DragOver += OnDragEnterHandler;
                boardMatrix[row, col].DropCompleted += OnDropCompletedHandler;

                boardMatrix[row, col].Background = new ImageBrush
                {
                    ImageSource =
                        new BitmapImage(new Uri($"ms-appx:///Assets/{_game.GetSquare(row, col).ImageFile}.jpg"))
                };

                Update(row,col);
                GrdMain.Children.Add(boardMatrix[row, col]);
            }

            

        }



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

        private void OnDropCompletedHandler(UIElement sender, DropCompletedEventArgs args)
        {
            try
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
            catch (Exception ex)
            {
                ShowExceptionDialog(ex);
            }

            ChechGameEnd();


        }

        private void ChechGameEnd()
        {
            if (_game.isGameEnded())
            {
                TxtPlayer.Text = "Game Over";
                for (int row = 0; row < 9; row++)
                for (int col = 0; col < 7; col++)
                    boardMatrix[row, col].CanDrag = false;
            }
        }


        private void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            InitiateBoard();
        }



        private void OnSaveClicked(object sender, RoutedEventArgs e)
        {

            DataHandler dao= new DataHandler();
            dao.SaveGame(_game.MoveList);


            
        }

        private void OnDragStartHandler(UIElement sender, DragStartingEventArgs args)
        {
            _startSquare = sender as Border;
            _dragStartRow = Grid.GetRow(_startSquare);
            _dragStartColumn = Grid.GetColumn(_startSquare);
        }

        private async void OnLoadClicked(object sender, RoutedEventArgs e)
        {

            DataHandler dao = new DataHandler();
            _moveList= await dao.LoadGame();
            if (_moveList.Count>0)
            {
                OnNewClicked(sender, e);
                if (sender == BtnReplay)
                {
                    enableButtons(false);
                    _timer =new DispatcherTimer();
                    _timer.Interval = new TimeSpan(0, 0, 1);
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

        private void OnNewClicked(object sender, RoutedEventArgs e)
        {
            _game.Reset();
            for (int row = 0; row < 9; row++)
            for (int col = 0; col < 7; col++)
                Update(row, col);
        }

        private void ResetDragState()
        {
            _startSquare = null;
            _endSquare = null;
            _dragStartRow = -1;
            _dragStartColumn = -1;
            _dragEndRow = -1;
            _dragEndColumn = -1;
        }

        private void Update(int row, int col)
        {
            Piece piece= _game.GetSquare(row, col).Piece;
            if (_game.GetSquare(row, col).Piece == null)
            {
                boardMatrix[row, col].Child = null;
                boardMatrix[row, col].BorderBrush = new SolidColorBrush(Colors.Black);
                boardMatrix[row, col].BorderThickness = new Thickness(1) ;
                boardMatrix[row, col].CanDrag = false;
            }

            
            else
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri($"ms-appx:///Assets/{piece.ImageFile}.jpg"));
                boardMatrix[row, col].Child = img;
                boardMatrix[row, col].BorderThickness = new Thickness(5);
                boardMatrix[row, col].CanDrag = true;
                if (piece.Color == "red")
                    boardMatrix[row, col].BorderBrush = new SolidColorBrush(Colors.Red);
                else
                    boardMatrix[row, col].BorderBrush = new SolidColorBrush(Colors.Blue);
            }
        }

        private async void ShowExceptionDialog(Exception ex)
        {
            MessageDialog dialog = new MessageDialog(ex.Message);
            await dialog.ShowAsync();
        }

        public void SaveGame(string fileName,List<Move> moveList)
        {

            List<string> moveAsString = new List<string>();
            foreach (Move move in moveList)
                moveAsString.Add(move.ToString());
            File.WriteAllLines(fileName, moveAsString);
        }


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

        private void MoveAndUpdate(Move move)
        {
            _game.MakeMove(_game.UpdateSquare(move));
            Update(move.StartSquare.Row, move.StartSquare.Col);
            Update(move.EndSquare.Row, move.EndSquare.Col);
        }

        private void enableButtons(bool enabled)
        {
            BtnNew.IsEnabled = enabled;
            BtnSave.IsEnabled = enabled;
            BtnLoad.IsEnabled = enabled;
        }


    }
}
