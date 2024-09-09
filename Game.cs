using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Threading;

namespace NEA
{
    class Game
    {
        protected int score;
        protected List<string> ListOfWords;
        protected int dimensions;
        protected string username;
        protected string boardName;
        protected static Timer _timer = null;
        protected static bool attemptMade;
        public Game(string username, string boardName)
        {
            this.username = username;
            this.boardName = boardName;
        }
    
        public void setListOfWords(List<string> ListOfWordsIn)
        {
            ListOfWords = ListOfWordsIn;
            XmlDocument doc = ImportXML.Load();
            XMLStream xmlstream = new XMLStream();
            xmlstream.TraverseNodes(doc.ChildNodes, ref ListOfWords, ref dimensions, username, boardName);
        }

        public virtual void playGame()
        {
            Console.Clear();

            int letterCount = 0;
            int x = -1;
            int y = -1;
            int x2 = -1;
            int y2 = -1;

            for (int i = 0; i < ListOfWords.Count(); i++)
            {
                letterCount += ListOfWords[i].Length;
            }

            Board NewBoard = new Board(username,boardName);
            NewBoard.SetHeight(dimensions);
            NewBoard.SetWidth(dimensions);
            NewBoard.GenerateRandomLetters();
            NewBoard.GenerateGameBoard();
            NewBoard.PlaceRandomWords();
            NewBoard.DrawBoard();


            while (ListOfWords.Count > 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Words left: ");
                for (int i = 0; i < ListOfWords.Count(); i++)
                {
                    Console.WriteLine(ListOfWords[i]);
                }

                bool validInput = false;
                string input = "";

                while (validInput == false)
                {

                    try
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Find a word.");
                        Console.WriteLine("");
                        Console.WriteLine("Enter the co ordinate of the first letter (row followed by column with a space inbetween).");

                        input = (Console.ReadLine());

                        string[] RowCol = input.Split(' ');
                        y = Convert.ToInt32(RowCol[0]);
                        x = Convert.ToInt32(RowCol[1]);

                        Console.WriteLine("");
                        Console.WriteLine("Enter the co ordinate of the last letter.");
                        string input2 = (Console.ReadLine());
                        string[] RowCol2 = input2.Split(' ');
                        y2 = Convert.ToInt32(RowCol2[0]);
                        x2 = Convert.ToInt32(RowCol2[1]);

                        if (x != x2 && y != y2) //if user enters word wrong
                        {
                            Console.WriteLine("Word must be a straight line.");
                        }

                        else if (x > dimensions || x2 > dimensions || y > dimensions || y2 > dimensions)
                        {
                            Console.WriteLine("Co ordinate is not on the board");
                        }

                        else if (x < 1 || x2 < 1 || y < 1 || y2 < 1)
                        {
                            Console.WriteLine("Co ordinate is not on the board");
                        }

                        else
                        {
                            validInput = true;
                        }

                    }

                    catch
                    {
                        Console.WriteLine("Co-ordinate entered incorrectly.");
                    }
                }

                CalculateScore(NewBoard.WordFound(x, y, x2, y2)); //score not needed for offline, but the method removes the word

                if (NewBoard.WordFound(x,y,x2,y2) == "") //if word highlighted is not a word
                {
                    score -= NewBoard.wordNotFound(x,y,x2,y2).Length;
                    Console.Clear();
                    x = -1;
                    y = -1;
                    x2 = -1;
                    y2 = -1;
                    NewBoard.DrawBoard();
                }

                else
                {
                    Console.Clear();
                    NewBoard.ChangeColours(y, x, y2, x2);
                    NewBoard.DrawBoard();
                }

            }

            Console.WriteLine("All words found. Well done!");
        }

        public int CalculateScore(string word) //This sees if the word is in the ListOfWords, and calculates how many points
        {                                      //it is worth based off the length of the word.
            foreach (string WordInList in ListOfWords)
            {
                if (WordInList == word) 
                {
                    ListOfWords.Remove(WordInList); //This method also removes the word from the list, for when it is displayed
                    return word.Count();            //to the user.
                }
            }

            return 0;
        }
    }
}
