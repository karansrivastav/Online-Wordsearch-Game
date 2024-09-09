using System;

namespace NEA
{
    class Tile
    {
        Char Letter;
        ConsoleColor LetterColour = ConsoleColor.White;
        string check;
        
        public Tile(Char Letter)
        {
            this.Letter = Letter;
        }
       
        public Char GetLetter()
        {
            return this.Letter;
        }

        public ConsoleColor GetColour()
        {
            return this.LetterColour;
        }

        public void SetColour(ConsoleColor LetterColour)
        {
            this.LetterColour = LetterColour;
        }

        public void setCheck(string checkIn)
        {
            check = checkIn;
        }

        public string getCheck()
        {
            return check;
        }
    }
}
