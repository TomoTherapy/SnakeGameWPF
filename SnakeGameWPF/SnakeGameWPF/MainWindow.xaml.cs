using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SnakeGameWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Rectangle> CurrentRecs { get; set; }
        public string Score 
        { 
            get 
            {
                return $"SCORE : {score}";
            } 
        }
        private int score;
        private Stopwatch watch;
        private bool isGameStart;
        private Food food;
        private Snake snake;
        private int targetInterval = 140;
        private const int BOARD_SIZE = 30;

        private SolidColorBrush snakeColor;
        private SolidColorBrush foodColor;

        private System.Media.SoundPlayer beepPlayer;
        private System.Media.SoundPlayer deathPlayer;
        private WMPLib.WindowsMediaPlayer wmp = new WMPLib.WindowsMediaPlayer();

        public MainWindow()
        {
            InitializeComponent();

            beepPlayer = new System.Media.SoundPlayer();
            beepPlayer.SoundLocation = "Assets/beep3-98810.wav";
            deathPlayer = new System.Media.SoundPlayer();
            deathPlayer.SoundLocation = "Assets/videogame-death-sound-43894.wav";
            wmp.URL = "Assets/videoplayback.mp3";
            wmp.settings.volume = 4;
            wmp.settings.setMode("loop", true);
            wmp.controls.play();

            snake = new Snake(0, 0, SnakeDirection.Up);
            food = new Food(0, 0);
            CurrentRecs = new List<Rectangle>();
            watch = new Stopwatch();
            isGameStart = true;
            score = 0;

            snakeColor = new SolidColorBrush(Color.FromArgb(255, 40, 200, 50));
            foodColor = new SolidColorBrush(Color.FromArgb(255, 250, 40, 40));
        }

        private void GameBoard_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    snake.Direction = SnakeDirection.Up;
                    break;
                case Key.Down:
                    snake.Direction = SnakeDirection.Down;
                    break;
                case Key.Left:
                    snake.Direction = SnakeDirection.Left;
                    break;
                case Key.Right:
                    snake.Direction = SnakeDirection.Right;
                    break;
                case Key.W:
                    snake.Direction = SnakeDirection.Up;
                    break;
                case Key.S:
                    snake.Direction = SnakeDirection.Down;
                    break;
                case Key.A:
                    snake.Direction = SnakeDirection.Left;
                    break;
                case Key.D:
                    snake.Direction = SnakeDirection.Right;
                    break;
                case Key.Space:
                    GameStart_button_Click(sender, e);
                    break;
                case Key.Enter:
                    GameStart_button_Click(sender, e);
                    break;
                default:
                    break;
            }
        }

        private void GameStart_button_Click(object sender, RoutedEventArgs e)
        {
            GameStart_button.Visibility = Visibility.Collapsed;
            GameOver_textBlock.Visibility = Visibility.Collapsed;
            WASD_image.Visibility = Visibility.Collapsed;
            watch.Start();
            CompositionTarget.Rendering += GameLoop;
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            double elapsedTime = watch.Elapsed.TotalMilliseconds;
            if (elapsedTime < targetInterval) return;

            watch.Restart();

            if (isGameStart)
            {
                InitializeGame();
                isGameStart = false;
                return;
            }

            Position pos = null;
            switch (snake.Direction)
            {
                case SnakeDirection.Up:
                    pos = snake.Positions[0];
                    snake.Positions.Insert(0, new Position(pos.X, pos.Y - 1));
                    break;
                case SnakeDirection.Down:
                    pos = snake.Positions[0];
                    snake.Positions.Insert(0, new Position(pos.X, pos.Y + 1));
                    break;
                case SnakeDirection.Left:
                    pos = snake.Positions[0];
                    snake.Positions.Insert(0, new Position(pos.X - 1, pos.Y));
                    break;
                case SnakeDirection.Right:
                    pos = snake.Positions[0];
                    snake.Positions.Insert(0, new Position(pos.X + 1, pos.Y));
                    break;
            }

            // detect collision
            pos = snake.Positions[0];
            if (pos.X < 0 || pos.X > BOARD_SIZE - 1 || pos.Y < 0 || pos.Y > BOARD_SIZE - 1)
            {
                EndGame();
                return;
            }

            for (int i = 2; i < snake.Positions.Count; ++i)
            {
                if (CheckCollision(pos.X, pos.Y, snake.Positions[i].X, snake.Positions[i].Y))
                {
                    EndGame();
                    return;
                }
            }

            // check food
            if (pos.X == food.Position.X && pos.Y == food.Position.Y)
            {
                beepPlayer.Play();
                score++;
                if (score == 7) targetInterval = 120;
                if (score == 14) targetInterval = 100;
                if (score == 21) targetInterval = 80;
                if (score == 28) targetInterval = 60;
                if (score == 36) targetInterval = 50;

                Random rand = new Random();
                int x = rand.Next(0, BOARD_SIZE - 1);
                int y = rand.Next(0, BOARD_SIZE - 1);

                while (true)
                {
                    bool isCollide = false;
                    foreach (var position in snake.Positions)
                    {
                        if (CheckCollision(x, y, position.X, position.Y))
                        {
                            isCollide = true;
                            break;
                        }
                    }

                    if (isCollide)
                    {
                        x = rand.Next(0, BOARD_SIZE - 1);
                        y = rand.Next(0, BOARD_SIZE - 1);
                    }
                    else
                    {
                        food.Position.X = x;
                        food.Position.Y = y;
                        break;
                    }
                }
            }
            else
            {
                snake.Positions.RemoveAt(snake.Positions.Count - 1);
            }

            Dispatcher.Invoke(() =>
            {
                // draw it on game board
                DrawGameBoard();
            });
        }
        private void InitializeGame()
        {
            Random rand = new Random();
            int foodX = rand.Next(0, BOARD_SIZE - 1);
            int foodY = rand.Next(0, BOARD_SIZE - 1);

            int goldFoodX = rand.Next(0, BOARD_SIZE - 1);
            int goldFoodY = rand.Next(0, BOARD_SIZE - 1);

            int snakeX = rand.Next(9, BOARD_SIZE - 10);
            int snakeY = rand.Next(9, BOARD_SIZE - 10);

            double dist1 = Math.Sqrt(Math.Pow(foodX - snakeX, 2) + Math.Pow(foodY - snakeY, 2));
            double dist2 = Math.Sqrt(Math.Pow(goldFoodX - snakeX, 2) + Math.Pow(goldFoodY - snakeY, 2));
            while (dist1 < 5 && dist2 < 5)
            {
                snakeX = rand.Next(9, BOARD_SIZE - 10);
                snakeY = rand.Next(9, BOARD_SIZE - 10);
                dist1 = Math.Sqrt(Math.Pow(foodX - snakeX, 2) + Math.Pow(foodY - snakeY, 2));
                dist2 = Math.Sqrt(Math.Pow(goldFoodX - snakeX, 2) + Math.Pow(goldFoodY - snakeY, 2));
            }

            int dir = rand.Next(0, 3);
            food = new Food(foodX, foodY);
            snake = new Snake(snakeX, snakeY, (SnakeDirection)dir);

            // draw it on game board
            DrawGameBoard();
        }

        private void DrawGameBoard()
        {
            CurrentRecs.Clear();
            GameBoard.Children.Clear();
            foreach (var position in snake.Positions)
            {
                Thickness thick = new Thickness() { Left = position.X * 10, Top = position.Y * 10 };
                CurrentRecs.Add(new Rectangle() { Fill = snakeColor, Margin = thick, Width = 10, Height = 10, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top });
                GameBoard.Children.Add(CurrentRecs.Last());
            }
            Thickness foodThick = new Thickness() { Left = food.Position.X * 10, Top = food.Position.Y * 10 };
            Ellipse foodEll = new Ellipse() { Fill = foodColor, Margin = foodThick, Width = 10, Height = 10, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
            Score_textBlock.Text = Score;
            GameBoard.Children.Add(foodEll);
        }

        private void EndGame()
        {
            deathPlayer.Play();
            CompositionTarget.Rendering -= GameLoop;
            isGameStart = true;
            GameStart_button.Visibility = Visibility.Visible;

            score = 0;
            targetInterval = 140;
            GameOver_textBlock.Visibility = Visibility.Visible;
        }

        private bool CheckCollision(int x1, int y1, int x2, int y2)
        {
            if (x1 == x2 && y1 == y2)
            {
                return true;
            }
            return false;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class Snake
    {
        public List<Position> Positions { get; set; }
        public SnakeDirection Direction { get; set; }

        public Snake(int x, int y, SnakeDirection dir)
        {
            Positions = new List<Position>
            {
                new Position(x, y)
            };

            Direction = dir;
            switch (dir)
            {
                case SnakeDirection.Up:
                    Positions.Add(new Position(x, y + 1));
                    Positions.Add(new Position(x, y + 2));
                    break;
                case SnakeDirection.Down:
                    Positions.Add(new Position(x, y - 1));
                    Positions.Add(new Position(x, y - 2));
                    break;
                case SnakeDirection.Left:
                    Positions.Add(new Position(x + 1, y));
                    Positions.Add(new Position(x + 2, y));
                    break;
                case SnakeDirection.Right:
                    Positions.Add(new Position(x - 1, y));
                    Positions.Add(new Position(x - 2, y));
                    break;
            }
        }
    }
    
    public class Food
    {
        public Position Position { get; set; }

        public Food(int x, int y)
        {
            Position = new Position(x, y);
        }
    }

    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public enum SnakeDirection
    {
        Up,
        Down,
        Left,
        Right
    }
}
