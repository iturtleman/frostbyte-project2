using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows;

namespace convertAnimtospsh
{
    class Program
    {
        static Dictionary<string, XDocument> docs = new Dictionary<string, XDocument>();
        static void Main(string[] args)
        {
            Console.WriteLine("Enter filename of anim file");
            string FileName=Console.ReadLine();
            XDocument doc = XDocument.Load(FileName);
            foreach (var frame in doc.Descendants("Frame"))
            {
                int h = int.Parse(frame.Attribute("Height").Value);
                int w = int.Parse(frame.Attribute("Width").Value);
                Point TL = Point.Parse(frame.Attribute("TLPos").Value);
                string file = frame.Attribute("SpriteSheet").Value;
                if (!docs.ContainsKey(file))
                {
                    docs[file] = new XDocument();
                }
                var output = docs[file];
                if (output == null || output.Root == null)
                {
                    output.Add(new XElement("SpriteSheet"));
                }
                var newframe = new XElement("Frame");
                newframe.SetAttributeValue("Height", h);
                newframe.SetAttributeValue("Width", w);
                newframe.SetAttributeValue("TLPos", TL);
                output.Root.Add(newframe);
            }
            foreach (var pair in docs)
            {
                pair.Value.Save(string.Format("{0}.png.spsh",pair.Key));
            }
        }
    }
}
