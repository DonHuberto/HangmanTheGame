using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static System.IO.File;

namespace HangmanGameProject
{
    public class Hangman
    {
        /// <summary>
        /// ASCII art represents the "states" of hangman.
        /// Additional spaces are only used to improve visibility of code during it's review.
        /// </summary>
        private static readonly string[] HangmanDrawings =
        {
            // full hangman - game over
            "______   \n" +
            "|/   |   \n" +
            "|    o   \n" +
            "|   / \\ \n" +
            "|    |   \n" +
            "|   / \\ \n" +
            "|\n",

            // 1 life remaining
            "______   \n" +
            "|/   |   \n" +
            "|    o   \n" +
            "|   / \\ \n" +
            "|    |   \n" +
            "|   /    \n" +
            "|\n",

            // 2 lives remaining
            "______   \n" +
            "|/   |   \n" +
            "|    o   \n" +
            "|   / \\ \n" +
            "|    |   \n" +
            "|        \n" +
            "|        \n",

            // 3 lives remaining
            "______   \n" +
            "|/   |   \n" +
            "|    o   \n" +
            "|   / \\ \n" +
            "|        \n" +
            "|        \n" +
            "|        \n",

            // 4 lives remaining
            "______   \n" +
            "|/   |   \n" +
            "|    o   \n" +
            "|   /    \n" +
            "|        \n" +
            "|        \n" +
            "|        \n",

            // 5 lives remaining
            "______   \n" +
            "|/   |   \n" +
            "|    o   \n" +
            "|        \n" +
            "|        \n" +
            "|        \n" +
            "|        \n",

            // 6 lives remaining
            "______   \n" +
            "|/   |   \n" +
            "|        \n" +
            "|        \n" +
            "|        \n" +
            "|        \n" +
            "|        \n",
        };

        private readonly int _maxLives;
        private int _remainingLives;
        private int _remainingLetters;
        private char _givenLetter;
        private readonly char[] _usedLetters;
        private readonly List<string> _missedWords;
        private string _hintWord;
        private readonly int _livesToRevealHint;
        private readonly string _pathToGuessingWords;
        private const int AlphabetRange = 'z' - 'a' + 1; // it's 26, however it helps understanding the code

        public string GuessingWord { get; private set; }

        public Hangman( int maxLives, int livesToRevealHint, string pathToGuessingWords )
        {
            if ( maxLives >= HangmanDrawings.Length || maxLives < 1 )
                throw new IndexOutOfRangeException(
                    $"Lives value should be between 1 and {HangmanDrawings.Length - 1}.\nYou put value: {maxLives}." );
            _maxLives = maxLives;
            _livesToRevealHint = livesToRevealHint;
            // there is no need to check value of this variable
            // 0 or less means user doesn't want any hints,
            // maxLives or more means hint is revealed from the beginning

            if ( !Exists( pathToGuessingWords ) )
                throw new FileNotFoundException( $"File not found at current path: {pathToGuessingWords}" );
            _pathToGuessingWords = pathToGuessingWords;

            // _usedLetters is an array representing each letter of alphabet
            // _usedLetters[0] = 'A' ... _usedLetters[ AlphabetRange - 1 ] = 'Z'
            // whenever player chooses a letter, it's index is incremented and revealed in @ShowHiddenWord method
            _usedLetters = new char[AlphabetRange];
            _missedWords = new List<string>();
        }


        private void PrepareNewGame()
        {
            _remainingLives = _maxLives;

            var tmpString = GetWordToGuess()
               .Split( "|" );
            // first word is the name of the country, which capital is to guess
            _hintWord = tmpString[0]
               .Trim();
            // second word is the word to guess
            GuessingWord = tmpString[1]
                          .ToUpper()
                          .Trim();
            _remainingLetters = GuessingWord.Length - GuessingWord.Count( c => c < 'A' || c > 'Z' );

            // clearing arrays and list from data recorded in previous game
            _missedWords.Clear();
            for ( var i = 0; i < _usedLetters.Length; i++ )
            {
                _usedLetters[i] = (char) 0;
            }
        }

        private string GetWordToGuess()
        {
            var records = ReadAllLines( _pathToGuessingWords );

            if ( records.Length <= 0 )
                throw new IOException( "File with words to guess is empty. Please choose another file." );

            var randomInt = new Random();
            var index = randomInt.Next( 0, records.Length - 1 );

            if ( records[index].Count( c => c == '|' ) != 1 )
                throw new IOException( "Data stored in source file are not coded in the proper way.\n" +
                                       "Every record line should be coded like:" +
                                       "country | capital\n" +
                                       $"Error in line {index + 1}: {records[index]}\n" );

            return records[index];
        }

