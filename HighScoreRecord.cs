﻿using System;

 namespace HangmanGameProject
{
    public class HighScoreRecord : IComparable<HighScoreRecord>
    {
        private readonly string _name;
        private readonly string _date;
        private readonly int _guessingTime;
        private readonly int _tries;
        private readonly string _guessingWord;

        public HighScoreRecord( string name, string date, int guessingTime, int tries, string guessingWord )
        {
            _name = name;
            _date = date;
            _guessingTime = guessingTime;
            _tries = tries;
            _guessingWord = guessingWord;
        }

        public int CompareTo( HighScoreRecord other )
        {
            if ( ReferenceEquals( this, other ) ) return 0;
            if ( ReferenceEquals( null, other ) ) return 1;

            if ( _guessingTime == other._guessingTime )
                return _tries - other._tries;
            return _guessingTime - other._guessingTime;
        }

        private bool Equals( HighScoreRecord other )
        {
            return _name == other._name
                   && _date == other._date
                   && _guessingTime == other._guessingTime
                   && _tries == other._tries
                   && _guessingWord == other._guessingWord;
        }

        public override bool Equals( object obj )
        {
            if ( ReferenceEquals( null, obj ) )
                return false;
            if ( ReferenceEquals( this, obj ) )
                return true;

            return obj.GetType() == GetType() && Equals( (HighScoreRecord) obj );
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( _name, _date, _guessingTime, _tries, _guessingWord );
        }

        public override string ToString()
        {
            // pipes are crucial to read/write records from/to file
            // formatting is used to create a table
            return $"{_name,-20} {"|",-4}" +
                   $"{_date} {"|",-4}" +
                   $"{_guessingTime,4}\t {"|",-4}" +
                   $"{_tries,4}   {"|",-4}" +
                   $"{_guessingWord}";
        }
    }
}