using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Jungle
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private int _currentRow, _dragStartRow, _dragEndRow, _dragEndColumn;
        private int _currentColumn, _dragStartColumn;
        private Border[,] boardMatrix = new Border[9, 7];
        private Border _startSquare, _endSquare;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void InitiateBoard()
        {
            //https://docs.microsoft.com/en-us/dotnet/framework/wpf/controls/how-to-create-a-grid-element
            Grid myGrid = new Grid();
            myGrid.Width = 250;
            myGrid.Height = 100;
            myGrid.HorizontalAlignment = HorizontalAlignment.Left;
            myGrid.VerticalAlignment = VerticalAlignment.Top;


            //Button button = new Button();
            //button.Content = "Test";
            //GrdMain.SetValue(Grid.RowProperty,9);
            //GrdMain.SetValue(Grid.ColumnProperty,7);
            ////GrdMain.SetValue(Grid.BorderBrushProperty, 7);
            //GrdMain.Width = 210;
            //GrdMain.Height = 270;

            for (int i = 0; i < 9; i++)
            for (int j = 0; j < 7; j++)
            {
                boardMatrix[i, j] = new Border()
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
                boardMatrix[i, j].SetValue(Grid.RowProperty, i);
                boardMatrix[i, j].SetValue(Grid.ColumnProperty, j);
                //boardMatrix[i, j].DragEnter += OnDragEnterHandler;
                boardMatrix[i, j].Drop += OnDropHandler;
                boardMatrix[i, j].AllowDrop = true;
                boardMatrix[i, j].CanDrag = true;
                boardMatrix[i, j].DragStarting += OnDragStartHandler;
                boardMatrix[i, j].DragOver += OnDragEnterHandler;
                boardMatrix[i, j].DropCompleted += OnDropCompletedHandler;
                GrdMain.Children.Add(boardMatrix[i, j]);
            }

            Image img = new Image();
            img.Source = new BitmapImage(new Uri("ms-appx:///Assets/LockScreenLogo.scale-200.png"));
            //img.CanDrag = true;
            boardMatrix[0, 0].Child = img;

            //boardMatrix[5, 5].PointerEntered += (object sender, PointerRoutedEventArgs e) =>
            //{
            //    Border currentGrid = sender as Border;
            //    _currentRow = (int) currentGrid.GetValue(Grid.RowProperty);
            //    _currentColumn = (int) currentGrid.GetValue(Grid.ColumnProperty);
            //    Console.WriteLine($"{_currentRow} {_currentColumn}");
            //};


            //img.DragStarting += (UIElement sender, DragStartingEventArgs e) => { };

            //img.PointerPressed += (object sender, PointerRoutedEventArgs e) =>
            //{
            //    //e.Handled = true;
            //    Image selectedImage = sender as Image;
            //    Border parent = selectedImage.Parent as Border;
            //    parent.Child = null;
            //    CanvasMain.Children.Add(selectedImage);
            //    PointerPoint point = e.GetCurrentPoint(CanvasMain);
            //    //originalPointerX = point.Position.X;
            //    //originalPointerY = point.Position.Y;
            //    Canvas.SetTop(img, point.Position.Y - 20);
            //    Canvas.SetLeft(img, point.Position.X - 20);
            //    //DataObject dataObj = new DataObject(selectedImage.Source);

            //    //boardMatrix[3, 3].Background = new SolidColorBrush(Color.FromArgb(255, 48, 179, 221));
            //    //.Child = null;
            //    //Canvas.SetTop(selectedImage, originalPositionTop + 100);
            //    //Canvas.SetLeft(selectedImage, originalPositionLeft +100);

            //};

            //img.PointerMoved += (object sender, PointerRoutedEventArgs e) =>
            //{
            //    //e.Handled = true;
            //    Image selectedImage = sender as Image;
            //    PointerPoint point = e.GetCurrentPoint(CanvasMain);
            //    //double currentPointerX = point.Position.X;
            //    //double currentPointerY = point.Position.Y;
            //    Canvas.SetTop(selectedImage, point.Position.Y - 20);
            //    Canvas.SetLeft(selectedImage, point.Position.X - 20);
            //};

            //img.PointerReleased += OnPointerReleasedHandler;



            //img.DragLeave += (object sender, DragEventArgs e) =>
            //{
            //    boardMatrix[5, 5].Background = new SolidColorBrush(Color.FromArgb(255, 48, 179, 221));
            //};



            //myBorder.SetValue(Grid.ColumnProperty, 1);
            //myBorder.SetValue(Grid.RowProperty, 1);

            //GrdMain.Children.Add(button);
            //mainWindow.SetTitleBar("Grid Sample");
        }

        //private void OnDragEnterHandler(object sender, DragEventArgs e)
        //{
        //    Border currentGrid = sender as Border;
        //    _currentRow = (int) currentGrid.GetValue(Grid.RowProperty);
        //    _currentColumn = (int) currentGrid.GetValue(Grid.ColumnProperty);
        //}

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
                Grid.SetRow(_startSquare, _dragEndRow);
                Grid.SetColumn(_startSquare, _dragEndColumn);
                Grid.SetRow(_endSquare, _dragStartRow);
                Grid.SetColumn(_endSquare, _dragStartColumn);
                ResetDragState();
            }
        }

        private async void OnDropHandler(object sender, DragEventArgs e)
        {
            Border currentGrid = sender as Border;
            //var selectedImage = await e.DataView.GetBitmapAsync();
            _currentRow = (int) currentGrid.GetValue(Grid.RowProperty);
            _currentColumn = (int) currentGrid.GetValue(Grid.ColumnProperty);
            Image selectedImage = sender as Image;
            boardMatrix[_currentRow, _currentColumn].Child = selectedImage;
        }

        private void OnPointerReleasedHandler(object sender, PointerRoutedEventArgs e)
        {
            //e.Handled = true;
            Image selectedImage = sender as Image;
            CanvasMain.Children.Remove(selectedImage);
            boardMatrix[_currentRow, _currentColumn].Child = selectedImage;

        }

        private void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            InitiateBoard();
        }

        private void OnPointerEnteredHandler(object sender, PointerRoutedEventArgs e)
        {
            Border currentGrid = sender as Border;
            currentGrid.Background = new SolidColorBrush(Color.FromArgb(255, 48, 179, 221)); 
            _currentRow = (int) currentGrid.GetValue(Grid.RowProperty);
            _currentColumn = (int) currentGrid.GetValue(Grid.ColumnProperty);
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
    }
}
