using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
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
        public int Score { get; set; }
        private DispatcherTimer timer;

        private bool isGameStart;
        private Food food;
        private Snake snake;

        public MainWindow()
        {
            InitializeComponent();

            CurrentRecs = new List<Rectangle>();
            timer = new DispatcherTimer();
            isGameStart = true;

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
                default:
                    break;
            }
        }

        private void GameStart_button_Click(object sender, RoutedEventArgs e)
        {
            GameStart_button.Visibility = Visibility.Collapsed;

            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += GameLoop;
            timer.Start();

        }

        private void GameLoop(object? sender, EventArgs e)
        {
            if (isGameStart)
            {
                InitializeGame();
                isGameStart = false;

                // draw it on game board
                CurrentRecs.Clear();
                foreach (var position in snake.Positions)
                {
                    Thickness thick = new Thickness() { Left = position.X * 10, Top = position.Y * 10 };
                    CurrentRecs.Add(new Rectangle() { Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)), Margin = thick, Width = 10, Height = 10, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top });
                    GameBoard.Children.Add(CurrentRecs.Last());
                }
                Thickness foodThick1 = new Thickness() { Left = food.Position.X * 10, Top = food.Position.Y * 10 };
                Rectangle foodRec1 = new Rectangle() { Fill = new SolidColorBrush(Color.FromArgb(255, 200, 10, 10)), Margin = foodThick1, Width = 10, Height = 10, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
                GameBoard.Children.Add(foodRec1);
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
            if (pos.X < 0 || pos.X > 49 || pos.Y < 0 || pos.Y > 49)
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
                Random rand = new Random();
                int x = rand.Next(0, 49);
                int y = rand.Next(0, 49);

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
                        x = rand.Next(0, 49);
                        y = rand.Next(0, 49);
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

            // draw it on game board
            CurrentRecs.Clear();
            GameBoard.Children.Clear();
            foreach (var position in snake.Positions)
            {
                Thickness thick = new Thickness() { Left = position.X * 10, Top = position.Y * 10 };
                CurrentRecs.Add(new Rectangle() { Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)), Margin = thick, Width = 10, Height = 10, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top });
                GameBoard.Children.Add(CurrentRecs.Last());
            }
            Thickness foodThick = new Thickness() { Left = food.Position.X * 10, Top = food.Position.Y * 10 };
            Rectangle foodRec = new Rectangle() { Fill = new SolidColorBrush(Color.FromArgb(255, 200, 10, 10)), Margin = foodThick, Width = 10, Height = 10, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
            GameBoard.Children.Add(foodRec);
        }

        private void EndGame()
        {
            timer.Stop();
            GameStart_button.Visibility = Visibility.Visible;

            GameBoard.Children.Clear();
        }

        private bool CheckCollision(int x1, int y1, int x2, int y2)
        {
            if (x1 == x2 && y1 == y2)
            {
                return true;
            }
            return false;
        }

        private void InitializeGame()
        {
            Random rand = new Random();
            int foodX = rand.Next(0, 49);
            int foodY = rand.Next(0, 49);

            int snakeX = rand.Next(9, 40);
            int snakeY = rand.Next(9, 40);

            double dist = Math.Sqrt(Math.Pow(foodX - snakeX, 2) + Math.Pow(foodY - foodX, 2));
            while (dist < 5)
            {
                snakeX = rand.Next(9, 40);
                snakeY = rand.Next(9, 40);
                dist = Math.Sqrt(Math.Pow(foodX - snakeX, 2) + Math.Pow(foodY - foodX, 2));
            }

            int dir = rand.Next(0, 3);
            food = new Food(foodX, foodY);
            snake = new Snake(snakeX, snakeY, (SnakeDirection)dir);
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
