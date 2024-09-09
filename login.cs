using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NEA
{
    class login
    {
        private string username;
        private string password;

        public string getUsername()
        {
            return this.username;
        }

        public void setUsername(string usernameIn)
        {
            this.username = usernameIn;
        }

        public string getPassword()
        {
            return password;
        }

        public void setPassword(string passwordIn)
        {
            this.password = passwordIn;
        }

        

        public void createAccount()
        {
            
            XmlDocument doc = new XmlDocument();
            bool usernameExists = true;
            XMLStream xmlstream = new XMLStream();

            doc.Load("GameFile.xml");
            XmlNode login = doc.CreateElement("LOGIN");
            XmlNode name = doc.CreateElement("USERNAME");
            XmlNode Password = doc.CreateElement("PASSWORD");
            //XmlNode TotalScore = doc.CreateElement("TOTALSCORE");

            while (usernameExists == true)
            {
                usernameExists = false;
                
                Console.WriteLine("Enter your username");
                username = Console.ReadLine();
                Console.WriteLine("Enter your password");
                password = Console.ReadLine();

                xmlstream.UsernameExist(doc.ChildNodes, username, ref usernameExists);

                if (username.Length == 0)
                {
                    usernameExists = true;
                }
            }

            name.InnerText = username;
            Password.InnerText = encryptPassword(password);
            //TotalScore.InnerText = "0";
            login.AppendChild(name);
            login.AppendChild(Password);
            //login.AppendChild(TotalScore);
            doc.DocumentElement.AppendChild(login);
            doc.Save("GameFile.xml");
            Console.WriteLine("Account created.");
        }


        public bool signIn()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("GameFile.xml");
            bool usernameValid = false;
            bool passwordValid = false;

            XMLStream xmlstream = new XMLStream();
            xmlstream.TraverseNodesLogin(doc.ChildNodes, username, encryptPassword(password), ref usernameValid, ref passwordValid);

            if (usernameValid == true && passwordValid == true)
            {
                return true;
            }

            else
            {
                return false;
            }

        }

        public string encryptPassword(string password) //caeser cipher to encrypt passwords
        {
            string encryptedPassword = "";

            if (password == null) //since encrypt password is used in signIn(), if it is null then program will crash in the foreach below
            {
                return "";
            }
            
            foreach (char letter in password)
            {
                int ascii = Convert.ToInt32(letter);
                ascii++;
                encryptedPassword = encryptedPassword + Convert.ToChar(ascii);
            }

            return encryptedPassword;
        }
    }
}
