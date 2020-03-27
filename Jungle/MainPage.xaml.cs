using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Gaming.UI;
using Windows.Security.Cryptography.Core;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
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

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void InitiateBoard()
        {
            
            Grid myGrid = new Grid();
            myGrid.Width = 250;
            myGrid.Height = 100;
            myGrid.HorizontalAlignment = HorizontalAlignment.Left;
            myGrid.VerticalAlignment = VerticalAlignment.Top;

            for (int row = 0; row < 9; row++)
            for (int col = 0; col < 7; col++)
            {
                boardMatrix[row, col] = new Border()
                {
                    BorderThickness = new Thickness()
                    {
                        Bottom = 1,
                        Left = 1,
                        Right = 1,
                        Top = 1
                    },
                    BorderBrush = new SolidColorBrush(Colors.Black)
                };
                boardMatrix[row, col].SetValue(Grid.RowProperty, row);
                boardMatrix[row, col].SetValue(Grid.ColumnProperty, col);
                boardMatrix[row, col].AllowDrop = true;
                boardMatrix[row, col].CanDrag = true;
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
            if (_startSquare != null && _endSquare != null)
            {
                _game.MakeMove(new Move(_game.GetSquare(_dragStartRow, _dragStartColumn),_game.GetSquare(_dragEndRow, _dragEndColumn)));
                Update(_dragStartRow, _dragStartColumn);
                Update(_dragEndRow, _dragEndColumn);

                //Grid.SetRow(_startSquare, _dragEndRow);
                //Grid.SetColumn(_startSquare, _dragEndColumn);
                //Grid.SetRow(_endSquare, _dragStartRow);
                //Grid.SetColumn(_endSquare, _dragStartColumn);
                ResetDragState();
            }
        }



        private void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            InitiateBoard();
        }



        private void OnDragStartHandler(UIElement sender, DragStartingEventArgs args)
        {
            _startSquare = sender as Border;
            _dragStartRow = Grid.GetRow(_startSquare);
            _dragStartColumn = Grid.GetColumn(_startSquare);
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
                boardMatrix[row, col].Child = null;
            else
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri($"ms-appx:///Assets/{piece.ImageFile}.jpg"));
                boardMatrix[row, col].Child = img;
            }
        }
    }
}
