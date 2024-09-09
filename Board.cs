using System;
using System.Collections.Generic;
using System.Xml;

namespace NEA
{
    class Board
    {
        private int Width;
        private int Height;
        private int HorV;
        private int x;
        private int y;
        private int dimensions;
        private string username;
        private string boardName;
        List<Tile> AllLetters =  new List<Tile>();
        Tile[,] GameBoard;

        public Board(string usernameIn, string boardNameIn)
        {
            this.username = usernameIn;
            this.boardName = boardNameIn;
        }

        public void SetHeight(int Height)
        {
            this.Height = Height;
        }

        public void SetWidth(int Width)
        {
            this.Width = Width;
        }

        public void GenerateRandomLetters()
        {
            Random RNoGen = new Random();

            ConsoleColor LetterColour = ConsoleColor.White;

            int LetterNumber;
            Char Letter;

            for (int i = 0; i < (this.Width * this.Height); i++)
            {
                LetterNumber = RNoGen.Next(65, 90);
                Letter = Convert.ToChar(LetterNumber);
                Tile newLetter = new Tile(Letter);
                newLetter.SetColour(LetterColour);
                this.AllLetters.Add(newLetter);
            }
        }

        public void GenerateGameBoard() //Adding all of the random letters to the board first
        {
            GameBoard = new Tile[this.Width, this.Height];

            int counter = 0;

            for (int i = 0; i < this.Height; i++)
            {
                for (int j = 0; j < this.Width; j++)
                {
                    this.GameBoard[i, j] = this.AllLetters[counter];
                    counter += 1;
                }
            }
        }

        public void GenerateOnlineBoard(string BoardAsString) //This generates the board when it is being received by someone else
        {                                                     //It reads the CSV file and sets each tile to the corresponding letter in the CSV
            GameBoard = new Tile[this.Width, this.Height];

            Tile TempTile;

            int counter = 0;

            for (int i = 0; i < this.Height; i++)
            {
                for (int j = 0; j < this.Width; j++)
                {
                    TempTile = new Tile(BoardAsString[counter]);
                    this.GameBoard[i, j] = TempTile;
                    counter += 3;
                }
            }
        }

        public void ChangeColorsOnline(string BoardAsString) //This method changes the colour of the board when the other player sends their game state
        {                                                    //It does this by reading the CSV file which has colours, and setting each tile the appropriate colour
            int counter = 0;

            string[] boardArray = BoardAsString.Split(',');

            Dictionary<string, ConsoleColor> colours = new Dictionary<string, ConsoleColor>();

            colours.Add("1", ConsoleColor.White); //giving each colour a number value
            colours.Add("2", ConsoleColor.Red);
            colours.Add("3", ConsoleColor.Green);

            for (int i = 0; i < this.Height; i++)
            {
                for (int j = 0; j < this.Width; j++)
                {
                    this.GameBoard[i, j].SetColour(colours[Convert.ToString(boardArray[counter][1])]); 
                    counter += 1;
                }
            }
        }

