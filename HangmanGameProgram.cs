using System;
using System.IO;

namespace HangmanGameProject
{
    internal static class HangmanGameProgram
    {
        public static void Main()
        {
            // My IDE (JetBrains Rider) stores *.exe files in catalogue:
            // %PROJECTFILE\bin\Debug\netcoreapp3.1\
            // so if you keep source files in project directory, you have to get proper path.
            var pathToGuessingWords = GetProjectDirectoryPath() + "\\countries_and_capitals.txt";
            var pathToHighScore = GetProjectDirectoryPath() + "\\highScore.txt";
            
            // If you keep *.exe files in the same directory as source files, comment above lines and use this:
            // var pathToGuessingWords = ".\\countries_and_capitals.txt";
            // var pathToHighScore = ".\\highScore.txt";

            var hangman = new Hangman( 6, 3, pathToGuessingWords );

            LaunchGame( hangman, pathToHighScore );
        }

        private static void LaunchGame( Hangman hangman, string pathToHighScore )
        {
            do
            {
                var gameTime = hangman.Play();
                var highScore = new HighScore( path: pathToHighScore );

                if ( hangman.IsGameWon() )
                {
                    Console.WriteLine( "Congratulations! You Won!\n\n" +
                                       $"You guessed the capital after {hangman.GetTries()} tries. It took you {gameTime} seconds.\n\n"
                                       + "What is your name?" );

                    var playerName = Console.ReadLine();
                    while ( playerName == null || playerName.Contains( '|' ) )
                    {
                        Console.WriteLine( "Player name cannot be empty nor contains pipe '|' character.\n" +
                                           "Please type different name." );
                        playerName = Console.ReadLine();
                    }

                    var newRecord = new HighScoreRecord(
                        playerName,
                        DateTime.Now.ToString( "dd.MM.yyyy HH:mm" ),
                        gameTime,
                        hangman.GetTries(),
                        IStringModifier.ToTitleCase( hangman.GuessingWord )
                    );
                    highScore.Update( newRecord );
                    highScore.ShowScoreBoard( newRecord );
                    highScore.TrimRecords();
                    highScore.UpdateFile();
                }
                else
                {
                    Console.WriteLine( "\nGAME OVER, LOSER!!!" );
                    highScore.ShowScoreBoard();
                    Console.WriteLine( $"\nGame took you {gameTime} seconds to lose.\n" );
                    AskIfRevealAnswer( hangman.GuessingWord );
                }
            } while ( PlayAgain() );
        }

        private static void AskIfRevealAnswer( string guessingWord )
        {
            do
            {
                Console.WriteLine( "Would you like to see the answer? [Y/N]" );
                var tmpKey = Console.ReadKey().KeyChar;
                switch ( tmpKey )
                {
                    case 'y':
                    case 'Y':
                        Console.WriteLine( $"\nThe answer was {guessingWord}.\nGood luck next time!" );
                        return;
                    case 'n':
                    case 'N':
                        return;
                    default:
                        Console.WriteLine( "\nUse only 'y', 'Y', 'n' or 'N' character key." );
                        continue;
                }
            } while ( true );
        }

        private static bool PlayAgain()
        {
            Console.WriteLine();
            do
            {
                Console.WriteLine( "Do you want to play again? [Y/N]" );
                var tmpChar = Console.ReadKey().KeyChar;
                switch ( tmpChar )
                {
                    case 'y':
                    case 'Y':
                        return true;
                    case 'n':
                    case 'N':
                        return false;
                    default:
                        Console.WriteLine( "\nUse only 'y', 'Y', 'n' or 'N' keys to navigate." );
                        continue;
                }
            } while ( true );
        }
        /// <summary>
        /// Return path to the project directory when using IDE
        /// </summary>
        /// <returns>Path to Project directory</returns>
        public static string GetProjectDirectoryPath()
        {
            return Directory.GetParent(
                                 Environment.CurrentDirectory
                             )
                            .Parent
                            .Parent
                            .FullName;
        }
    }
}