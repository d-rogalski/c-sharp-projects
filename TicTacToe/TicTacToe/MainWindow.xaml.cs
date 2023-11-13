using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading;
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

namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ImageBrush _crossImage = Utils.ImageToImageBrush("images/cross.png");
        private ImageBrush _circleImage = Utils.ImageToImageBrush("images/circle.png");
        private TicTacToeGame _game;
        private bool _singlePlayer;
        public MainWindow()
        {
            InitializeComponent();

            ShowMenu();
        }

        private void field_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle field = (Rectangle)sender;
            if (_game.MakeMove(int.Parse(field.Uid)) >= 0)
            {
                field.Style = (Style)this.FindResource($"field{_game.Opponent}Style");
                field.IsEnabled = false;
            }

            if (TestResult()) return;

            if (_singlePlayer)
            {
                int res = _game.ComputerMove();
                if (res >= 0)
                {
                    field = (Rectangle)this.FindName($"field{res}");
                    field.Style = (Style)this.FindResource($"field{_game.Opponent}Style");
                    field.IsEnabled = false;
                }
            }

            if (TestResult()) return;
        }

        private void ShowMenu(string gameResult="")
        {
            mainMenu.Visibility = Visibility.Visible;
            gameSpace.IsEnabled = false;
            gameBlur.Radius = 10;
            menuResultTextBlock.Text = gameResult;
        }

        private void HideMenu()
        {
            mainMenu.Visibility = Visibility.Hidden;
            gameSpace.IsEnabled = true;
            gameBlur.Radius = 0;
        }

        private void ClearFields()
        {
            for (int i = 0; i < 9; i++)
            {
                Rectangle field = (Rectangle)this.FindName($"field{i}");
                field.IsEnabled = true;
                field.Style = (System.Windows.Style)this.FindResource("fieldStyle");
            }
        }

        private bool TestResult()
        {
            var result = _game.IsGameOver();
            if (result == null) return false;
            else if (result == TicTacToeGame.Field.Empty) ShowMenu("It's a draw!");
            else ShowMenu($"{result} wins!");
            return true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }

        private void menuSingleButton_Click(object sender, RoutedEventArgs e)
        {
            _game = new TicTacToeGame();
            ClearFields();
            
            _singlePlayer = true;
            HideMenu();
        }

        private void menuMultiButton_Click(object sender, RoutedEventArgs e)
        {
            _game = new TicTacToeGame();
            ClearFields();

            _singlePlayer = false;
            HideMenu();
        }

        private void menuExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
