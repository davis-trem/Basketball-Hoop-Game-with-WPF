using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CSc_4820_Final_Project.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point? _start = null;
        bool passed = false; //checks if bomb passed hoop
        int score = 0;
        int lives = 5;
        DispatcherTimer _timer;
        TimeSpan _time;

        public MainWindow()
        {
            InitializeComponent();

            _time = TimeSpan.FromSeconds(30);

            _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                timeText.Content = "Time: " + _time.ToString("ss");
                if (_time == TimeSpan.Zero)
                {
                    _timer.Stop();
                    loser();
                }
                _time = _time.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);
            _timer.Stop();

        }

        private void bStart_Click(object sender, RoutedEventArgs e)
        {
            BallViewModel.MoveHoop(hoopBack);
            BallViewModel.MoveHoop(hoopFront);
            BallViewModel.MoveHoop(hoopInner);
            bStart.Visibility = Visibility.Hidden;
            cover.Visibility = Visibility.Hidden;

            if (modeText.Content.Equals("Mode: Time Trail"))
            {
                _timer.Start();
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (_start == null) _start = Mouse.GetPosition((UIElement)sender);
            Path path1 = new Path();
            path1.Fill = Brushes.White;
            path1.Stroke = Brushes.BlueViolet;
            path1.StrokeThickness = 5;
            drawCanvas.Children.Add(path1);
            path1.Data = new PathGeometry();
            var pg = path1.Data as PathGeometry;
            pg.FillRule = FillRule.Nonzero;

            pg.Figures.Add(new PathFigure
            {
                StartPoint = _start.Value,
                IsClosed = false,
                IsFilled = false,
                Segments =
                {
                    new LineSegment {IsStroked = false}
                }
            });

        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_start == null) return;
            var p = Mouse.GetPosition((UIElement)sender);
            var path1 = drawCanvas.Children[0] as Path;
            var gg = path1.Data as PathGeometry;
            gg.FillRule = FillRule.Nonzero;

            var figs = gg.Figures.Last();
            var seg = figs.Segments.Last() as LineSegment;
            seg.Point = p;
            seg.IsStroked = true;
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (_start == null) return;
            var p = Mouse.GetPosition((UIElement)sender);
            var path1 = drawCanvas.Children[0] as Path;
            var gg = path1.Data as PathGeometry;
            gg.FillRule = FillRule.Nonzero;

            var fig = gg.Figures.Last();
            fig.StartPoint = _start.Value;
            var seg = fig.Segments.Last() as LineSegment;
            seg.Point = p;

            BallViewModel.MoveBall( ((_start.Value.X-p.X)*10), (_start.Value.Y - p.Y) * 5.7, imageBomb);
            passed = false; //reset check for bomb passing hoop
            
            _start = null;
            drawCanvas.Children.Clear();

        }

        private void xDataChanged(object sender, RoutedEventArgs e)
        {
            
            if (passed==false && Canvas.GetLeft(imageBomb) > Canvas.GetLeft(hoopBack) && Canvas.GetLeft(imageBomb) < (Canvas.GetLeft(hoopBack) + hoopBack.Width))
            {
                passed = true;
                Console.WriteLine("PASSED, must enter"+ Canvas.GetTop(hoopBack) + " to "+ (Canvas.GetTop(hoopBack) + hoopBack.Height));
                if(Canvas.GetTop(imageBomb) >= Canvas.GetTop(hoopBack) && (Canvas.GetTop(imageBomb)+imageBomb.Height) <= (Canvas.GetTop(hoopBack) + hoopBack.Height))
                {
                    score++;
                    scoreText.Content = "Score: " + score;
                    Console.WriteLine("ENTERED!!!");
                }
                else
                {
                    explosion();
                    if (modeText.Content.Equals("Mode: Life Limit"))
                    {
                        lives--;
                        liveText.Content = "Lives: " + lives;
                        if (lives <= 0)
                        {
                            loser();
                        }
                    }
                    Console.WriteLine("BOOOM!! at top:"+Canvas.GetTop(imageBomb) +", left:"+ Canvas.GetLeft(imageBomb));
                }
            }
            
        }

        public void explosion()
        {
            Canvas.SetTop(exploImg, (Canvas.GetTop(imageBomb)-50) );
            Canvas.SetLeft(exploImg, (Canvas.GetLeft(imageBomb)-50) );
            
            double duration = 0.2;

            DoubleAnimation exploAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(duration)));
            exploAnimation.AutoReverse = true;
            exploAnimation.FillBehavior = FillBehavior.Stop;

            DoubleAnimation sizeHAnimation = new DoubleAnimation(exploImg.Height, (exploImg.Height+200), new Duration(TimeSpan.FromSeconds(duration)));
            sizeHAnimation.FillBehavior = FillBehavior.Stop;
            DoubleAnimation sizeWAnimation = new DoubleAnimation(exploImg.Width, (exploImg.Width + 200), new Duration(TimeSpan.FromSeconds(duration)));
            sizeWAnimation.FillBehavior = FillBehavior.Stop;

            exploImg.BeginAnimation(OpacityProperty, exploAnimation);
            exploImg.BeginAnimation(HeightProperty, sizeHAnimation);
            exploImg.BeginAnimation(WidthProperty, sizeWAnimation);
        }

        // CUSTOMIZE MENUS
        private void menuBomb_Click(object sender, RoutedEventArgs e)
        {
            imageBomb.Source = new BitmapImage(new Uri(@"/Images/bomb.png", UriKind.Relative));
        }

        private void menuBball_Click(object sender, RoutedEventArgs e)
        {
            imageBomb.Source = new BitmapImage(new Uri(@"/Images/bball.png", UriKind.Relative));
        }

        private void menuKermit_Click(object sender, RoutedEventArgs e)
        {
            imageBomb.Source = new BitmapImage(new Uri(@"/Images/kermit.png", UriKind.Relative));
        }

        private void menuBeach_Click(object sender, RoutedEventArgs e)
        {
            imageBG.Source = new BitmapImage(new Uri(@"/Images/bgBeach.jpg", UriKind.Relative));
        }

        private void menuCourt_Click(object sender, RoutedEventArgs e)
        {
            imageBG.Source = new BitmapImage(new Uri(@"/Images/bgBasketball.jpg", UriKind.Relative));
        }

        private void menuHell_Click(object sender, RoutedEventArgs e)
        {
            imageBG.Source = new BitmapImage(new Uri(@"/Images/bgHell.png", UriKind.Relative));
        }

        // GAME MENUS
        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void menuRestart_Click(object sender, RoutedEventArgs e)
        {
            score = 0;
            scoreText.Content = "Score: " + score;
            bStart.Visibility = Visibility.Visible;
            cover.Visibility = Visibility.Visible;
            resultText.Visibility = Visibility.Hidden;
            BallViewModel.StopHoop(hoopBack);
            BallViewModel.StopHoop(hoopInner);
            BallViewModel.StopHoop(hoopFront);

            lives = 5;
            liveText.Content = "Lives: " + lives;

            if (modeText.Content.Equals("Mode: Time Trail"))
            {
                _time = TimeSpan.FromSeconds(30);

                _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
                {
                    timeText.Content = "Time: " + _time.ToString("ss");
                    if (_time == TimeSpan.Zero)
                    {
                        _timer.Stop();
                        loser();
                    }
                    _time = _time.Add(TimeSpan.FromSeconds(-1));
                }, Application.Current.Dispatcher);
                _timer.Stop();

            }
        }

        private void menuFree_Click(object sender, RoutedEventArgs e)
        {
            score = 0;
            scoreText.Content = "Score: " + score;
            modeText.Content = "Mode: Free Play";
            bStart.Visibility = Visibility.Visible;
            cover.Visibility = Visibility.Visible;
            liveText.Visibility = Visibility.Hidden;
            timeText.Visibility = Visibility.Hidden;
            resultText.Visibility = Visibility.Hidden;
            BallViewModel.StopHoop(hoopBack);
            BallViewModel.StopHoop(hoopInner);
            BallViewModel.StopHoop(hoopFront);
        }

        private void menuLives_Click(object sender, RoutedEventArgs e)
        {
            score = 0;
            scoreText.Content = "Score: " + score;
            modeText.Content = "Mode: Life Limit";
            bStart.Visibility = Visibility.Visible;
            cover.Visibility = Visibility.Visible;
            liveText.Visibility = Visibility.Visible;
            timeText.Visibility = Visibility.Hidden;
            resultText.Visibility = Visibility.Hidden;

            lives = 5;
            liveText.Content = "Lives: " + lives;

            BallViewModel.StopHoop(hoopBack);
            BallViewModel.StopHoop(hoopInner);
            BallViewModel.StopHoop(hoopFront);
        }

        private void menuTime_Click(object sender, RoutedEventArgs e)
        {
            score = 0;
            scoreText.Content = "Score: " + score;
            modeText.Content = "Mode: Time Trail";
            bStart.Visibility = Visibility.Visible;
            cover.Visibility = Visibility.Visible;
            timeText.Visibility = Visibility.Visible;
            liveText.Visibility = Visibility.Hidden;
            resultText.Visibility = Visibility.Hidden;

            _time = TimeSpan.FromSeconds(30);

            _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                timeText.Content = "Time: " + _time.ToString("ss");
                if (_time == TimeSpan.Zero)
                {
                    _timer.Stop();
                    loser();
                }
                _time = _time.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);
            _timer.Stop();

            BallViewModel.StopHoop(hoopBack);
            BallViewModel.StopHoop(hoopInner);
            BallViewModel.StopHoop(hoopFront);
        }

        public void loser()
        {
            cover.Visibility = Visibility.Visible;
            resultText.Visibility = Visibility.Visible;
        }

    }
}
