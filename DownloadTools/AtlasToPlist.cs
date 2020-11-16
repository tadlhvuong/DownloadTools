using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadTools
{
    class AtlasToPlist
    {
        private string outDir;
        private string srcUrl;
        public AtlasToPlist(string srcUrl)
        {
            this.srcUrl = srcUrl;
        }
        public void ReadFileAtlas(string urlFileAtlas)
        {
            string[] lines = System.IO.File.ReadAllLines(urlFileAtlas);
            string nameFile = lines[1];
            int position = nameFile.IndexOf(".");
            //name File .plist
            nameFile = nameFile.Substring(0,position) + ".plist";
            Console.WriteLine(nameFile);
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(srcUrl, nameFile)))
            {
                //header file
                outputFile.WriteLine("<?xml version=\"1.0\" encoding=\"UTF - 8\"?>");
                outputFile.WriteLine("<!DOCTYPE plist PUBLIC \" -//Apple Computer//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">");
                outputFile.WriteLine("<plist version=\"1.0\">");
                outputFile.WriteLine("<dict>");
                outputFile.WriteLine("<key>metadata</key>");
                outputFile.WriteLine("<dict>");
                outputFile.WriteLine("<key>format</key>");
                outputFile.WriteLine("<integer>3</integer>");
                outputFile.WriteLine("<key>realTextureFileName</key>");
                outputFile.WriteLine("<string>" + lines[1] + "</string>");
                outputFile.WriteLine("<key>size</key>");
                outputFile.WriteLine("<string>" + lines[2].Substring(6) + "</string>");
                outputFile.WriteLine("<key>textureFileName</key>");
                outputFile.WriteLine("<string>" + lines[1] + "</string>");
                outputFile.WriteLine("</dict>");
                outputFile.WriteLine("<key>frames</key>");
                outputFile.WriteLine("<dict>");

                //body file (image child)
                for (int i = 6; i < lines.Length; i += 7)
                {
                    outputFile.WriteLine("<key>" + lines[i] + ".png</key>");
                    outputFile.WriteLine("<dict>");
                    outputFile.WriteLine("<key>aliases</key>");
                    outputFile.WriteLine("<array/>");
                    outputFile.WriteLine("<key>spriteOffset</key>");
                    outputFile.WriteLine("<string>{" + lines[i + 5].Substring(10) + "}</string>");
                    outputFile.WriteLine("<key>spriteSize</key>");
                    outputFile.WriteLine("<string>{" + lines[i + 3].Substring(9) + "}</string>");
                    outputFile.WriteLine("<key>spriteSourceSize</key>");
                    outputFile.WriteLine("<string>{" + lines[i + 4].Substring(9) + "}</string>");
                    outputFile.WriteLine("<key>textureRect</key>");
                    outputFile.WriteLine("<string>{{" + lines[i + 2].Substring(6) + "},{"+ lines[i+3].Substring(8) + "}}</string>");
                    outputFile.WriteLine("<key>textureRotated</key>");
                    outputFile.WriteLine("<"+lines[i+1].Substring(10)+ "/>");
                    outputFile.WriteLine("</dict>");
                }

                //footer file
                outputFile.WriteLine("</dict>");
                outputFile.WriteLine("</dict>");
                outputFile.WriteLine("</plist>");
            }
            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }
    }
}
