using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
namespace DownloadTools
{
    class JsonToAtlas
    {
        private string outDir;
        private string srcUrl;
        public JsonToAtlas(string srcUrl)
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
                foreach(dynamic item in json.frames)
                {
                    dynamic temp1 = item.Value;
                    outputFile.WriteLine(item.Name+".png");
                    outputFile.WriteLine("  rotate: false");
                    outputFile.WriteLine("  xy: "+ temp1.x + ", " + temp1.y);
                    outputFile.WriteLine("  size: " + temp1.w + ", " + temp1.h);
                    outputFile.WriteLine("  orig: " + temp1.sourceW + ", " + temp1.sourceH);
                    outputFile.WriteLine("  offset: " + temp1.offX + ", " + temp1.offY);
                    outputFile.WriteLine("  index: -1");
                }
            }
            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }

        //tham so x,y,w,h
        public void ReadFileJson2(string urlFileJson)
        {
            string[] lines = File.ReadAllLines(urlFileJson);
            dynamic json = JsonConvert.DeserializeObject(lines[0]);
            string name = json.file;
            int position = name.IndexOf(".");
            //name File .atlas
            string nameFile = name.Substring(0, position) + ".atlas";

            dynamic temp = json.res.Count;
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(srcUrl, nameFile)))
            {
                //header file
                outputFile.WriteLine(name);
                outputFile.WriteLine("size: 4096, 2048");
                outputFile.WriteLine("format: RGBA8888");
                outputFile.WriteLine("filter: Linear,Linear");
                outputFile.WriteLine("repeat: none");

                //body file (image child)
                foreach (dynamic item in json.res)
                {
                    outputFile.WriteLine(item.Name + ".png");
                    outputFile.WriteLine("  rotate: false");
                    outputFile.WriteLine("  xy: " + item.Value.x + ", " + item.Value.y);
                    outputFile.WriteLine("  size: " + item.Value.w + ", " + item.Value.h);
                    outputFile.WriteLine("  orig: " + item.Value.w + ", " + item.Value.h);
                    outputFile.WriteLine("  offset: 0, 0");
                    outputFile.WriteLine("  index: -1");
                }
            }
            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }


        public void ReadFileJson3(string urlFileJson)
        {
            string lines = File.ReadAllText(urlFileJson);
            dynamic json = JsonConvert.DeserializeObject(lines);
            //string name = json.imagePath;
            //int position = name.IndexOf(".");
            //name File .atlas
            string nameFile = "28.atlas";

            dynamic temp = json.Count;
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(srcUrl, nameFile)))
            {
                //header file
                outputFile.WriteLine("1");
                outputFile.WriteLine("size: 464,2021");
                outputFile.WriteLine("format: RGBA8888");
                outputFile.WriteLine("filter: Linear,Linear");
                outputFile.WriteLine("repeat: none");
                //{ "__type__": "cc.SpriteFrame"}
                //body file (image child)
                foreach (dynamic item in json)
                {
                    if (item.Count >= 2 || item.Count ==1)
                        continue;
                    if ( item["__type__"] == "cc.SpriteFrame")
                    {
                        foreach (dynamic item1 in item)
                        {
                            if (item1.Value.Value != "cc.SpriteFrame")
                            {

                                if (item1.Name == "content")
                                {
                                    outputFile.WriteLine(item1.Value["name"].Value);
                                    if (item1.Value["rotated"] ==  null)
                                        outputFile.WriteLine("  rotate: true");
                                    else
                                        outputFile.WriteLine("  rotate: false");
                                    outputFile.WriteLine("  xy: " + item1.Value["rect"][0].Value + ", " + item1.Value["rect"][1].Value);
                                    outputFile.WriteLine("  size: " + item1.Value["rect"][3].Value + ", " + item1.Value["rect"][2].Value);
                                    outputFile.WriteLine("  orig: " + item1.Value["originalSize"][0].Value + ", " + item1.Value["originalSize"][1].Value);
                                    outputFile.WriteLine("  offset: " + item1.Value["offset"][0].Value + ", " + item1.Value["offset"][1].Value);
                                    outputFile.WriteLine("  index: -1");
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }
    }
}
