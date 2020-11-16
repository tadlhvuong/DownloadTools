using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace DownloadTools
{
    public class PList : Dictionary<string, dynamic>
    {
        public List<dynamic> textAtals = new List<dynamic>();
        public PList()
        {
        }

        public PList(string file)
        {
            Load(file);
            var newName = file.Replace(".plist", ".atlas");
            var file2 = Path.Combine(newName);
            File.WriteAllText(file2, textAtals[0]["realTextureFileName"].ToString());
            File.WriteAllText(file2, textAtals[0]["size"].ToString());
            File.WriteAllText(file2, "filter: Linear,Linear\nrepeat: none");
            foreach (var line in textAtals[1])
            {
                File.WriteAllText(file2, line);
            }
        }

        public void Load(string file)
        {
            Clear();

            XDocument doc = XDocument.Load(file);
            XElement plist = doc.Element("plist");
            XElement dict = plist.Element("dict");

            var dictElements = dict.Elements();
            Parse(this, dictElements);
        }

        private void Parse(PList dict, IEnumerable<XElement> elements)
        {
            for (int i = 0; i < elements.Count(); i += 2)
            {
                XElement key = elements.ElementAt(i);
                XElement val = elements.ElementAt(i + 1);

                dict[key.Value] = ParseValue(val);
            }
        }

        private List<dynamic> ParseArray(IEnumerable<XElement> elements)
        {
            List<dynamic> list = new List<dynamic>();
            foreach (XElement e in elements)
            {
                dynamic one = ParseValue(e);
                list.Add(one);
            }

            return list;
        }

        private dynamic ParseValue(XElement val)
        {
            switch (val.Name.ToString())
            {
                case "string":
                    return val.Value;
                case "integer":
                    return int.Parse(val.Value);
                case "real":
                    return float.Parse(val.Value);
                case "true":
                case "True":
                    return true;
                case "false":
                case "False":
                    return false;
                case "dict":
                    PList plist = new PList();
                    Parse(plist, val.Elements());
                    textAtals.Add(plist);
                    return plist;
                case "array":
                    List<dynamic> list = ParseArray(val.Elements());
                    return list;
                default:
                    throw new ArgumentException("Unsupported");
            }
        }
    }
}
