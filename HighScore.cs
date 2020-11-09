using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HangmanGameProject
{
    public class HighScore : IStringModifier
    {
        private readonly string _path;
        private List<HighScoreRecord> _highScoreRecords;
        private const int MaxRecordsToStore = 10;

        public HighScore( string path )
        {
            if ( string.IsNullOrWhiteSpace( path ) )
                throw new IOException( "Path is null, empty or contains only white spaces\n." +
                                       $"Your path: {path}\n" +
                                       "Give correct path to the program." );

            _highScoreRecords = new List<HighScoreRecord>();

            if ( !File.Exists( path ) )
            {
                File.Create( path );
            }
            else
            {
                _path = path;
                _highScoreRecords = LoadHighScore();
            }
        }

        private List<HighScoreRecord> LoadHighScore()
        {
            var highScoreFromFile = File.ReadAllLines( _path );

            if ( highScoreFromFile.Length < 1 )
            {
                Console.WriteLine( "Scoreboard is empty" );
                return null;
            }

            var highScore = new List<HighScoreRecord>( highScoreFromFile.Length );

            foreach ( var record in highScoreFromFile )
            {
                var detailedRecord = record.Split( "|" );

                try
                {
                    highScore.Add( new HighScoreRecord(
                            detailedRecord[0].Trim(),
                            detailedRecord[1].Trim(),
                            Convert.ToInt32( detailedRecord[2] ),
                            Convert.ToInt32( detailedRecord[3] ),
                            detailedRecord[4].Trim()
                        )
                    );
                }
                catch ( Exception exception ) when
                (
                    exception is FormatException || exception is IndexOutOfRangeException
                )
                {
                    // All records must have form:
                    // name | date | guessing_time | guessing_tries | guessed_word
                    // That means every record (line) must have exactly 4 pipes
                    Console.WriteLine( "Data stored in file is not coded in a proper way.\n" +
                                       "Every record should be coded like:\n" +
                                       "name | date | guessing_time | guessing_tries | guessed_word\n" +
                                       $"Error found in line: {record}\n" );
                    Console.WriteLine( exception.StackTrace );
                }
            }

            return highScore;
        }

        private static void ShowScoreBoardHeader()
        {
            // formatting headers of the scoreboard
            Console.WriteLine( "                         |                    | Guessing | Guessing |   Guessed\n" +
                               "      Best players       |   Date of winning  |   Time   |  Tries   |    Word" );
            Console.WriteLine( new string( '-', 85 ) ); // 85 is just a value to create long enough separator line
        }

        public void ShowScoreBoard()
        {
            if ( _highScoreRecords == null ) return;

            ShowScoreBoardHeader();

            var placeCounter = 1;
            foreach ( var record in _highScoreRecords )
            {
                Console.WriteLine( $"{placeCounter,2}. {record}" );
                placeCounter++;
            }
        }

        public void ShowScoreBoard( HighScoreRecord newRecord )
        {
            if ( _highScoreRecords == null ) return;

            ShowScoreBoardHeader();

            var placeCounter = 1;
            foreach ( var record in _highScoreRecords )
            {
                if ( record.Equals( newRecord ) )
                    IStringModifier.WriteHighlightText( $"{placeCounter,2}. {record}" );
                else
                    Console.WriteLine( $"{placeCounter,2}. {record}" );
                placeCounter++;
            }
        }

        public void Update( HighScoreRecord newRecord )
        {
            _highScoreRecords ??= new List<HighScoreRecord>();
            _highScoreRecords.Add( newRecord );
            // records are sorted by guessing time first and guessing tries second
            _highScoreRecords.Sort();
        }

        /// <summary>
        /// Removes any record above MaxRecordsToStore limit from high score
        /// (worst first - list is sorted after every Update() )
        /// </summary>
        public void TrimRecords()
        {
            if ( _highScoreRecords == null ) return;
            while ( _highScoreRecords.Count > MaxRecordsToStore )
                _highScoreRecords.RemoveAt( MaxRecordsToStore );
        }

        /// <summary>
        /// Overwrites high score in file under the _path directory
        /// </summary>
        public void UpdateFile()
        {
            var stringBuilder = new StringBuilder();
            foreach ( var record in _highScoreRecords )
            {
                stringBuilder.AppendLine( record.ToString() );
            }

            File.WriteAllText( _path, stringBuilder.ToString() );
        }
    }
}