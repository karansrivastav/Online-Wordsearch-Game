using System;
using System.Collections.Generic;
using System.Xml;

namespace NEA
{
    class XMLStream
    {
        public void TraverseNodesLogin(XmlNodeList nodes, string username, string password, ref bool usernameValid, ref bool passwordValid) 
        {   
            //This recursive loop checks to see if the username and password matches in the XML file
            foreach (XmlNode node in nodes)
            {
                if (node.Name == "USERNAME")
                {
                    if (node.InnerText == username)
                    {
                        usernameValid = true;
                    }

                }
                else if (node.Name == "PASSWORD")
                {
                    if (node.InnerText == password)
                    {
                        passwordValid = true;
                    }
                }

                TraverseNodesLogin(node.ChildNodes, username, password, ref usernameValid, ref passwordValid);
            }
        }

        public void TraverseNodes(XmlNodeList nodes,  ref List<string> ListOfWords, ref int dimensions, string username, string boardname)
        {   
            //This recursive loop checks if the board exists, and sets all the attributes if it does exist
            foreach (XmlNode node in nodes)
            {
                if (node.Name == "CREATOR" && node.InnerText != username)
                {
                    break; //if the creator is not the given username, pop off the recursion stack and go back to previous node
                }

                if (node.Name == "BOARDNAME" && node.InnerText != boardname)
                {
                    break;
                }

                if (node.Name == "Word") //Add every word to the list of words
                {
                    ListOfWords.Add(node.InnerText);
                }

                else if (node.Name == "DIMENSIONS")
                {
                    dimensions = Convert.ToInt32(node.InnerText);
                }

                TraverseNodes(node.ChildNodes, ref ListOfWords, ref dimensions, username, boardname);
            }
        }

        public void BoardNameExist(XmlNodeList nodes, string boardName, ref bool nameExists)
        {   
            //This recursive loop checks to see if the board name entered is already taken
            foreach (XmlNode node in nodes)
            {
                if (node.Name == "BOARDNAME" && node.InnerText == boardName)
                {
                    Console.WriteLine("Name already taken.");
                    nameExists = true;
                    break; //break once board name has been found
                }

                BoardNameExist(node.ChildNodes, boardName, ref nameExists);
            }
        }

        public void UsernameExist(XmlNodeList nodes, string username, ref bool nameExists)
        {   
            //This recursive loop checks to see if the username entered is already taken
            foreach (XmlNode node in nodes)
            {
                if (node.Name == "USERNAME" && node.InnerText == username)
                {
                    Console.WriteLine("Name already taken.");
                    nameExists = true;
                    break;  //break once username has been found
                }

                UsernameExist(node.ChildNodes, username, ref nameExists);
            }
        }
    }
}
