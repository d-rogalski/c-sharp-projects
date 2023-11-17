using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// <summary>
        /// Whether the player plays against computer or another person.
        /// </summary>
        private bool _singlePlayer;

        /// <summary>
        /// Constructor of the window.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            // Adding the DataContext
            gameSpace.DataContext = new TicTacToeGame();
            this.ShowMenu();
        }


        // -------------------- Control Actions --------------------
        /// <summary>
        /// Invoked when user clicks on a game field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void field_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Getting the field number
            Rectangle field = (Rectangle)sender;
            int fieldNumber = int.Parse(field.Uid);

            var _game = (TicTacToeGame)gameSpace.DataContext;

            // Making move
            _game.MakeMove(fieldNumber);

            if (this.TestResult()) return;

            // In the case of single player mode, simulating the other player's move
            if (_singlePlayer) _game.ComputerMove();

            this.TestResult();
        }

        /// <summary>
        /// Invoked when Single Player mode is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSingleButton_Click(object sender, RoutedEventArgs e)
        {
            var _game = (TicTacToeGame)this.gameSpace.DataContext;
            _game.ResetGame();

            _singlePlayer = true;
            this.HideMenu();
        }

        /// <summary>
        /// Invoked when Multi Player mode is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMultiButton_Click(object sender, RoutedEventArgs e)
        {
            var _game = (TicTacToeGame)this.gameSpace.DataContext;
            _game.ResetGame();

            _singlePlayer = false;
            this.HideMenu();
        }

        /// <summary>
        /// Invoked when Exit button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        // -------------------- Additional Methods --------------------
        /// <summary>
        /// Displays the main menu.
        /// </summary>
        /// <param name="gameResult">Message to display in the menu</param>
        private void ShowMenu(string gameResult="")
        {
            mainMenu.Visibility = Visibility.Visible;
            gameSpace.IsEnabled = false;
            gameBlur.Radius = 10;
            menuResultTextBlock.Text = gameResult;
        }

        /// <summary>
        /// Hides the main menu.
        /// </summary>
        private void HideMenu()
        {
            mainMenu.Visibility = Visibility.Hidden;
            gameSpace.IsEnabled = true;
            gameBlur.Radius = 0;
        }

        /// <summary>
        /// Evaluates the game and if the game is over dipslays the main menu.
        /// </summary>
        /// <returns>Whether the game is over or not</returns>
        private bool TestResult()
        {
            var _game = (TicTacToeGame)this.gameSpace.DataContext;
            var result = _game.IsGameOver();
            if (result == null) return false;
            else if (result == TicTacToeGame.Field.Empty) this.ShowMenu("It's a draw!");
            else this.ShowMenu($"{result} wins!");
            return true;
        }
    }
}
