namespace Puzzle_Game {
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    /// <summary>
    /// Interaction logic for StartGameWindow.xaml
    /// </summary>
    public partial class StartGameWindow : Window {
        private DispatcherTimer transition = new DispatcherTimer();
        private DispatcherTimer timeTimer = new DispatcherTimer();
        private DispatcherTimer congratsCanvasTimer = new DispatcherTimer();
        private DispatcherTimer imageOpacityTimer = new DispatcherTimer();
        private List<Image> imageList = new List<Image>();
        private List<Image> copyImageList = new List<Image>();
        private List<string> pictureList = new List<string>();
        private List<string> copyPictureList = new List<string>();

        private Image image1 = new Image();
        private Image image2 = new Image();

        private int count = 0;
        private int columnLength = 0;
        private int rowLength = 0;
        private int seconds = 0;
        private int minutes = 0;
        private int moveNow = 0;

        private double opacityDeduction = 0.1;
        private bool reachZero = false;

        public StartGameWindow() {
            InitializeComponent();

            MainWindow window = Application.Current.Windows[0] as MainWindow;
            if (window._difficultyRadioButton1.IsChecked == true) {
                _setDifficultyImage.Source = new BitmapImage(new Uri("../../Images/Background/EASY.png", UriKind.Relative));
                columnLength = 3;
                rowLength = 3;
            }
            else if (window._difficultyRadioButton2.IsChecked == true) {
                _setDifficultyImage.Source = new BitmapImage(new Uri("../../Images/Background/MEDIUM.png", UriKind.Relative));
                columnLength = 5;
                rowLength = 5;
            }
            else if (window._difficultyRadioButton3.IsChecked == true) {
                _setDifficultyImage.Source = new BitmapImage(new Uri("../../Images/Background/HARD.png", UriKind.Relative));
                columnLength = 7;
                rowLength = 7;
            }

            AddPicturesToList();
            Random random = new Random();
            random = new Random();
            int index = random.Next(pictureList.Count);
            AddColumnAndRow(columnLength, rowLength);
            CutImage(columnLength, rowLength, pictureList[index]);
            pictureList.RemoveAt(index);

            transition.Interval = TimeSpan.FromSeconds(0.09);
            transition.Tick += new EventHandler(Transition);
            transition.Start();

            timeTimer.Interval = TimeSpan.FromSeconds(1);
            timeTimer.Tick += new EventHandler(StartTimer);
            timeTimer.Start();

            congratsCanvasTimer.Interval = TimeSpan.FromSeconds(.03);
            congratsCanvasTimer.Tick += new EventHandler(MoveCongratsCanvas);

            imageOpacityTimer.Interval = TimeSpan.FromSeconds(.05);
            imageOpacityTimer.Tick += new EventHandler(SelectedImageAnimate);
        }
        private void AddPicturesToList() {
            for (int i = 1; i <= 17; i++) {
                pictureList.Add("../../Images/Pictures/pic" + i.ToString() + ".jpg");
                copyPictureList.Add("../../Images/Pictures/pic" + i.ToString() + ".jpg");
            }
        }
        private bool CheckIfWin() {
            for (int i = 0; i < imageList.Count; i++) {
                if (imageList[i] != copyImageList[i]) {
                    return false;
                }
            }
            return true;
        }
        private void GetRandomImage() {
            RemoveImageFromGrid();
            Random random = new Random();
            int index = random.Next(pictureList.Count);
            CutImage(columnLength, rowLength, pictureList[index]);
            pictureList.RemoveAt(index);
            count = 0;
            seconds = 0;
            minutes = 0;
        }
        private void RemoveImageFromGrid() {
            for (int i = _boardGrid.Children.Count - 1; i >= 0; i--) {
                if (_boardGrid.Children[i] is Image) {
                    _boardGrid.Children.RemoveAt(i);
                }
            }
        }
        private void AddColumnAndRow(int columnLength, int rowLength) {
            for (int i = 0; i < rowLength; i++) {
                for (int j = 0; j < columnLength; j++) {
                    ColumnDefinition column = new ColumnDefinition();
                    column.Width = new GridLength(_boardGrid.Width / columnLength);
                    _boardGrid.ColumnDefinitions.Add(column);
                }
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(_boardGrid.Height / rowLength);
                _boardGrid.RowDefinitions.Add(row);
            }
        }
        private void CutImage(int columnLength, int rowLength, string imagePath) {
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(imagePath, UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();

            imageList.Clear();
            copyImageList.Clear();

            for (int i = 0; i < rowLength; i++) {
                for (int j = 0; j < columnLength; j++) {
                    Image image = new Image();
                    image.Name = "_image" + i.ToString() + j.ToString();
                    image.Source = new CroppedBitmap(src, new Int32Rect(i * (int)src.Width / columnLength, j * (int)src.Height / rowLength, (int)src.Width / columnLength, (int)src.Height / rowLength));
                    image.Stretch = Stretch.Fill;
                    imageList.Add(image);
                    copyImageList.Add(image);
                    image.PreviewMouseDown += new MouseButtonEventHandler(MoveImage);
                    image.Cursor = Cursors.Hand;

                }
            }
            AddImageToGrid(columnLength, rowLength);
        }
        private void AddImageToGrid(int columnLength, int rowLength) {
            Random random = new Random();
            for (int i = 0; i < rowLength; i++) {
                for (int j = 0; j < columnLength; j++) {
                    int index = random.Next(imageList.Count);
                    Grid.SetColumn(imageList[index], i);
                    Grid.SetRow(imageList[index], j);
                    _boardGrid.Children.Add(imageList[index]);
                    imageList.RemoveAt(index);
                }
            }
            foreach (Image image in _boardGrid.Children) {
                imageList.Add(image);
            }
        }

        private void MoveCongratsCanvas(object sender, EventArgs e) {
            moveNow++;
            if(moveNow < 15) {
                return;
            }
            if(Canvas.GetTop(_congratsCanvas) < 0) {
                Canvas.SetTop(_congratsCanvas, Canvas.GetTop(_congratsCanvas) + 50);
            }
            else {
                moveNow = 0;
                congratsCanvasTimer.Stop();
            }
        }
        private void StartTimer(object sender, EventArgs e) {
            seconds++;
            if (seconds == 60) {
                seconds = 0;
                minutes++;
            }
            if(minutes >= 10) {
                if (seconds < 10) {
                    _timerLabel.Content = minutes.ToString() + ":0" + seconds.ToString();
                }
                else {
                    _timerLabel.Content = minutes.ToString() + ":" + seconds.ToString();
                }
            }
            else {
                if (seconds < 10) {
                    _timerLabel.Content = "0" + minutes.ToString() + ":0" + seconds.ToString();
                }
                else {
                    _timerLabel.Content = "0" + minutes.ToString() + ":" + seconds.ToString();
                }
            }
          
        }
        private void Transition(object sender, EventArgs e) {
            Canvas.SetLeft(_backGround1, Canvas.GetLeft(_backGround1) - 10);
            Canvas.SetLeft(_backGround2, Canvas.GetLeft(_backGround2) - 10);
            if (Canvas.GetLeft(_backGround1) <= -_backGround1.ActualWidth) {
                Canvas.SetLeft(_backGround1, _backGround1.ActualWidth);
            }
            if (Canvas.GetLeft(_backGround2) <= -_backGround2.ActualWidth) {
                Canvas.SetLeft(_backGround2, _backGround2.ActualWidth);
            }
           
        }
      
        private void SelectedImageAnimate(object sender, EventArgs e) {
            if (!reachZero) {
                if (image1.Opacity >= .5) {
                    image1.Opacity -= opacityDeduction;
                }
                else {
                    reachZero = true;
                }
            }
            if (reachZero) {
                if (image1.Opacity <= 1) {
                    image1.Opacity += opacityDeduction;
                }
                else {
                    reachZero = false;
                }
            }
        }
        private void MoveImage(object sender, MouseButtonEventArgs e) {
            Image image = sender as Image;
            count++;
            if (count == 1) {
                image1 = image;
                imageOpacityTimer.Start();
            }
            if (count == 2) {
                image1.Opacity = 1;
                imageOpacityTimer.Stop();
                image2 = image;
                count = 0;
                int tempColumn = Grid.GetColumn(image1);
                int tempRow = Grid.GetRow(image1);

                Grid.SetColumn(image1, Grid.GetColumn(image2));
                Grid.SetRow(image1, Grid.GetRow(image2));

                Grid.SetColumn(image2, tempColumn);
                Grid.SetRow(image2, tempRow);

                int index1 = -1;
                int index2 = -1;
                for(int i = 0; i < imageList.Count; i++) {
                    if(imageList[i] == image1) {
                        index1 = i;
                    }
                    if (imageList[i] == image2) {
                        index2 = i;
                    }
                }
                Image tempImage = imageList[index1];
                imageList[index1] = imageList[index2];
                imageList[index2] = tempImage;

                if (CheckIfWin()) {
                    congratsCanvasTimer.Start();
                }
            }
           
        }   
        private void ExitButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
        private void GiveUpButton_Click(object sender, RoutedEventArgs e) {
            if(pictureList.Count == 0) {
                foreach(var items in copyPictureList) {
                    pictureList.Add(items);
                }
            }
            GetRandomImage();
        }
        private void OKButton_Click(object sender, RoutedEventArgs e) {
            Canvas.SetTop(_congratsCanvas, -_congratsCanvas.Height);
            GetRandomImage();
        }
    }
}
