using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NEA
{
    class HostGame : Game
    {
        //this class is for the host
        
        private string ListOfWordsAsString;
        private string BoardAsString;
        private int duration = 20000;
        
        public HostGame(string username, string boardName) : base(username, boardName)
        {
            base.username = username;
            base.boardName = boardName;
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!"); //throws this error message in return if there isn't a network address

        }

        private static void TimerCallback(Object o)
        {
            //Set attemptmade to False when this method is called.
            attemptMade = false;
        }

        public override void playGame()
        {
            Console.Clear();
            int playerTurn = 1;
            bool gameOver = false;
            IPAddress ipAd = null;
            ASCIIEncoding asen = new ASCIIEncoding();
            TcpListener myList = null;

            Board NewBoard = new Board(username, boardName);
            NewBoard.SetHeight(dimensions);
            NewBoard.SetWidth(dimensions);
            NewBoard.GenerateRandomLetters();
            NewBoard.GenerateGameBoard();
            NewBoard.PlaceRandomWords();
            this.BoardAsString = NewBoard.GetBoardAsString();
            
            foreach (string word in ListOfWords)
            {
                ListOfWordsAsString += word + ":";
            }

            try
            {
                ipAd = IPAddress.Parse(GetLocalIPAddress());

                myList = new TcpListener(ipAd, 8001);

                myList.Start();

                Console.WriteLine("The local End point is : " + myList.LocalEndpoint);
                Console.WriteLine("Waiting for a connection...");
                
                Socket s = myList.AcceptSocket();

                Console.Clear();

                Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);

                Console.WriteLine("");

                NewBoard.DrawBoard();

                string transmit = Convert.ToInt32(dimensions) + ";" +  BoardAsString + ";" + username + ";" + boardName + ";" + ListOfWordsAsString;

                s.Send(asen.GetBytes(transmit));

                s.Close();
                myList.Stop();


            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.Message);
            }

            while (gameOver == false)
            {
                attemptMade = true;

                if (playerTurn == 1) //host is player 1
                {
                    myList.Start();
                    Socket s2 = myList.AcceptSocket();

                    Console.WriteLine("");
                    Console.WriteLine("Current score: " + score);
                    Console.WriteLine("");
                    Console.WriteLine("Words left: ");
                    Console.WriteLine("");

                    foreach (string word in ListOfWords)
                    {
                        Console.WriteLine(word);
                    }

                    int x = -1;
                    int y = -1;
                    int x2 = -1;
                    int y2 = -1;

                    Console.WriteLine(""); Console.WriteLine("Your turn");

                    _timer = new Timer(TimerCallback, null, duration, duration);

                    bool validInput = false;
                    string input = "";

                    while (validInput == false)
                    {
                        try
                        {
                            Console.WriteLine("");
                            Console.WriteLine((duration / 1000) + " seconds to find a word.");
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

                    if (attemptMade == true)
                    {
                        score += CalculateScore(NewBoard.WordFound(x, y, x2, y2));
                    }


                    if (attemptMade == false)
                    {
                        Console.WriteLine("Too late. 10 points lost.");
                        _timer.Dispose();
                        Console.ReadLine();

                        score -= 10;

                        Console.Clear();
                        x = -1;
                        y = -1;
                        x2 = -1;
                        y2 = -1;
                        NewBoard.DrawBoard();
                    }

                    else if (NewBoard.WordFound(x, y, x2, y2) == "") //if word highlighted is not a word
                    {
                        Console.WriteLine("No word found. " + NewBoard.wordNotFound(x, y, x2, y2).Length + " points lost.");
                        _timer.Dispose();
                        Console.ReadLine();

                        score -= NewBoard.wordNotFound(x, y, x2, y2).Length;

                        Console.Clear();
                        x = -1;
                        y = -1;
                        x2 = -1;
                        y2 = -1;
                        NewBoard.DrawBoard();
                    }

                    else
                    {
                        Console.WriteLine("Word found. " + NewBoard.WordFound(x, y, x2, y2).Length + " points gained.");
                        _timer.Dispose();
                        Console.ReadLine();

                        Console.Clear();
                        NewBoard.ChangeColours(y, x, y2, x2);
                        NewBoard.DrawBoard();
                    }


                    playerTurn = 2;
                    BoardAsString = NewBoard.GetBoardAsString();

                    BoardAsString += ";"; //for the CSV

                    ListOfWordsAsString = ""; //clearing it

                    foreach (string word in ListOfWords)
                    {
                        ListOfWordsAsString += word + ":"; //the colon is so we can use the .Split() to get into a list quickly
                    }

                    BoardAsString = BoardAsString + ListOfWordsAsString;  //have to send the list of words as well, so it can be
                                                                          //displayed to the user
                    s2.Send(asen.GetBytes(BoardAsString));

                    s2.Close();
                    myList.Stop();
                    
                    if (duration > 10000)
                    {
                        duration -= 2000; //shortening timer
                    }

                }

                else if (playerTurn == 2) //client is player 2
                {

                    myList.Start();

                    Console.WriteLine("");
                    Console.WriteLine("Current score: " + score);

                    Console.WriteLine("");
                    Console.WriteLine("Words left: ");
                    Console.WriteLine("");

                    foreach (string word in ListOfWords)
                    {
                        Console.WriteLine(word);
                    }

                    Console.WriteLine(""); Console.WriteLine("Waiting for Player 2's response...");

                    Socket s2 = myList.AcceptSocket();

                    byte[] b = new byte[1000000];
                    int k = s2.Receive(b);

                    BoardAsString = "";

                    for (int i = 0; i < k; i++)
                    {
                        BoardAsString += Convert.ToChar(b[i]);
                    }

                    string[] receivedText = BoardAsString.Split(';');

                    BoardAsString = receivedText[0];
                    ListOfWordsAsString = receivedText[1];

                    ListOfWords = receivedText[1].Split(':').ToList();

                    NewBoard.SetHeight(20);
                    NewBoard.SetWidth(20);
                    NewBoard.ChangeColorsOnline(BoardAsString);
                    Console.Clear();
                    NewBoard.DrawBoard();

                    playerTurn = 1;

                    s2.Close();
                    myList.Stop();

                }

                if (CheckIfGameOver() == true)
                {
                    gameOver = true;
                }


            }

            //Time to send scores to each other

            string ReceivedText = "";

            try
            {
                ipAd = IPAddress.Parse(GetLocalIPAddress());

                myList = new TcpListener(ipAd, 8001);

                myList.Start();

                Socket s = myList.AcceptSocket();

                Console.WriteLine("");


                byte[] b = new byte[100];
                int k = s.Receive(b);

                for (int i = 0; i < k; i++)
                {
                    ReceivedText += Convert.ToChar(b[i]);
                }

                s.Send(asen.GetBytes(Convert.ToString(score)));

                s.Close();
                myList.Stop();


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            int OpponentScore = Convert.ToInt32(ReceivedText);

            Console.WriteLine("");
            Console.WriteLine("Game over. All words found.");
            Console.WriteLine("");
            Console.WriteLine("Your score: " + score);
            Console.WriteLine("Opponent score: " + OpponentScore);
            Console.WriteLine("");

            if (OpponentScore > score)
            {
                Console.WriteLine("You lose!"); //this writes to the user that they lose
            }
            else if (OpponentScore == score)
            {
                Console.WriteLine("Draw"); //this writes to the user that they drew
            }
            else
            {
                Console.WriteLine("You win!"); //this writes to the user that they win
            }


        }

        public bool CheckIfGameOver() //This checks through every word in ListOfWords. If all are empty, return true so game ends.
        {                             //If any aren't empty, return false and game continues.           
            
            foreach (string word in ListOfWords)
            {
                if (word != "")
                {
                    return false;
                }
            }

            return true;
        }
    }
}
