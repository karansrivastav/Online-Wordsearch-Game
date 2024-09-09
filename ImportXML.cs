using System.Xml;

namespace NEA
{
    static class ImportXML
    {
        static XmlDocument doc = new XmlDocument();

        public static XmlDocument Load()
        {
            doc.Load("GameFile.xml");
            return doc;
        }
        
    }
}
