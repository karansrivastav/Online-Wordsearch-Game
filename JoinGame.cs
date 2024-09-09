using System;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace NEA
{
    class JoinGame : Game
    {
        //this class is for the guest

        private string BoardAsString;
        private string ListOfWordsAsString;
        private int duration = 20000;

        public JoinGame(string username, string boardName) : base(username, boardName)
        {
            base.username = username;
        }

        public bool validateIp(string ipaddress) //This method uses regular expressions in order to ensure the entered IP Address is valid
        {
            string ipPattern = @"\d\d?\d?\.\d\d?\d?\.\d\d?\d?\.\d\d?\d?";

            Regex regex = new Regex(ipPattern);

            if (regex.IsMatch(ipaddress) == false)
            {
                return false;
            }
            else
            {
                string[] SplitIp = ipaddress.Split('.');

                foreach (string s in SplitIp)
                {
                    if (Convert.ToInt32(s) > 255)
                    {
                        return false;
                    }
                }             
            }
            return true;
        }

        private static void TimerCallback(Object o)
        {
            //Set attemptmade to False when this method is called.
            attemptMade = false;
        }

        public override void playGame()
        {
            Board NewBoard = new Board(username, boardName);
            string ipaddress = "";
            bool isValid = false;

            while (isValid == false)
            {
                Console.WriteLine("Enter IP Address of player you would like to join : ");

                ipaddress = Console.ReadLine();

                if (validateIp(ipaddress) == false)
                {
                    Console.WriteLine("Invalid IP Address");
                }
                else
                {
                    isValid = true;
                }

            }

            bool gameOver = false;
            int playerTurn = 1;
            ASCIIEncoding asen = new ASCIIEncoding();

            try
            {
                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");

                Thread.Sleep(500);

                tcpclnt.Connect(ipaddress, 8001);

                Console.WriteLine("Connected");

                Stream stm = tcpclnt.GetStream();

                byte[] bb = new byte[1000000];
                int k = stm.Read(bb, 0, 1000000);
                string gameState = "";

                for (int i = 0; i < k; i++)
                    gameState += Convert.ToChar(bb[i]);


                string[] gameStateArray = gameState.Split(';');

                this.dimensions = Convert.ToInt32(gameStateArray[0]);
                this.BoardAsString = gameStateArray[1];
                this.username = gameStateArray[2];
                this.boardName = gameStateArray[3];
                this.ListOfWordsAsString = gameStateArray[4];
                
                ListOfWords = ListOfWordsAsString.Split(':').ToList();

                Console.Clear();

                NewBoard = new Board(this.username, this.boardName);

                NewBoard.SetHeight(this.dimensions);
                NewBoard.SetWidth(this.dimensions);
                NewBoard.GenerateOnlineBoard(this.BoardAsString);
                NewBoard.DrawBoard();

                this.BoardAsString = "";

                tcpclnt.Close();
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

                    Console.WriteLine("");
                    Console.WriteLine("Current score: " + score);
                    Console.WriteLine("");
                    Console.WriteLine("Words left: ");
                    Console.WriteLine("");
                    for (int i = 0; i < ListOfWords.Count(); i++)
                    {
                        Console.WriteLine(ListOfWords[i]);
                    }


                    Console.WriteLine(""); Console.WriteLine("Waiting for Player 1's response..");

                    try
                    {
                        TcpClient tcpclnt2 = new TcpClient();

                        Thread.Sleep(500); 

                        tcpclnt2.Connect(ipaddress, 8001);
                        Stream stm = tcpclnt2.GetStream();

                        byte[] bb = new byte[1000000];
                        int k = stm.Read(bb, 0, 1000000);

                        BoardAsString = "";

                        for (int i = 0; i < k; i++)
                            BoardAsString += Convert.ToChar(bb[i]);

                        string[] receivedText = BoardAsString.Split(';');

                        BoardAsString = receivedText[0];
                        ListOfWordsAsString = receivedText[1];

                        ListOfWords = receivedText[1].Split(':').ToList();

                        NewBoard.ChangeColorsOnline(BoardAsString);
                        Console.Clear();
                        NewBoard.DrawBoard();

                        playerTurn = 2;

                        tcpclnt2.Close();
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                }

                else if (playerTurn == 2)  //client is player 2
                {
                    Console.WriteLine("");
                    Console.WriteLine("Current score: " + score);
                    Console.WriteLine("");
                    Console.WriteLine("Words left: ");
                    Console.WriteLine("");

                    foreach (string word in ListOfWords)
                    {
                        Console.WriteLine(word);
                    }

                    Console.WriteLine(""); Console.WriteLine("Your turn");

                    _timer = new Timer(TimerCallback, null, duration, duration);

                    int x = -1;
                    int y = -1;
                    int x2 = -1;
                    int y2 = -1;

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

                    

                    BoardAsString = NewBoard.GetBoardAsString();

                    //BoardAsString += ";";

                    ListOfWordsAsString = ""; //clearing it

                    foreach (string word in ListOfWords)
                    {
                        ListOfWordsAsString += word + ":"; //the colon is so we can use the .Split() to get into a list quickly
                    }

                    BoardAsString = BoardAsString + ";" + ListOfWordsAsString;

                    TcpClient tcpclnt2 = new TcpClient();

                    Thread.Sleep(500);

                    tcpclnt2.Connect(ipaddress, 8001);
                    Stream stm = tcpclnt2.GetStream();

                    byte[] ba = asen.GetBytes(BoardAsString); //String must be sent as bytes
                    stm.Write(ba, 0, ba.Length);

                    tcpclnt2.Close();

                    playerTurn = 1;

                    if (duration > 10000)
                    {
                        duration -= 2000; //shortening timer
                    }

                }


                if (CheckIfGameOver() == true)
                {
                    gameOver = true;
                }
            }

            //Time to send scores to each other

            string receivedScore = "";
            

            try
            {
                TcpClient tcpclnt = new TcpClient();
                

                Thread.Sleep(500);

                tcpclnt.Connect(ipaddress, 8001);


                Stream stm = tcpclnt.GetStream();

                byte[] ba = asen.GetBytes(Convert.ToString(score));
                stm.Write(ba, 0, ba.Length);

                byte[] bb = new byte[1000000];
                int k = stm.Read(bb, 0, 1000000);
                

                for (int i = 0; i < k; i++)
                    receivedScore += Convert.ToChar(bb[i]);


                tcpclnt.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            int OpponentScore = Convert.ToInt32(receivedScore);

            Console.WriteLine("");
            Console.WriteLine("Game over. All words found.");
            Console.WriteLine("");
            Console.WriteLine("Your score: " + score);
            Console.WriteLine("Opponent score: " + OpponentScore);
            Console.WriteLine("");

            if (OpponentScore > score)
            {
                Console.WriteLine("You lose!");
            }
            else if (OpponentScore == score)
            {
                Console.WriteLine("Draw");
            }
            else
            {
                Console.WriteLine("You win!");
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