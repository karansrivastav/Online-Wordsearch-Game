using System;
using System.Collections.Generic;
using System.Xml;

namespace NEA
{
    class CustomBoard
    {
        private int dimensions;
        private string username;
        private string boardName;

        public CustomBoard(string usernameIn)
        {
            this.username = usernameIn;
        }

        public void createBoard()
        {
            Console.Clear();
            bool validInput = false;
            bool nameExists = true;

            List<string> ListOfWords = new List<string>();
            XmlDocument doc = ImportXML.Load();
            XMLStream xmlstream = new XMLStream();

            while (nameExists == true)
            {
                nameExists = false;

                Console.WriteLine("Enter the name of your board");
                boardName = Console.ReadLine();

                xmlstream.BoardNameExist(doc.ChildNodes, boardName, ref nameExists);

                if (boardName.Length == 0) //if the user has entered nothing for their board
                {
                    nameExists = true;
                }

            }
            
            while (validInput == false)
            {
                try
                {
                    Console.WriteLine("Enter width/height between 15 and 30");
                    dimensions = Convert.ToInt32(Console.ReadLine());
                    //wordsearch must be a square for it to work

                    if (dimensions >= 15 && dimensions <= 30)
                    {
                        validInput = true;
                    }

                    else
                    {
                        Console.WriteLine("Enter a number between 15 and 30");
                    }
                }

                catch
                {
                    Console.WriteLine("Enter a number");
                }
            }
            Console.Clear();

            double letterCount = Math.Floor((dimensions * dimensions) * 0.1);
            //need most letters in the board to be random, so have only have 1/10 of the board actual words

            while (letterCount > 0) //making the user enter the words they want
            {
                bool validWord = false;
                string word = "";

                while (validWord == false)
                {
                    bool specialCharacters = false;
                    Console.WriteLine("Enter a word you want to be in the wordsearch");
                    word = Console.ReadLine();
                    word = word.ToLower();

                    try
                    {
                        for (int i = 0; i < word.Length; i++)
                        {
                            int ascii = Convert.ToInt32(word[i]);

                            if (ascii < 97 || ascii > 122)
                            {
                                specialCharacters = true;
                            }
                        }

                        if (word.Length <= letterCount && word.Length < dimensions && specialCharacters == false)
                        {   //this makes sure the word isn't too big for the board and the word doesn't have special characters
                            validWord = true;
                        }

                        if (ListOfWords.Contains(word.ToUpper()) == true)  //no repeating words
                        {
                            Console.WriteLine("Word already in board.");
                            validWord = false;
                        }

                        else if (word.Length < 3)
                        {   //if word is too short it will be hard to tell if the word is actually the word or just 2 random letters together
                            Console.WriteLine("Word too short");
                            validWord = false;
                        }

                        else
                        {
                            Console.WriteLine("Your word is too long or has special characters");
                        }

                    }

                    catch
                    {
                        Console.WriteLine("Invalid input");
                    }
                    
                }
        
                int wordValue = word.Length;
                letterCount -= wordValue;
                ListOfWords.Add(word.ToUpper());
                Console.Clear();

                if (letterCount >= 3)
                {
                    Console.WriteLine("Characters left = " + letterCount); //making sure user fills wordsearch fully
                }

                if (letterCount < 3)
                {
                    letterCount = 0;
                }
            }

            Console.WriteLine("Board created.");
            
            XmlNode Board = doc.CreateElement("BOARD");
            XmlNode Dimensions = doc.CreateElement("DIMENSIONS");
            XmlNode CreatorName = doc.CreateElement("CREATOR");
            XmlNode BoardName = doc.CreateElement("BOARDNAME");
            CreatorName.InnerText = username;
            Board.AppendChild(CreatorName);

            BoardName.InnerText = boardName;
            Board.AppendChild(BoardName);
            Dimensions.InnerText = Convert.ToString(dimensions);
            Board.AppendChild(Dimensions);
            doc.Save("GameFile.xml");

            for (int i = 0; i < ListOfWords.Count; i++)
            {
                XmlNode Addedword = doc.CreateElement("Word");
                Addedword.InnerText = ListOfWords[i];
                Board.AppendChild(Addedword);
                doc.DocumentElement.AppendChild(Board);
                doc.Save("GameFile.xml");
            }

        }
    }
}