        private void DrawHangman()
        {
            if ( _remainingLives < HangmanDrawings.Length )
                Console.WriteLine(
                    HangmanDrawings[_remainingLives]
                );
        }

        private void ShowUserInterface()
        {
            DrawHangman();
            ShowHiddenWord();
            ShowHint();
            Console.WriteLine( $"Remaining lives: {_remainingLives}" );
            Console.WriteLine( $"Remaining letters: {_remainingLetters}" );

            Console.Write( "Already used letters: " );
            for ( var i = 0; i < _usedLetters.Length; i++ )
            {
                if ( _usedLetters[i] > 0 )
                    Console.Write( $"{(char) (i + 'A')}, " ); // 'A' is equal to 65
            }

            Console.WriteLine();

            if ( _missedWords.Count <= 0 )
                return;
            Console.Write( "Missed guesses: " );
            foreach ( var word in _missedWords )
            {
                Console.Write( $"{word}, " );
            }

            Console.WriteLine();
        }

        private void ShowHiddenWord()
        {
            foreach ( var c in GuessingWord )
            {
                if ( c >= 'A' && c <= 'Z' )
                    Console.Write( _usedLetters[c - 'A'] > 0 ? c : '_' ); // 'A' is equal to 65
                else
                    Console.Write( c );
            }

            Console.WriteLine();
        }

        private void ShowHint()
        {
            if ( _remainingLives <= _livesToRevealHint )
                Console.WriteLine( $"HINT: The capital of {_hintWord}" );
        }

        private bool IsGameOver()
        {
            return IsGameWon() || _remainingLives <= 0;
        }

        public bool IsGameWon()
        {
            return _remainingLetters <= 0;
        }

        private void GuessSingleLetter()
        {
            Console.Write( "Your guess: " );
            _givenLetter = Console.ReadKey().KeyChar;

            if ( _givenLetter >= 'a' && _givenLetter <= 'z' )
                _givenLetter = char.ToUpper( _givenLetter );

            Console.Clear();

            if ( _givenLetter < 'A' || _givenLetter > 'Z' )
                Console.WriteLine( "Typed key is not a letter!\n\n" );
            else
            {
                var keyCharIndex = _givenLetter - 'A'; // 'A' is equal to 65
                _usedLetters[keyCharIndex]++;
                if ( _usedLetters[keyCharIndex] != 1 )
                    return;

                if ( !GuessingWord.Contains( _givenLetter ) )
                    _remainingLives--;
                else
                    _remainingLetters -= GuessingWord.Count(
                        c => c == _givenLetter
                    );
            }
        }

        private void GuessWholeWord()
        {
            // while ( true )
            // {
            Console.Write( "Your guess: " );
            var typedWord = Console.ReadLine()
                                  ?.Trim()
                                   .ToUpper();
            if ( string.IsNullOrWhiteSpace( typedWord ) )
            {
                Console.WriteLine( "Type a proper word" );
                return;
            }

            if ( typedWord.Equals( GuessingWord ) )
                _remainingLetters = 0;
            else
            {
                _remainingLives -= 2;
                _missedWords.Add( typedWord );
            }

            Console.Clear();
            //     break;
            // }
        }

        public int GetTries()
        {
            // counting only single guess of a letter, even if player type it many times
            return _missedWords.Count + _usedLetters.Sum( x => x > 1 ? 1 : x );
        }

        private void GiveUp()
        {
            _remainingLives = -1;
        }

        /// <summary>
        /// Play a new Hangman game and measure the time (in seconds) it takes to finish.
        /// </summary>
        /// <returns> Time of a game in seconds </returns>
        public int Play()
        {
            var stopwatch = new Stopwatch();

            Console.Clear();
            PrepareNewGame();
            stopwatch.Start();
            do
            {
                ShowUserInterface();
                Console.WriteLine(
                    "Would you like to guess a letter [L] or the whole word [W]? ...or give up : [Q]" );
                var tmpChar = Console.ReadKey().KeyChar;
                switch ( tmpChar )
                {
                    case 'l':
                    case 'L':
                        Console.WriteLine();
                        GuessSingleLetter();
                        break;
                    case 'w':
                    case 'W':
                        Console.WriteLine();
                        GuessWholeWord();
                        break;
                    case 'q':
                    case 'Q':
                        Console.Clear();
                        GiveUp();
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine( "Use only 'l', 'L', 'w', 'W', 'q' or 'Q' key.\n" );
                        continue;
                }
            } while ( !IsGameOver() );

            stopwatch.Stop();
            return stopwatch.Elapsed.Seconds;
        }
    }
}