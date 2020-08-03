namespace Puzzle_Game {
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        
        public MainWindow() {
            InitializeComponent();
        
            AddImageTitleToList();

            titleTimer.Interval = TimeSpan.FromSeconds(.04);
            titleTimer.Tick += new EventHandler(MoveTitle);
            titleTimer.Start();

            transition.Interval = TimeSpan.FromSeconds(.09);
            transition.Tick += new EventHandler(Transition);
            transition.Start();

            preperatioTimer.Interval = TimeSpan.FromSeconds(0.05);
            preperatioTimer.Tick += new EventHandler(MovePreparation);

            helpTimer.Interval = TimeSpan.FromSeconds(.05);
            helpTimer.Tick += new EventHandler(MoveHelpGrid);

            _videoMedia.Play();
            _videoMedia.Pause();
        }
        private List<Image> titleList = new List<Image>();
        private DispatcherTimer titleTimer = new DispatcherTimer();
        private DispatcherTimer preperatioTimer = new DispatcherTimer();
        private DispatcherTimer helpTimer = new DispatcherTimer();
        private DispatcherTimer transition = new DispatcherTimer();
        private int index = 0;

        private void AddImageTitleToList() {
            titleList.Add(_title1);
            titleList.Add(_title2);
            titleList.Add(_title3);
            titleList.Add(_title4);
            titleList.Add(_title5);
            titleList.Add(_title6);
            titleList.Add(_title7);
            titleList.Add(_title8);
            titleList.Add(_title9);
            titleList.Add(_title10);
        }
        private bool moveRight = false;
        private void MoveHelpGrid(object sender, EventArgs e) {
            if(!moveRight) {
                Canvas.SetLeft(_helpGrid, Canvas.GetLeft(_helpGrid) + 50);
                if(Canvas.GetLeft(_helpGrid) >= 0) {
                    moveRight = true;
                    helpTimer.Stop();
                }
            }
            else {
                Canvas.SetLeft(_helpGrid, Canvas.GetLeft(_helpGrid) - 50);
                if (Canvas.GetLeft(_helpGrid) <= -_helpGrid.Width) {
                    _buttonGrid.IsEnabled = true;
                    _buttonGrid.Visibility = Visibility.Visible;
                    moveRight = false;
                    helpTimer.Stop();
                }
            }
        }
        private void MovePreparation(object sender, EventArgs e) {
            if (Canvas.GetLeft(_preperaionGrid) < 0) {
                Canvas.SetLeft(_preperaionGrid, Canvas.GetLeft(_preperaionGrid) + 50);
            }
            else {
                preperatioTimer.Stop();
            }
        }
        private void MoveTitle(object sender, EventArgs e) {
            if (Canvas.GetTop(titleList[index]) < 5) {
                Canvas.SetTop(titleList[index], Canvas.GetTop(titleList[index]) + 10);
            }
            if (Canvas.GetTop(titleList[index]) >= 5) {
                index++;
            }
            if (index >= titleList.Count) {
                _buttonGrid.IsEnabled = true;
                _buttonGrid.Visibility = Visibility.Visible;
                titleTimer.Stop();
            }
        }
        private void Transition(object sender, EventArgs e) {
            Canvas.SetLeft(_backGround1, Canvas.GetLeft(_backGround1) - 10);
            Canvas.SetLeft(_backGround2, Canvas.GetLeft(_backGround2) - 10);
            if(Canvas.GetLeft(_backGround1) <= -_backGround1.ActualWidth) {
                Canvas.SetLeft(_backGround1, _backGround1.ActualWidth);
            }
            if (Canvas.GetLeft(_backGround2) <= -_backGround2.ActualWidth) {
                Canvas.SetLeft(_backGround2, _backGround2.ActualWidth);
            }
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e) {
            _buttonGrid.IsEnabled = false;
            _buttonGrid.Visibility = Visibility.Hidden;
            helpTimer.Start();
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e) {
            _buttonGrid.IsEnabled = false;
            _buttonGrid.Visibility = Visibility.Hidden;
            preperatioTimer.Start();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e) {
            if(_difficultyRadioButton1.IsChecked == false &&
               _difficultyRadioButton2.IsChecked == false &&
               _difficultyRadioButton3.IsChecked == false) {
                MessageBox.Show("Please select difficulty", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            _buttonGrid.IsEnabled = true;
            _buttonGrid.Visibility = Visibility.Visible;
            Canvas.SetLeft(_preperaionGrid, -_preperaionGrid.Width);
            this.Hide();
            StartGameWindow startGameWindow = new StartGameWindow();
            startGameWindow.ShowDialog();
            this.Show();
        }

        private void Media_Ended(object sender, RoutedEventArgs e) {
            _videoMedia.Position = TimeSpan.Zero;
            _videoMedia.Stop();
        }

        private void PlayVideoButton_Click(object sender, RoutedEventArgs e) {
            _videoMedia.Play();
        }

        private void PauseVideoButton_Click(object sender, RoutedEventArgs e) {
            _videoMedia.Pause();
        }

        private void BackVideoButton_Click(object sender, RoutedEventArgs e) {
            _videoMedia.Position = TimeSpan.Zero;
            _videoMedia.Stop();
            helpTimer.Start();
          
        }

        private void PrepExitButton_Click(object sender, RoutedEventArgs e) {
            Canvas.SetLeft(_preperaionGrid, -_preperaionGrid.Width);
            _buttonGrid.IsEnabled = true;
            _buttonGrid.Visibility = Visibility.Visible;
        }
    }
}
