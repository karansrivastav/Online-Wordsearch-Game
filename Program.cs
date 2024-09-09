using System;
using System.Collections.Generic;
using System.Linq;

namespace NEA
{
    class Program
    {
        static void Main(string[] args)
        {
            login login = new login();
            List<string> ListOfWords = new List<string>();
            bool gameOver = false;

            while (gameOver == false)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("*************************************");
                Console.WriteLine("*             Wordsearch            *");
                Console.WriteLine("*                                   *");
                Console.WriteLine("*      1:     Join Game             *");
                Console.WriteLine("*      2:     Play Game (Solo)      *");
                Console.WriteLine("*      3:     Host Game             *");
                Console.WriteLine("*      4:     Create Board          *");
                Console.WriteLine("*      5:     Create account        *");
                Console.WriteLine("*      6:     Login                 *");
                Console.WriteLine("*                                   *");
                Console.WriteLine("*************************************");
                string userChoice = Console.ReadLine();

                if (userChoice == "1")
                {
                    //join another game
                    JoinGame joinGame = new JoinGame(login.getUsername(), "");
                    joinGame.playGame();
                    Console.ReadLine();
                }

                if (userChoice == "2")
                {   //play by yourself

                    if (login.signIn() == false)
                    {
                        Console.WriteLine("User must be signed in.");
                    } 

                    else
                    {
                        Console.WriteLine("Enter name of board you would like to use: ");
                        string boardName = Console.ReadLine();

                        Game newGame = new Game(login.getUsername(), boardName);
                        newGame.setListOfWords(ListOfWords);

                        if (ListOfWords.Count() == 0) //checking to see if the user has created a board
                        {
                            Console.WriteLine("Board does not exist.");
                        }

                        else
                        {
                            newGame.playGame();
                            gameOver = true;
                        }
                    }
                    Console.ReadLine();
                }

                if (userChoice == "3")
                {
                    //host a game

                    if (login.signIn() == false)
                    {
                        Console.WriteLine("User must be signed in.");
                    }

                    else
                    {
                        Console.WriteLine("Enter name of board you will be using.");
                        string boardName = Console.ReadLine();


                        HostGame newGame = new HostGame(login.getUsername(), boardName);
                        newGame.setListOfWords(ListOfWords);

                        if (ListOfWords.Count == 0)
                        {
                            Console.WriteLine("Board does not exist.");
                        }
                        else
                        {
                            newGame.playGame();

                        }

                    }

                    Console.ReadLine();

                }

                if (userChoice == "4")
                {
                    if (login.signIn() == false)
                    {
                        Console.WriteLine("User must be signed in");
                    }

                    else
                    {
                        CustomBoard customboard = new CustomBoard(login.getUsername());
                        customboard.createBoard();
                    }
                    Console.ReadLine();

                }

                else if (userChoice == "5")
                {
                    login.createAccount();
                    Console.ReadLine();
                }

                else if (userChoice == "6")
                {
                    Console.WriteLine("Enter your username");
                    login.setUsername(Console.ReadLine());
                    Console.WriteLine("Enter your password");
                    login.setPassword(Console.ReadLine());

                    if (login.signIn() == false)
                    {
                        Console.WriteLine("Incorrect name and/or password.");
                    }
                    else
                    {
                        Console.WriteLine("Welcome back, " + login.getUsername());
                    }
                    Console.ReadLine();

                }

            }

            Console.ReadLine();

        }
    }
}

