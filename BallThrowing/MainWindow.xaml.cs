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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Diagnostics;
using System.IO;
///
/// формула нахождения точек на окружности: x^2+y^2=r^2
/// (x-a)^2+(y-b)^2=r^2, a и b - координаты центра окружности
/// x=√(r^2-(y-b)^2)+a
/// y=√(r^2-(x-a)^2)+b
/// 
namespace BallThrowing
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        DispatcherTimer MainGameTimer = new DispatcherTimer();
        Stopwatch GameTimerstWatch = new Stopwatch();

        string pathSaveFile = @"C:\Program Files\BallThrowing\save.txt";
        string pathDirectorySaveFile = @"C:\Program Files\BallThrowing";
        string strHighScore;
        int MainLvl = 0;

        Random rand = new Random();
        int dx = 0;             //перемещение мяча по X
        int dy = 6;             //перемещение мяча по Y
        int timeBallThrow = 40; //Время анимации мяча
        int speedPlatform = 0;  //Скорость платформы
        int SpRatioBall = 4;    //Ускорение анимации перемещения мяча
        int[,] randomThrowAngle = new int[6, 2] { { 4, 6 }, { 6, 4 }, { 3, 6 }, { 6, 3 }, { 5, 6 }, { 6, 5 } };

        int positionEnemyLeft = 220; //позиция в канвасе первого блока слева
        int positionEnemyTop = 50;  //позиция в канвасе первого блока сверху
        int widthEnemy = 180;       //ширина блоков
        int heightEnemy = 70;       //высота блоков
        int marginLeftEnemy = 10;   //отступ слева блоков
        int marginTopEnemy = 10;    //отступ сверху блоков
        int countRowsEnemy = 2;     //количество строк
        int countColumnsEnemy = 5;  //количество столбцов

        bool STOP = false;

        int lifes = 3;
        bool isLifesLost = false;
        Rectangles[,] massRectangles;
        SolidColorBrush[,] massColors = new SolidColorBrush[,] { };
        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = TimeSpan.FromMilliseconds(15);
            timer.Tick += timerTick;

            MainGameTimer.Interval = TimeSpan.FromSeconds(1);
            MainGameTimer.Tick += MainGameTimerTick;

            DirectoryInfo BallThrowing = new DirectoryInfo(pathDirectorySaveFile);
            if(!BallThrowing.Exists)
            {
                BallThrowing.Create();
                
            }
            FileInfo HighScoreSaveFile = new FileInfo(pathSaveFile);
            if(!HighScoreSaveFile.Exists)
            {
                File.Create(pathSaveFile).Close();
            }
            StreamReader reader = new StreamReader(pathSaveFile);
            strHighScore = reader.ReadLine();
            reader.Close();
            if (strHighScore == null)
            {
                StreamWriter writer = new StreamWriter(pathSaveFile);
                writer.Write(HighScoreCountText.Text);
                writer.Close();
                strHighScore = HighScoreCountText.Text;
            }
            HighScoreCountText.Text = strHighScore.ToString();
            
            
        }

        private void MainGameTimerTick(object sender, EventArgs e)
        {
            if (GameTimerstWatch.IsRunning)
            {
                TimeSpan timespan = GameTimerstWatch.Elapsed;
                string time = String.Format("{0:00}:{1:00}", timespan.Minutes, timespan.Seconds);
                Timer.Content = time;
            }
        }


        private void timerTick(object sender, EventArgs e)
        {
            if ((Canvas.GetLeft(Platform) < 10) && (speedPlatform < 0))
            {
                return;  
            }
            if ((Canvas.GetLeft(Platform) + Platform.ActualWidth > MainCanvas.ActualWidth - 10) && (speedPlatform > 0))
            {
                return;
            }
            Canvas.SetLeft(Platform, Canvas.GetLeft(Platform) + speedPlatform);
        }



        private void StartGame(object sender, RoutedEventArgs e)
        {
            STOP = false;
            StartButtonControlsPanel.IsEnabled = false;
            isLifesLost = false;
            MainGameTimer.Start();
            GameTimerstWatch.Restart();
            Canvas.SetLeft(Platform, 570);
            Canvas.SetLeft(Ball, 674);
            Canvas.SetTop(Ball, 533);
            dx = 0;
            dy = 6;
            Ball.Visibility = Visibility.Visible;
            Platform.Visibility = Visibility.Visible;
            Platform.Focusable = true;
            Platform.Focus();

            massRectangles = new Rectangles[countRowsEnemy, countColumnsEnemy];
            for (int i = 0; i < massRectangles.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < massRectangles.GetUpperBound(1) + 1; j++)
                {
                    SolidColorBrush randomColor = new SolidColorBrush(Color.FromRgb(Convert.ToByte(rand.Next(0, 255)), Convert.ToByte(rand.Next(0, 245)), (byte)rand.Next(0, 250)));
                    massRectangles[i, j] = new Rectangles(heightEnemy, widthEnemy, randomColor, Colors.Black, positionEnemyLeft + j * marginLeftEnemy + j * widthEnemy, positionEnemyTop + i * marginTopEnemy + i * heightEnemy);
                    MainCanvas.Children.Add(massRectangles[i, j].rect);
                }
            }
            moveBall();

        }

        private void GenerationBlocks()
        {
            switch(MainLvl) {
                case 1:
                    {
                        Ball.BeginAnimation(Canvas.LeftProperty, null);
                        Ball.BeginAnimation(Canvas.TopProperty, null);

                        positionEnemyLeft = 220; //позиция в канвасе первого блока слева
                        positionEnemyTop = 50;  //позиция в канвасе первого блока сверху
                        widthEnemy = 180;       //ширина блоков
                        heightEnemy = 55;       //высота блоков
                        marginLeftEnemy = 10;   //отступ слева блоков
                        marginTopEnemy = 10;    //отступ сверху блоков
                        countRowsEnemy = 4;     //количество строк
                        countColumnsEnemy = 5;  //количество столбцов

                        Canvas.SetLeft(Platform, 570);
                        Canvas.SetLeft(Ball, 674);
                        Canvas.SetTop(Ball, 533);
                        dx = 0;
                        dy = 6;
                        massRectangles = new Rectangles[countRowsEnemy, countColumnsEnemy];
                        for (int i = 0; i < massRectangles.GetUpperBound(0) + 1; i++)
                        {
                            for (int j = 0; j < massRectangles.GetUpperBound(1) + 1; j++)
                            {
                                SolidColorBrush randomColor = new SolidColorBrush(Color.FromRgb(Convert.ToByte(rand.Next(0, 255)), Convert.ToByte(rand.Next(0, 245)), (byte)rand.Next(0, 250)));
                                massRectangles[i, j] = new Rectangles(heightEnemy, widthEnemy, randomColor, Colors.Black, positionEnemyLeft + j * marginLeftEnemy + j * widthEnemy, positionEnemyTop + i * marginTopEnemy + i * heightEnemy);
                                MainCanvas.Children.Add(massRectangles[i, j].rect);
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        Ball.BeginAnimation(Canvas.LeftProperty, null);
                        Ball.BeginAnimation(Canvas.TopProperty, null);

                        positionEnemyLeft = 65; //позиция в канвасе первого блока слева
                        positionEnemyTop = 40;  //позиция в канвасе первого блока сверху
                        widthEnemy = 200;       //ширина блоков
                        heightEnemy = 55;       //высота блоков
                        marginLeftEnemy = 10;   //отступ слева блоков
                        marginTopEnemy = 10;    //отступ сверху блоков
                        countRowsEnemy = 5;     //количество строк
                        countColumnsEnemy = 6;  //количество столбцов

                        Canvas.SetLeft(Platform, 570);
                        Canvas.SetLeft(Ball, 674);
                        Canvas.SetTop(Ball, 533);
                        dx = 0;
                        dy = 6;
                        massRectangles = new Rectangles[countRowsEnemy, countColumnsEnemy];
                        for (int i = 0; i < massRectangles.GetUpperBound(0) + 1; i++)
                        {
                            for (int j = 0; j < massRectangles.GetUpperBound(1) + 1; j++)
                            {
                                SolidColorBrush randomColor = new SolidColorBrush(Color.FromRgb(Convert.ToByte(rand.Next(0, 255)), Convert.ToByte(rand.Next(0, 245)), (byte)rand.Next(0, 250)));
                                massRectangles[i, j] = new Rectangles(heightEnemy, widthEnemy, randomColor, Colors.Black, positionEnemyLeft + j * marginLeftEnemy + j * widthEnemy, positionEnemyTop + i * marginTopEnemy + i * heightEnemy);
                                MainCanvas.Children.Add(massRectangles[i, j].rect);
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        Ball.BeginAnimation(Canvas.LeftProperty, null);
                        Ball.BeginAnimation(Canvas.TopProperty, null);

                        positionEnemyLeft = 40; //позиция в канвасе первого блока слева
                        positionEnemyTop = 40;  //позиция в канвасе первого блока сверху
                        widthEnemy = 200;       //ширина блоков
                        heightEnemy = 55;       //высота блоков
                        marginLeftEnemy = 10;   //отступ слева блоков
                        marginTopEnemy = 10;    //отступ сверху блоков
                        countRowsEnemy = 6;     //количество строк
                        countColumnsEnemy = 6;  //количество столбцов

                        Canvas.SetLeft(Platform, 570);
                        Canvas.SetLeft(Ball, 674);
                        Canvas.SetTop(Ball, 533);
                        dx = 0;
                        dy = 6;
                        massRectangles = new Rectangles[countRowsEnemy, countColumnsEnemy];
                        for (int i = 0; i < massRectangles.GetUpperBound(0) + 1; i++)
                        {
                            for (int j = 0; j < massRectangles.GetUpperBound(1) + 1; j++)
                            {
                                SolidColorBrush randomColor = new SolidColorBrush(Color.FromRgb(Convert.ToByte(rand.Next(0, 255)), Convert.ToByte(rand.Next(0, 245)), (byte)rand.Next(0, 250)));
                                massRectangles[i, j] = new Rectangles(heightEnemy, widthEnemy, randomColor, Colors.Black, positionEnemyLeft + j * marginLeftEnemy + j * widthEnemy, positionEnemyTop + i * marginTopEnemy + i * heightEnemy);
                                MainCanvas.Children.Add(massRectangles[i, j].rect);
                            }
                        }
                        break;
                    }
            }

            if (MainLvl > 3)
            {
                Ball.BeginAnimation(Canvas.LeftProperty, null);
                Ball.BeginAnimation(Canvas.TopProperty, null);

                positionEnemyLeft = 10; //позиция в канвасе первого блока слева
                positionEnemyTop = 10;  //позиция в канвасе первого блока сверху
                widthEnemy = 190;       //ширина блоков
                heightEnemy = 55;       //высота блоков
                marginLeftEnemy = 9;   //отступ слева блоков
                marginTopEnemy = 8;    //отступ сверху блоков
                countRowsEnemy = 8;     //количество строк
                countColumnsEnemy = 7;  //количество столбцов

                Canvas.SetLeft(Platform, 570);
                Canvas.SetLeft(Ball, 674);
                Canvas.SetTop(Ball, 533);
                dx = 0;
                dy = 6;
                massRectangles = new Rectangles[countRowsEnemy, countColumnsEnemy];
                for (int i = 0; i < massRectangles.GetUpperBound(0) + 1; i++)
                {
                    for (int j = 0; j < massRectangles.GetUpperBound(1) + 1; j++)
                    {
                        SolidColorBrush randomColor = new SolidColorBrush(Color.FromRgb(Convert.ToByte(rand.Next(0, 255)), Convert.ToByte(rand.Next(0, 245)), (byte)rand.Next(0, 250)));
                        massRectangles[i, j] = new Rectangles(heightEnemy, widthEnemy, randomColor, Colors.Black, positionEnemyLeft + j * marginLeftEnemy + j * widthEnemy, positionEnemyTop + i * marginTopEnemy + i * heightEnemy);
                        MainCanvas.Children.Add(massRectangles[i, j].rect);
                    }
                }
            }
        }

        private void moveBall()
        {

            DoubleAnimation BallMovingTop = new DoubleAnimation
            {

                From = Canvas.GetTop(Ball),
                By = dy,
                Duration = TimeSpan.FromMilliseconds(timeBallThrow),
                SpeedRatio = SpRatioBall,

            };
            BallMovingTop.Completed += IsCompletedBallAnim;
            
            DoubleAnimation BallMovingLeft = new DoubleAnimation
            {

                From = Canvas.GetLeft(Ball),
                By = dx,
                Duration = TimeSpan.FromMilliseconds(timeBallThrow),
                SpeedRatio = SpRatioBall
            };
            Ball.BeginAnimation(Canvas.LeftProperty, BallMovingLeft);
            Ball.BeginAnimation(Canvas.TopProperty, BallMovingTop);
        }
        private void LooseLife()
        {
            Canvas.SetLeft(Platform, 570);
            Canvas.SetLeft(Ball, 674);
            Canvas.SetTop(Ball, 533);
            switch (lifes)
            {
                case 3:
                    {
                        dx = 0;
                        dy = 6;
                        Life3.Opacity = 0;
                        break;
                    }
                case 2:
                    {
                        dx = 0;
                        dy = 6;
                        Life2.Opacity = 0;
                        break;
                    }
                case 1:
                    {
                        dx = 0;
                        dy = 6;
                        Life1.Opacity = 0;
                        isLifesLost = true;
                        CheckRecord();
                        RestartTheGame();
                        return;
                    }
                default:
                    {
                        break;
                    }
            }
            lifes -= 1;

        }

        private void CheckRecord()
        {
            string countOfpoints = ScoreCount.Text;
            if(Convert.ToInt32(countOfpoints) > Convert.ToInt32(strHighScore))
            {
                StreamWriter writer = new StreamWriter(pathSaveFile);
                writer.Write(countOfpoints);
                writer.Close();
                HighScoreCountText.Text = countOfpoints;
                MessageBox.Show("NewRecord!");

            }
        }

        //RestartGame
        private void RestartTheGame()
        {
            STOP = true;
            CheckRecord();
            ScoreCount.Text = "0";
            MainGameTimer.Stop();
            GameTimerstWatch.Stop();
            Ball.BeginAnimation(Canvas.LeftProperty, null);
            Ball.BeginAnimation(Canvas.TopProperty, null);
            MainLvl = 0;
            Canvas.SetLeft(Platform, 570);
            Canvas.SetLeft(Ball, 674);
            Canvas.SetTop(Ball, 533);
            timer.Stop();
            lifes = 3;
            Life3.Opacity = 1;
            Life2.Opacity = 1;
            Life1.Opacity = 1;
            Ball.Visibility = Visibility.Hidden;
            Platform.Visibility = Visibility.Hidden;
            Platform.Focusable = false;
            dx = 0;
            dy = 0;
            for (int i = 0; i < massRectangles.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < massRectangles.GetUpperBound(1) + 1; j++)
                {
                    if(massRectangles[i, j] != null)
                    {
                        MainCanvas.Children.Remove(massRectangles[i, j].rect);
                        massRectangles[i, j] = null;
                    } 
                }
            }
            
            StartButtonControlsPanel.IsEnabled = true;
            
        }
        private void IsBlocksNull()
        {
            for (int i = 0; i < massRectangles.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < massRectangles.GetUpperBound(1) + 1; j++)
                {
                    if (massRectangles[i, j] == null)
                    {
                        continue;
                    } else
                    {
                        return;
                    }

                }
            }

            MainLvl += 1;
            GenerationBlocks();
        }
        private void IsCompletedBallAnim(object sender, EventArgs e)
        {
            int tempRand = rand.Next(6);
            double CenterBall_x;
            double CenterBall_y;
            double Ball_x;
            double Ball_y;
            double BallRad = Convert.ToDouble(Ball.ActualWidth) / 2;
            double RectX;
            double RectY;
            
            

            for (int i = 0; i < massRectangles.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < massRectangles.GetUpperBound(1) + 1; j++)
                {
                    if (massRectangles[i, j] == null)
                    {
                        continue;
                    }
                    //x=√(r^2-(y-b)^2)+a
                    //y=√(r^2-(x-a)^2)+b
                    CenterBall_x = Canvas.GetLeft(Ball) + BallRad;
                    CenterBall_y = Canvas.GetTop(Ball) + BallRad;


                    if ((Canvas.GetTop(Ball)) <= (Canvas.GetTop(massRectangles[i, j].rect) + massRectangles[i, j].rect.ActualHeight) &&
                            ((Canvas.GetLeft(massRectangles[i, j].rect)) <= CenterBall_x) &&
                            ((Canvas.GetLeft(massRectangles[i, j].rect) + massRectangles[i, j].rect.ActualWidth) >= CenterBall_x) &&
                            (Canvas.GetTop(Ball) >= (Canvas.GetTop(massRectangles[i, j].rect))))
                    {

                        //MessageBox.Show("Снизу");
                        MainCanvas.Children.Remove(massRectangles[i, j].rect);
                        massRectangles[i, j] = null;
                        
                        if (dx > 0)
                        {
                            dx = randomThrowAngle[tempRand, 0];

                        }
                        else
                        {
                            dx = randomThrowAngle[tempRand, 0];
                            dx = -dx;
                        }
                        dy = randomThrowAngle[tempRand, 1];
                        ScoreCount.Text = (Int32.Parse(ScoreCount.Text) + 1).ToString();
                        IsBlocksNull();

                    }
                    else if ((Canvas.GetTop(Ball) + Ball.ActualHeight) >= (Canvas.GetTop(massRectangles[i, j].rect)) &&
                        ((Canvas.GetLeft(massRectangles[i, j].rect)) <= CenterBall_x) &&
                        ((Canvas.GetLeft(massRectangles[i, j].rect) + massRectangles[i, j].rect.ActualWidth) >= CenterBall_x) &&
                        ((Canvas.GetTop(Ball) + Ball.ActualHeight) <= (Canvas.GetTop(massRectangles[i, j].rect) + massRectangles[i, j].rect.ActualHeight)))
                    {
                        //MessageBox.Show("Сверху");
                        MainCanvas.Children.Remove(massRectangles[i, j].rect);
                        massRectangles[i, j] = null;
                        
                        if (dx > 0)
                        {
                            dx = randomThrowAngle[tempRand, 0];

                        }
                        else
                        {
                            dx = randomThrowAngle[tempRand, 0];
                            dx = -dx;
                        }
                        dy = randomThrowAngle[tempRand, 1];
                        dy = -dy;
                        ScoreCount.Text = (Int32.Parse(ScoreCount.Text) + 1).ToString();
                        IsBlocksNull();
                    }
                    else if (CenterBall_y >= (Canvas.GetTop(massRectangles[i, j].rect)) &&
                      (Canvas.GetLeft(massRectangles[i, j].rect) <= (Canvas.GetLeft(Ball) + Ball.ActualWidth)) &&
                      ((Canvas.GetLeft(massRectangles[i, j].rect) + massRectangles[i, j].rect.ActualWidth) >= (Canvas.GetLeft(Ball) + Ball.ActualWidth)) &&
                      (CenterBall_y <= (Canvas.GetTop(massRectangles[i, j].rect) + massRectangles[i, j].rect.ActualHeight)))
                    {
                        //MessageBox.Show("Слева");
                        MainCanvas.Children.Remove(massRectangles[i, j].rect);
                        massRectangles[i, j] = null;
                        
                        if (dy > 0)
                        {
                            dy = randomThrowAngle[tempRand, 1];
                        }
                        else
                        {
                            dy = randomThrowAngle[tempRand, 1];
                            dy = -dy;
                        }
                        dx = randomThrowAngle[tempRand, 0];
                        dx = -dx;
                        ScoreCount.Text = (Int32.Parse(ScoreCount.Text) + 1).ToString();
                        IsBlocksNull();
                    }

                    else if (CenterBall_y >= (Canvas.GetTop(massRectangles[i, j].rect)) &&
                       ((Canvas.GetLeft(massRectangles[i, j].rect) + massRectangles[i, j].rect.ActualWidth) >= Canvas.GetLeft(Ball)) &&
                       ((Canvas.GetLeft(massRectangles[i, j].rect)) <= Canvas.GetLeft(Ball)) &&
                       (CenterBall_y <= (Canvas.GetTop(massRectangles[i, j].rect) + massRectangles[i, j].rect.ActualHeight)))
                    {
                        //MessageBox.Show("Справа");
                        MainCanvas.Children.Remove(massRectangles[i, j].rect);
                        massRectangles[i, j] = null;
                        
                        if (dy > 0)
                        {
                            dy = randomThrowAngle[tempRand, 1];
                        }
                        else
                        {
                            dy = randomThrowAngle[tempRand, 1];
                            dy = -dy;
                        }
                        dx = randomThrowAngle[tempRand, 0];
                        ScoreCount.Text = (Int32.Parse(ScoreCount.Text) + 1).ToString();
                        IsBlocksNull();
                    }
                    else if ((Canvas.GetTop(Ball) <= Canvas.GetTop(massRectangles[i, j].rect) + massRectangles[i, j].rect.ActualHeight) &&
                        (Canvas.GetTop(Ball) >= Canvas.GetTop(massRectangles[i, j].rect)) &&
                        (Canvas.GetLeft(Ball) + Ball.ActualWidth >= Canvas.GetLeft(massRectangles[i, j].rect)) &&
                        (Canvas.GetLeft(Ball) + Ball.ActualWidth <= Canvas.GetLeft(massRectangles[i, j].rect) + massRectangles[i, j].rect.ActualWidth))
                    {
                        RectX = Canvas.GetLeft(massRectangles[i, j].rect);
                        RectY = Canvas.GetTop(massRectangles[i, j].rect) + massRectangles[i, j].rect.ActualHeight;
                        Ball_x = Math.Sqrt(BallRad * BallRad - (RectY - CenterBall_y) * (RectY - CenterBall_y)) + CenterBall_x;
                        Ball_y = -Math.Sqrt(BallRad * BallRad - (RectX - CenterBall_x) * (RectX - CenterBall_x)) + CenterBall_y;

                        if ((Ball_x >= RectX) && (Ball_x <= RectX + widthEnemy) && (Ball_y <= RectY) && (Ball_y >= RectY - heightEnemy))
                        {
                            if (Ball_x - RectX >= RectY - Ball_y)
                            {
                                //MessageBox.Show("Снизу_угол");
                                MainCanvas.Children.Remove(massRectangles[i, j].rect);
                                massRectangles[i, j] = null;
                                
                                if (dx > 0)
                                {
                                    dx = randomThrowAngle[tempRand, 0];

                                }
                                else
                                {
                                    dx = randomThrowAngle[tempRand, 0];
                                    dx = -dx;
                                }
                                dy = randomThrowAngle[tempRand, 1];
                                ScoreCount.Text = (Int32.Parse(ScoreCount.Text) + 1).ToString();
                                IsBlocksNull();
                            }
                            else
                            {
                                //MessageBox.Show("Слева_угол");
                                MainCanvas.Children.Remove(massRectangles[i, j].rect);
                                massRectangles[i, j] = null;
                                
                                if (dy > 0)
                                {
                                    dy = randomThrowAngle[tempRand, 1];
                                }
                                else
                                {
                                    dy = randomThrowAngle[tempRand, 1];
                                    dy = -dy;
                                }
                                dx = randomThrowAngle[tempRand, 0];
                                dx = -dx;
                                ScoreCount.Text = (Int32.Parse(ScoreCount.Text) + 1).ToString();
                                IsBlocksNull();
                            }
                        }

                    }
                    else if ((Canvas.GetTop(Ball) <= Canvas.GetTop(massRectangles[i, j].rect) + massRectangles[i, j].rect.ActualHeight) &&
                       (Canvas.GetTop(Ball) >= Canvas.GetTop(massRectangles[i, j].rect)) &&
                       (Canvas.GetLeft(Ball) <= Canvas.GetLeft(massRectangles[i, j].rect) + massRectangles[i, j].rect.ActualWidth) &&
                       (Canvas.GetLeft(Ball) >= Canvas.GetLeft(massRectangles[i, j].rect)))
                    {
                        RectX = Canvas.GetLeft(massRectangles[i, j].rect) + massRectangles[i, j].rect.ActualWidth;
                        RectY = Canvas.GetTop(massRectangles[i, j].rect) + massRectangles[i, j].rect.ActualHeight;
                        Ball_x = -Math.Sqrt(BallRad * BallRad - (RectY - CenterBall_y) * (RectY - CenterBall_y)) + CenterBall_x;
                        Ball_y = -Math.Sqrt(BallRad * BallRad - (RectX - CenterBall_x) * (RectX - CenterBall_x)) + CenterBall_y;

                        if ((Ball_x <= RectX) && (Ball_x >= RectX - widthEnemy) && (Ball_y <= RectY) && (Ball_y >= RectY - heightEnemy))
                        {
                            if (RectX - Ball_x>= RectY - Ball_y)
                            {
                                //MessageBox.Show("Снизу_угол");
                                MainCanvas.Children.Remove(massRectangles[i, j].rect);
                                massRectangles[i, j] = null;
                                if (dx > 0)
                                {
                                    dx = randomThrowAngle[tempRand, 0];

                                }
                                else
                                {
                                    dx = randomThrowAngle[tempRand, 0];
                                    dx = -dx;
                                }
                                dy = randomThrowAngle[tempRand, 1];
                                ScoreCount.Text = (Int32.Parse(ScoreCount.Text) + 1).ToString();
                                IsBlocksNull();
                            }
                            else
                            {
                                //MessageBox.Show("Справа_угол");
                                MainCanvas.Children.Remove(massRectangles[i, j].rect);
                                massRectangles[i, j] = null;
                                if (dy > 0)
                                {
                                    dy = randomThrowAngle[tempRand, 1];
                                }
                                else
                                {
                                    dy = randomThrowAngle[tempRand, 1];
                                    dy = -dy;
                                }
                                dx = randomThrowAngle[tempRand, 0];
                                ScoreCount.Text = (Int32.Parse(ScoreCount.Text) + 1).ToString();
                                IsBlocksNull();
                            }
                        }

                    }
                    else if ((Canvas.GetTop(Ball) + Ball.ActualHeight >= Canvas.GetTop(massRectangles[i, j].rect)) &&
                     (Canvas.GetTop(Ball) + Ball.ActualHeight <= Canvas.GetTop(massRectangles[i, j].rect) + massRectangles[i, j].rect.ActualHeight) &&
                     (Canvas.GetLeft(Ball) <= Canvas.GetLeft(massRectangles[i, j].rect) + massRectangles[i, j].rect.ActualWidth) &&
                     (Canvas.GetLeft(Ball) >= Canvas.GetLeft(massRectangles[i, j].rect)))
                    {
                        RectX = Canvas.GetLeft(massRectangles[i, j].rect) + massRectangles[i, j].rect.ActualWidth;
                        RectY = Canvas.GetTop(massRectangles[i, j].rect);
                        Ball_x = -Math.Sqrt(BallRad * BallRad - (RectY - CenterBall_y) * (RectY - CenterBall_y)) + CenterBall_x;
                        Ball_y = Math.Sqrt(BallRad * BallRad - (RectX - CenterBall_x) * (RectX - CenterBall_x)) + CenterBall_y;

                        if ((Ball_x <= RectX) && (Ball_x >= RectX - widthEnemy) && (Ball_y >= RectY) && (Ball_y <= RectY + heightEnemy))
                        {
                            if (RectX - Ball_x >= Ball_y - RectY)
                            {
                                //MessageBox.Show("Сверху_угол");
                                MainCanvas.Children.Remove(massRectangles[i, j].rect);
                                massRectangles[i, j] = null;
                                if (dx > 0)
                                {
                                    dx = randomThrowAngle[tempRand, 0];

                                }
                                else
                                {
                                    dx = randomThrowAngle[tempRand, 0];
                                    dx = -dx;
                                }
                                dy = randomThrowAngle[tempRand, 1];
                                dy = -dy;
                                ScoreCount.Text = (Int32.Parse(ScoreCount.Text) + 1).ToString();
                                IsBlocksNull();
                            }
                            else
                            {
                                //MessageBox.Show("Справа_угол");
                                MainCanvas.Children.Remove(massRectangles[i, j].rect);
                                massRectangles[i, j] = null;
                                if (dy > 0)
                                {
                                    dy = randomThrowAngle[tempRand, 1];
                                }
                                else
                                {
                                    dy = randomThrowAngle[tempRand, 1];
                                    dy = -dy;
                                }
                                dx = randomThrowAngle[tempRand, 0];
                                ScoreCount.Text = (Int32.Parse(ScoreCount.Text) + 1).ToString();
                                IsBlocksNull();
                            }

                        }
                        
                    }
                    else if ((Canvas.GetTop(Ball) + Ball.ActualHeight >= Canvas.GetTop(massRectangles[i, j].rect)) &&
                       (Canvas.GetTop(Ball) + Ball.ActualHeight <= Canvas.GetTop(massRectangles[i, j].rect) + massRectangles[i, j].rect.ActualHeight) &&
                       (Canvas.GetLeft(Ball) + Ball.ActualWidth >= Canvas.GetLeft(massRectangles[i, j].rect)) &&
                       (Canvas.GetLeft(Ball) + Ball.ActualWidth <= Canvas.GetLeft(massRectangles[i, j].rect) + massRectangles[i, j].rect.ActualWidth))
                    {
                        RectX = Canvas.GetLeft(massRectangles[i, j].rect);
                        RectY = Canvas.GetTop(massRectangles[i, j].rect);
                        Ball_x = Math.Sqrt(BallRad * BallRad - (RectY - CenterBall_y) * (RectY - CenterBall_y)) + CenterBall_x;
                        Ball_y = Math.Sqrt(BallRad * BallRad - (RectX - CenterBall_x) * (RectX - CenterBall_x)) + CenterBall_y;

                        if ((Ball_x >= RectX) && (Ball_x <= RectX + widthEnemy) && (Ball_y >= RectY) && (Ball_y <= RectY + heightEnemy))
                        {
                            if (Ball_x - RectX >= Ball_y - RectY)
                            {
                                //MessageBox.Show("Сверху_угол");
                                MainCanvas.Children.Remove(massRectangles[i, j].rect);
                                massRectangles[i, j] = null;
                                if (dx > 0)
                                {
                                    dx = randomThrowAngle[tempRand, 0];

                                }
                                else
                                {
                                    dx = randomThrowAngle[tempRand, 0];
                                    dx = -dx;
                                }
                                dy = randomThrowAngle[tempRand, 1];
                                dy = -dy;
                                ScoreCount.Text = (Int32.Parse(ScoreCount.Text) + 1).ToString();
                                IsBlocksNull();
                            }
                            else
                            {
                                //MessageBox.Show("Слева_угол");
                                MainCanvas.Children.Remove(massRectangles[i, j].rect);
                                massRectangles[i, j] = null;
                                if (dy > 0)
                                {
                                    dy = randomThrowAngle[tempRand, 1];
                                }
                                else
                                {
                                    dy = randomThrowAngle[tempRand, 1];
                                    dy = -dy;
                                }
                                dx = randomThrowAngle[tempRand, 0];
                                dx = -dx;
                                ScoreCount.Text = (Int32.Parse(ScoreCount.Text) + 1).ToString();
                                IsBlocksNull();
                            }
                        }

                    }

                }
            }

                if (((Canvas.GetTop(Ball) + Ball.ActualHeight) > Canvas.GetTop(Platform)) && (Canvas.GetLeft(Platform) < (Canvas.GetLeft(Ball) + Ball.ActualWidth)) && ((Canvas.GetLeft(Platform) + Platform.ActualWidth) > Canvas.GetLeft(Ball)))
            {

                if (dx > 0)
                {
                    dx = randomThrowAngle[tempRand, 0];

                }
                else
                {
                    dx = randomThrowAngle[tempRand, 0];
                    dx = -dx;
                }

                dy = randomThrowAngle[tempRand, 1];
                dy = -dy;
            }

            if (Canvas.GetTop(Ball) > MainCanvas.ActualHeight - 70)
            {
                dy = randomThrowAngle[tempRand, 1];
                dy = -dy;
                Ball.BeginAnimation(Canvas.LeftProperty, null);
                Ball.BeginAnimation(Canvas.TopProperty, null);
                LooseLife();
                if(isLifesLost)
                {
                    return;
                }
                
            }

            if (Canvas.GetTop(Ball) < 10)
            {
                dy = randomThrowAngle[tempRand, 1];
                if (dx > 0)
                {
                    dx = randomThrowAngle[tempRand, 0];
                }
                else
                {
                    dx = randomThrowAngle[tempRand, 0];
                    dx = -dx;
                }
            }
            if (Canvas.GetLeft(Ball) > MainCanvas.ActualWidth - 70)
            {
                dx = randomThrowAngle[tempRand, 0];
                dx = -dx;
                if (dy > 0)
                {
                    dy = randomThrowAngle[tempRand, 1];
                }
                else
                {
                    dy = randomThrowAngle[tempRand, 1];
                    dy = -dy;
                }
            }

            if (Canvas.GetLeft(Ball) < 10)
            {
                dx = randomThrowAngle[tempRand, 0];
                if (dy > 0)
                {
                    dy = randomThrowAngle[tempRand, 1];
                }
                else
                {
                    dy = randomThrowAngle[tempRand, 1];
                    dy = -dy;
                }
            }


            if(STOP)
            {
                return;
            }

            DoubleAnimation BallMovingLeft = new DoubleAnimation
            {
                From = Canvas.GetLeft(Ball),
                By = dx,
                Duration = TimeSpan.FromMilliseconds(timeBallThrow),
                SpeedRatio = SpRatioBall
            };
            DoubleAnimation BallMovingTop = new DoubleAnimation
            {

                From = Canvas.GetTop(Ball),
                By = dy,
                Duration = TimeSpan.FromMilliseconds(timeBallThrow),
                SpeedRatio = SpRatioBall
            };
            BallMovingTop.Completed += IsCompletedBallAnim;
            Ball.BeginAnimation(Canvas.LeftProperty, BallMovingLeft);
            Ball.BeginAnimation(Canvas.TopProperty, BallMovingTop);

        }

        

        private void PlatformMove(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A)
            {
                speedPlatform = -6;
                timer.Start();
            }
            if (e.Key == Key.D)
            {
                speedPlatform = 6;
                timer.Start();
            }
        }

        private void PlatformMoveKeyUp(object sender, KeyEventArgs e)
        {
            timer.Stop();
        }

        private void RestartButton(object sender, RoutedEventArgs e)
        {
            RestartTheGame();
        }

        private void InformationAboutUs(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Created By Mikhail Kuzikov & Mikhail Naiman");
        }
    }
    public class Rectangles
    {
        public Rectangle rect;

        public Rectangles(int H, int W, Brush clr, Color clrStr, int l, int t)
        {

            SolidColorBrush scbclrStr = new SolidColorBrush();
            scbclrStr.Color = clrStr;
            
            rect = new Rectangle
            {
                Width = W,
                Height = H,
                Fill = clr,
                Stroke = scbclrStr,
                StrokeThickness = 2
            };
            
            Canvas.SetLeft(rect, l);
            Canvas.SetTop(rect, t);
        }


    }
}
