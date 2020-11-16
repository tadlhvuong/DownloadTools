using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadTools
{
    class JsonToPlist
    {
        private string outDir;
        private string srcUrl;
        public JsonToPlist(string srcUrl)
        {
            this.srcUrl = srcUrl;
        }
        public void ReadFileJson(string urlFileJson)
        {
            string[] lines = File.ReadAllLines(urlFileJson);
            dynamic json = JsonConvert.DeserializeObject(lines[0]);
            //string name = json.imagePath;
            //int position = name.IndexOf(".");
            //name File .atlas
            string nameFile = "symbol_effect.atlas";

            dynamic temp = json.frames.Count;
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(srcUrl, nameFile)))
            {
                //header file
                outputFile.WriteLine("symbol_effect");
                outputFile.WriteLine("size: 1018,479");
                outputFile.WriteLine("format: RGBA8888");
                outputFile.WriteLine("filter: Linear,Linear");
                outputFile.WriteLine("repeat: none");

                //body file (image child)
                foreach (dynamic item in json.frames)
                {
                    dynamic temp1 = item.Value;
                    outputFile.WriteLine(item.Name + ".png");
                    outputFile.WriteLine("  rotate: false");
                    outputFile.WriteLine("  xy: " + temp1.x + ", " + temp1.y);
                    outputFile.WriteLine("  size: " + temp1.w + ", " + temp1.h);
                    outputFile.WriteLine("  orig: " + temp1.sourceW + ", " + temp1.sourceH);
                    outputFile.WriteLine("  offset: " + temp1.offX + ", " + temp1.offY);
                    outputFile.WriteLine("  index: -1");
                }
            }
            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }

    }
}