        public void DrawBoard()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            for (int i = 1; i<= this.Width; i++) //This writes out the column numbers
            {
                if (i > 9)
                {
                    Console.Write(i + " ");
                }
                else
                {
                    Console.Write("0" + i + " ");
                }
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;

            for (int i = 0; i < this.Height; i++)
            {
                for (int j = 0; j < this.Width; j++)
                {
                    Console.ForegroundColor = this.GameBoard[i, j].GetColour();
                    Console.Write(this.GameBoard[i, j].GetLetter() + "  ");
                }                

                Console.ForegroundColor = ConsoleColor.Yellow;
                if (i < 9) //this writes out the row numbers
                {
                    Console.Write("0" + (i + 1));
                }
                else
                {
                    Console.Write(i + 1);
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
            }
        }

        public void ChangeColours(int y, int x, int y2, int x2) //colour the letters the user has selected
        {
            if (y == y2)
            {
                for (int j = x; j <= x2; j++)
                {                    
                    GameBoard[y - 1, j - 1].SetColour(ConsoleColor.Green);                    
                }
            }

            if (x == x2)
            {
                for (int j = y; j <= y2; j++)
                {
                   GameBoard[j - 1, x - 1].SetColour(ConsoleColor.Green);
                }
            }
        }

        public void PlaceRandomWords()
        {
            XmlDocument doc = ImportXML.Load();
            
            XMLStream xmlstream = new XMLStream();
            List<string> ListOfWords = new List<string>();
            
            xmlstream.TraverseNodes(doc.ChildNodes, ref ListOfWords, ref dimensions, username, boardName);

            Random random = new Random();

            for (int i = 0; i < ListOfWords.Count; i ++)
            {
                bool validRandom = false; 
                while (validRandom == false) //making sure the word will fit on the board
                {
                    HorV = random.Next(0, 2);
                    x = random.Next(1, dimensions);
                    y = random.Next(1, dimensions);

                    if (x > dimensions || y > dimensions)
                    {
                        validRandom = false;
                    }

                    else if (x + ListOfWords[i].Length > dimensions && HorV == 0)
                    {
                        validRandom = false;
                    }

                    else if (y + ListOfWords[i].Length > dimensions && HorV == 1)
                    {
                        validRandom = false;
                    }

                    else if (check(ListOfWords[i]) == false)
                    {
                        validRandom = false;
                    }

                    else
                    {
                        validRandom = true;
                    }
                }

                PlaceWord(HorV, ListOfWords[i]);
            }
        }

        public bool check(string word)
        {
            for (int i = 0; i < word.Length; i ++)
            {
                if (HorV == 0)
                {
                    int index = x + i;
                    if (GameBoard[y, index].getCheck() == "taken" && GameBoard[y, index].GetLetter() != word[i])
                    {
                        return false;
                    }
                }
                else if (HorV == 1)
                {
                    int index = y + i;
                    if (GameBoard[index, x].getCheck() == "taken" && GameBoard[index, x].GetLetter() != word[i])
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }

        public void PlaceWord(int HorV, string word) //places words
        {
            int index = 0;
            if (HorV == 0)
            {
                //place horizontally
                for (int i = x; i < x + word.Length; i++)
                {
                    Tile AddLetter = new Tile(word[index]);
                    GameBoard[y, i] = AddLetter;
                    GameBoard[y, i].setCheck("taken");
                    index++;
                }
            }

            else if (HorV == 1)
            {
                //place vertically
                for (int i = y; i < y + word.Length; i++)
                {
                    Tile AddLetter = new Tile(word[index]);
                    GameBoard[i, x] = AddLetter;
                    GameBoard[i, x].setCheck("taken");
                    index++;
                }
            }
        }

        public string WordFound(int x, int y, int x2, int y2)
        {
            XmlDocument doc = ImportXML.Load();

            XMLStream xmlstream = new XMLStream();
            List<string> ListOfWords = new List<string>();

            xmlstream.TraverseNodes(doc.ChildNodes, ref ListOfWords, ref dimensions, username, boardName);

            string checkWord = "";

            if (y == y2)
            {
                for (int i = x; i <= x2; i++)
                {
                    checkWord = checkWord + GameBoard[y - 1, i - 1].GetLetter();
                }
            }

            if (x == x2)
            {
                for (int i = y; i <= y2; i++)
                {
                    checkWord = checkWord + GameBoard[i - 1, x - 1].GetLetter();
                }
            }

            for (int i = 0; i < ListOfWords.Count; i++)
            {
                if (ListOfWords[i] == checkWord)
                {
                    if (y == y2)
                    {
                        for (int j = x; j <= x2; j++)
                        {
                            GameBoard[y - 1, j - 1].setCheck("found");
                        }
                    }

                    if (x == x2)
                    {
                        for (int j = y; j <= y2; j++)
                        {
                            GameBoard[j - 1, x - 1].setCheck("found");
                        }
                    }

                    return ListOfWords[i];
                }
            }

            return "";
        }

        public string GetBoardAsString() //converting board into a csv
        {
            //temp
            string BoardAsString = "";

            Dictionary<ConsoleColor, string> colours = new Dictionary<ConsoleColor, string>();

            colours.Add(ConsoleColor.White, "1"); //giving each colour a number value
            colours.Add(ConsoleColor.Red, "2");
            colours.Add(ConsoleColor.Green, "3");

            for (int i = 0; i < this.Width; i++)
            {
                for (int j = 0; j < this.Height; j++)
                {
                    BoardAsString += GameBoard[i, j].GetLetter() + colours[GameBoard[i, j].GetColour()] + ",";
                }
            }
            return BoardAsString;
        }

        public string wordNotFound(int x, int y, int x2, int y2) //turning word red if word selected isn't a word
        {                                                        //also penalizes the user
            string word = "";
            
            if (y == y2)
            {
                for (int j = x; j <= x2; j++)
                {
                    if (GameBoard[y - 1, j - 1].getCheck() != "found")
                    {
                        GameBoard[y - 1, j - 1].SetColour(ConsoleColor.Red);
                    }

                    word = word + GameBoard[y - 1, j - 1].GetLetter();

                }


            }

            if (x == x2)
            {
                for (int j = y; j <= y2; j++)
                {
                    if (GameBoard[j - 1, x - 1].getCheck() != "found")
                    {
                        GameBoard[j - 1, x - 1].SetColour(ConsoleColor.Red);
                    }

                    word = word + GameBoard[j - 1, j - 1].GetLetter();
                }
            }

            return word;

        }

    }
}
