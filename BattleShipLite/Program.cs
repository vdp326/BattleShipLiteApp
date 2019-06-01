using BattleshipLiteLibrary;
using BattleshipLiteLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BattleShipLite
{
    class Program
    {
        static void Main(string[] args)
        {
            WelcomeMessage();

            PlayerInfoModel activePlayer = CreatePlayer("Player 1");
            PlayerInfoModel opponent = CreatePlayer("Player 2");
            PlayerInfoModel winner = null;

            do
            {
                //display grid from player 1 from where they fired
                DisplayShotGrid(activePlayer);
                
                //ask player 1 for a shot,
                //determing if it is a valid shot
                //determine shot results
                RecordPlayerShot(activePlayer, opponent);

                //determine is the game is should continue
                bool doesGameContinue = GameLogic.PlayerStillActive(opponent);

                //if over, set activePlayer as the winner 
                //else, swap position(activePlayer to opponent)
                if(doesGameContinue == true)
                {
                    //swap using a temp variable
                    PlayerInfoModel tempHolder = opponent;
                    opponent = activePlayer;
                    activePlayer = tempHolder;

                    //use tuple instead swap temp variable method
                    //(PlayerInfoModel opponent, PlayerInfoModel activePlayer) p = (opponent, activePlayer);
                    //(activePlayer, opponent) = p;
                }
                else
                {
                    winner = activePlayer;
                }

            } while (winner == null);

            IdentifyWinner(winner);

            Console.ReadLine();
        }

        private static void IdentifyWinner(PlayerInfoModel winner)
        {
            Console.WriteLine($"Congratulations to {winner.UserName} for winning!");
            Console.WriteLine($"{winner.UserName} took {GameLogic.GetShotCount(winner) } shots.");
        }

        private static void RecordPlayerShot(PlayerInfoModel activePlayer, PlayerInfoModel opponent)
        {
            bool isValidShot = false;
            string row = "";
            int column = 0;

            // Asks for a shot (do we ask for "B2")
            //determine what row and column that is - split it apart
            //determine if that is a valid shot
            //go back to the beginning if not a valid shot

            do
            {
                string shot = AskForShot(activePlayer);
                //a tuple
                try
                {
                    (row, column) = GameLogic.SplitShotIntoRowAndColumn(shot);
                    isValidShot = GameLogic.ValidateShot(activePlayer, row, column);
                }
                catch (Exception ex)
                {

               
                    isValidShot = false;
                }    
                
                if(isValidShot == false)
                {
                    Console.WriteLine("Invalide Shot Location. Please try again.");
                }
            } while (!isValidShot);

            //determing shot results
            bool isAHit = GameLogic.IdentifyShotResult(opponent, row, column);

            //record results
            GameLogic.MarkShotResult(activePlayer, row, column, isAHit);

            //display the shot results
            DisplayShotResults(row, column, isAHit);
        }

        private static void DisplayShotResults(string row, int column, bool isAHit)
        {
            if (isAHit)
            {
                Console.WriteLine($"{row}{column} is a Hit!");
            }
            else
            {
                Console.WriteLine($"{row}{column} is a miss!");
            }

            Console.WriteLine();
        }

        private static string AskForShot(PlayerInfoModel player)
        {
            Console.Write($"{player.UserName}, please enter your shot selection: ");
            string output = Console.ReadLine();

            return output;
        }

        private static void DisplayShotGrid(PlayerInfoModel activePlayer)
        {
            string currentRow = activePlayer.ShotGrid[0].SpotLetter;

            foreach (var gridSpot in activePlayer.ShotGrid)
            {
                if (gridSpot.SpotLetter != currentRow)
                {
                    Console.WriteLine();
                    currentRow = gridSpot.SpotLetter;
                }
                if (gridSpot.Status == GridSpotStatus.Empty)
                {
                    Console.Write($" {gridSpot.SpotLetter}{gridSpot.SpotNumber} ");
                }
                else if (gridSpot.Status == GridSpotStatus.Hit)
                {
                    Console.Write(" X  ");
                }
                else if (gridSpot.Status == GridSpotStatus.Miss)
                {
                    Console.Write(" O  ");
                }
                else
                {
                    Console.Write(" ?  ");
                }
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        private static void WelcomeMessage()
        {
            Console.WriteLine("Welcome to Battleship Lite");
            Console.WriteLine("Created by Vu Pham");
            Console.WriteLine();
        }

        private static PlayerInfoModel CreatePlayer(string playerTitle)
        {
            PlayerInfoModel output = new PlayerInfoModel();

            Console.WriteLine($"Player information for {playerTitle}");

            //ask the user for their name
            output.UserName = AskForUsersName();    

            //load up the shot grid
            GameLogic.InitializeGrid(output);

            //ask the user for their 5 ship placements
            PlaceShips(output);

            //clear
            Console.Clear();

            return output;
        }

        private static string AskForUsersName()
        {
            Console.Write("What is your name: ");
            string output = Console.ReadLine();

            return output;
        }

        private static void PlaceShips(PlayerInfoModel model)
        {
            do
            {
                Console.Write($"Where do you want to place ship number {model.ShipLocations.Count + 1}: ");
                string location = Console.ReadLine();

                bool isValidLocation = false;

                try
                {
                    isValidLocation = GameLogic.PlaceShip(model, location);
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Error: " + ex.Message);
                }

                if (isValidLocation == false)
                {
                    Console.WriteLine("That was not a valid location. Please try again.");
                }

            } while (model.ShipLocations.Count < 5);
        }
    }
}
