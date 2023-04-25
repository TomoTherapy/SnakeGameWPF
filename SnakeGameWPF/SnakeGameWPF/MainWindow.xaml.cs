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
using System.Windows.Threading;

namespace SnakeGameWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Rectangle> CurrentRecs { get; set; }
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();

            CurrentRecs = new List<Rectangle>();
            timer = new DispatcherTimer();

            InitializeGame();
        }

        private void InitializeGame()
        {

        }

        private void GameBoard_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    PressedUp();
                    break;
                case Key.Down:
                    PressedDown();
                    break;
                case Key.Left:
                    PressedLeft();
                    break;
                case Key.Right:
                    PressedRight();
                    break;
                case Key.W:
                    PressedUp();
                    break;
                case Key.S:
                    PressedDown();
                    break;
                case Key.A:
                    PressedLeft();
                    break;
                case Key.D:
                    PressedRight();
                    break;
                default:
                    break;
            }
        }

        private void PressedUp()
        {

        }

        private void PressedDown()
        {

        }

        private void PressedLeft()
        {

        }

        private void PressedRight()
        {

        }

        private void GameStart_button_Click(object sender, RoutedEventArgs e)
        {
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += GameLoop;
            timer.Start();
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            
        }
    }

    public class Snake
    {
        public int Length { get; set; }
        public Queue<Position> Locations { get; set; }

        public Snake()
        {
            Locations = new Queue<Position>();
        }


    }
    
    public class Food
    {
        public int PositionX { get; set; }
        public int PositionY { get; set; }

    }

    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    
}
