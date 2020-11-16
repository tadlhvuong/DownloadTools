//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;

//namespace DownloadTools
//{
//    public class CocosAsset
//    {
//        [JsonProperty("uuid")]
//        public string UUId { get; set; }

//        [JsonProperty("url")]
//        public string Url { get; set; }
//    }

//    public class CocosObj
//    {
//        [JsonProperty("__type__")]
//        public string Type { get; set; }

//        [JsonProperty("_name")]
//        public string Name { get; set; }

//        [JsonProperty("_skeletonJson")]
//        public JObject SkeletonJsonStr { get; set; }

//        [JsonProperty("_atlasText")]
//        public string AtlasText { get; set; }

//        [JsonProperty("textures")]
//        public JArray TexturesIDs { get; set; }

//        [JsonProperty("textureNames")]
//        public JArray TextureNames { get; set; }
//    }

//    public class CocosAtlas
//    {
//        public string temp;
//        public WebClient wc = new WebClient();
//        public Dictionary<string, string> Assets { get; private set; }

//        public CocosAtlas(string assetFile, string htmlFile)
//        {
//            var jsonText = File.ReadAllText(assetFile);
//            var allAssets = JsonConvert.DeserializeObject<List<CocosAsset>>(jsonText);
//            Assets = allAssets.Where(x => x.UUId != null).ToDictionary(k => k.UUId, v => v.Url);

//            temp = File.ReadAllText(htmlFile);
//        }

//        public void ExtractFolder(string srcDir, string outDir)
//        {
//            var allFiles = Directory.GetFiles(srcDir, "*.json");
//            foreach (var item in allFiles)
//                ExtractFile(item, outDir);

//            var subDirs = Directory.GetDirectories(srcDir);
//            foreach (var item in subDirs)
//                ExtractFolder(item, outDir);
//        }

//        public void ExtractFile(string jsonFile, string outDir)
//        {
//            Console.WriteLine($"Working on {jsonFile}");

//            var jsonText = File.ReadAllText(jsonFile);

//            var allItems = new JArray();
//            if (jsonText.StartsWith("["))
//                allItems = JArray.Parse(jsonText);
//            else
//            {
//                var oneItem = JObject.Parse(jsonText);
//                allItems.Add(oneItem);
//            }

//            foreach (var xItem in allItems)
//            {
//                if (xItem.Type == JTokenType.Array)
//                    continue;

//                dynamic item = xItem.ToObject<CocosObj>();
//                if (item.Type == null || !item.Type.Equals("sp.SkeletonData"))
//                    continue;

//                if (item.TexturesIDs == null || item.TexturesIDs.Count == 0)
//                    continue;

//                var fileUid = item.TexturesIDs[0]["__uuid__"].ToString();
//                if (!Assets.ContainsKey(fileUid))
//                    continue;

//                try
//                {
//                    Directory.CreateDirectory($"{outDir}\\{item.Name}");
//                    var file1 = Path.Combine($"{outDir}\\{item.Name}\\{item.Name}.json");

//                    //var stringSkeleton = JObject.Parse(item.SkeletonJsonStr.ToString());
//                    //File.WriteAllText(file1, item.SkeletonJsonStr);

//                    JObject stringSkeleton = JObject.Parse(item.SkeletonJsonStr.ToString());
//                    File.WriteAllText(file1, stringSkeleton.ToString());

//                    var file2 = Path.Combine($"{outDir}\\{item.Name}\\{item.Name}.atlas");
//                    File.WriteAllText(file2, item.AtlasText);

//                    var textureName = $"{outDir}\\{item.Name}\\{item.TextureNames[0]}";
//                    var url = Assets[fileUid];
//                    if (url.ToString().StartsWith("https"))
//                    {
//                        wc.DownloadFile(Assets[fileUid], textureName);
//                    }
//                    else
//                    {
//                        var urlString = url.ToString();
//                        var substring = urlString.Substring(0, 2);
//                        var dirUrl = @"D:\project\imgSRC\bonvip\res\raw-assets\" + substring;
//                        string[] files = System.IO.Directory.GetFiles(dirUrl, "*.png");
//                        File.Copy(files[0], textureName);
//                    }

//                    var newTemp = temp.Replace("{ANIM_NAME}", item.Name);
//                    File.WriteAllText($"{outDir}\\{item.Name}\\anims.html", newTemp);
//                }
//                catch (Exception e)
//                {
//                    var err = e;

//                }
//            }
//        }
//    }
//}
