using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace DownloadTools
{
    public class CocosAsset
    {
        [JsonProperty("uuid")]
        public string UUId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class CocosObjSkeleton
    {
        [JsonProperty("__type__")]
        public string Type { get; set; }

        [JsonProperty("_name")]
        public string Name { get; set; }

        [JsonProperty("_skeletonJson")]
        public JObject SkeletonJsonStr { get; set; }

        [JsonProperty("_atlasText")]
        public string AtlasText { get; set; }

        [JsonProperty("textures")]
        public JArray TexturesIDs { get; set; }

        [JsonProperty("textureNames")]
        public JArray TextureNames { get; set; }
    }

    public class CocosObjAtlas
    {
        [JsonProperty("__type__")]
        public string Type { get; set; }
        [JsonProperty("_name")]
        public string Name { get; set; }

        [JsonProperty("_spriteFrames")]
        public JObject SpriteFrames { get; set; }
    }

    public class CocosObjSpriteFrame
    {
        [JsonProperty("__type__")]
        public string Type { get; set; }
        [JsonProperty("content")]
        public ContentSpriteFrame Content { get; set; }
    }
    public class ContentSpriteFrame
    {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("texture")]
        public string texture { get; set; }

        [JsonProperty("rect")]
        public JArray Rect { get; set; }

        [JsonProperty("offset")]
        public JArray Offset { get; set; }

        [JsonProperty("originalSize")]
        public JArray OriginalSize { get; set; }
        [JsonProperty("rotated")]
        public int Rotated { get; set; }
        [JsonProperty("capInsets")]
        public JArray CapInsets { get; set; }
    }

    public class CocosObjFont
    {
        [JsonProperty("__type__")]
        public string Type { get; set; }
        [JsonProperty("_name")]
        public string Name { get; set; }
        [JsonProperty("spriteFrame")]
        public JObject SpriteFrameID { get; set; }
        [JsonProperty("fontSize")]
        public int FontSize { get; set; }
        [JsonProperty("_fntConfig")]
        public _fntConfig _fntConfig { get; set; }
    }

    public class _fntConfig
    {
        [JsonProperty("commonHeight")]
        public int CommonHeight { get; set; }
        [JsonProperty("fontSize")]
        public int FontSize { get; set; }
        [JsonProperty("atlasName")]
        public string AtlasName { get; set; }
        [JsonProperty("fontDefDictionary")]
        public JObject FontDictionary { get; set; }
        [JsonProperty("kerningDict")]
        public JObject KerningDict { get; set; }
    }

    public class CharConfig
    {
        [JsonProperty("rect")]
        public RectObj Rect { get; set; }
        [JsonProperty("xOffset")]
        public int xOffset { get; set; }
        [JsonProperty("yOffset")]
        public int yOffset { get; set; }
        [JsonProperty("xAdvance")]
        public int xAdvance { get; set; }
    }

    public class RectObj
    {
        [JsonProperty("x")]
        public int x { get; set; }
        [JsonProperty("y")]
        public int y { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class CocosNodeObj
    {

        [JsonProperty("__type__")]
        public string Type { get; set; }
        [JsonProperty("_components")]
        public JArray Components { get; set; }
    }

    public class Audio
    {
        [JsonProperty("__type__")]
        public string type;
        [JsonProperty]
        public JObject Name;
    }
    public class AudioUID
    {
        [JsonProperty("__uuid__")]
        public JObject uid;
    }

    public class GetCheckUID
    {
        public string UID;
        public int Count;
    }

    public class CocosAnim
    {
        public string temp;
        public string tempFont;
        public WebClient wc = new WebClient();
        public Dictionary<string, string> Assets { get; private set; }

        public CocosAnim(string assetFile, string htmlFile, string fontFile)
        {
            var jsonText = File.ReadAllText(assetFile);
            var allAssets = JsonConvert.DeserializeObject<List<CocosAsset>>(jsonText);
            Assets = allAssets.Where(x => x.UUId != null).ToDictionary(k => k.UUId, v => v.Url);

            temp = File.ReadAllText(htmlFile);
            tempFont = File.ReadAllText(fontFile);

        }

        public void ExtractFolder(string srcDir, string inDir, string outDir)
        {
            var allFiles = Directory.GetFiles(srcDir, "*.json");
            foreach (var item in allFiles)
                ExtractFile(item, inDir, outDir);

            var subDirs = Directory.GetDirectories(srcDir);
            foreach (var item in subDirs)
                ExtractFolder(item, inDir, outDir);
        }

        public void ExtractFile(string jsonFile, string inDir, string outDir)
        {
            Console.WriteLine($"Working on {jsonFile}");

            var jsonText = File.ReadAllText(jsonFile);

            var allItems = new JArray();
            if (jsonText.StartsWith("["))
                allItems = JArray.Parse(jsonText);
            else
            {
                var oneItem = JObject.Parse(jsonText);
                allItems.Add(oneItem);
            }


            List<CocosObjSpriteFrame> ListSpriteFrame = new List<CocosObjSpriteFrame>();
            foreach (var sprite in allItems)
            {
                if (sprite.Type == JTokenType.Object)
                {
                    try
                    {
                        var s = sprite.ToObject<CocosObjSpriteFrame>();
                        if (s.Type != null && s.Type.Equals("cc.SpriteFrame"))
                        {
                            ListSpriteFrame.Add(s);
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            foreach (var xItem in allItems)
            {
                CocosObjFont itemFont = new CocosObjFont();
                CocosNodeObj itemNode = new CocosNodeObj();
                CocosObjAtlas itemAtlas = new CocosObjAtlas();

                if (xItem.Type == JTokenType.Array)
                {
                    #region get Audio
                    foreach (var nodeTemp in xItem)
                    {
                        if (nodeTemp["__type__"].ToString() == "cc.Node")
                        {
                            var node = nodeTemp.ToObject<CocosNodeObj>();
                            if (node.Components != null)
                            {
                                foreach (var uidTemp in node.Components)
                                {

                                    var newOBj = uidTemp.ToObject<Audio>();
                                    foreach (JProperty uid in uidTemp)
                                    {
                                        var AudioName = uid.Name;
                                        if (AudioName == "soundPhao") 
                                        if (!AudioName.Contains("Sound"))
                                            if (!AudioName.Contains("sound"))
                                                continue;
                                        foreach (var uid2 in uid)
                                        {
                                            if (uid2.Type == JTokenType.Object)
                                            {
                                                foreach (var uid3 in uid)
                                                {
                                                    if (uid2["__uuid__"] != null)
                                                    {
                                                        var s = uid2["__uuid__"].ToString();
                                                        var fileUid = s.ToString();
                                                        if (!Assets.ContainsKey(fileUid))
                                                            continue;

                                                        try
                                                        {
                                                            var outAudio = outDir + "Audio";
                                                            var urlAudio = Assets[fileUid];
                                                            var renewUrl = urlAudio.ToString().Replace(".png", ".mp3");
                                                            Directory.CreateDirectory($"{outAudio}\\{AudioName}");
                                                            var file1 = Path.Combine($"{outAudio}\\{AudioName}\\{AudioName}.mp3");
                                                            if (urlAudio.ToString().StartsWith("https"))
                                                            {
                                                                wc.DownloadFile(renewUrl, file1);
                                                            }
                                                            else
                                                            {
                                                                var urlString = renewUrl.ToString();
                                                                var substring = urlString.Substring(0, 2);
                                                                var subname = urlString.Substring(3);
                                                                var dirUrl = inDir + substring;
                                                                string[] files = System.IO.Directory.GetFiles(dirUrl, "*.mp3");
                                                                foreach (var file in files)
                                                                {
                                                                    if (file.Contains(subname))
                                                                    {
                                                                        if (System.IO.File.Exists(file1))
                                                                            System.IO.File.Delete(file1);

                                                                        File.Copy(file, file1);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            var err = e;

                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    #endregion
                    continue;
                }
                else
                {
                    dynamic item = xItem.ToObject<CocosObjSkeleton>();
                    if (item.Type == null || !item.Type.Equals("sp.SkeletonData"))
                        if (item.Type == null || !item.Type.Equals("cc.SpriteAtlas"))
                            if (item.Type == null || !item.Type.Equals("cc.BitmapFont"))
                                if (item.Type == null || !item.Type.Equals("cc.Node"))
                                    continue;
                                else itemNode = xItem.ToObject<CocosNodeObj>();
                            else itemFont = xItem.ToObject<CocosObjFont>();
                        else itemAtlas = xItem.ToObject<CocosObjAtlas>();

                    #region getAnimation
                    if (item.Type.Equals("sp.SkeletonData"))
                    {
                        if (item.TexturesIDs == null || item.TexturesIDs.Count == 0)
                            continue;

                        for (var i = 0; i < item.TexturesIDs.Count; i++)
                        {
                            var fileUid = item.TexturesIDs[i]["__uuid__"].ToString();
                            if (!Assets.ContainsKey(fileUid))
                                continue;

                            try
                            {
                                var outAnim = outDir + "Anim";
                                Directory.CreateDirectory($"{outAnim}\\{item.Name}");
                                var file1 = Path.Combine($"{outAnim}\\{item.Name}\\{item.Name}.json");

                                JObject stringSkeleton = JObject.Parse(item.SkeletonJsonStr.ToString());
                                File.WriteAllText(file1, stringSkeleton.ToString());

                                var file2 = Path.Combine($"{outAnim}\\{item.Name}\\{item.Name}.atlas");
                                File.WriteAllText(file2, item.AtlasText);

                                var textureName = $"{outAnim}\\{item.Name}\\{item.TextureNames[i]}";
                                var url = Assets[fileUid];
                                if (url.ToString().StartsWith("https"))
                                {
                                    wc.DownloadFile(Assets[fileUid], textureName);
                                }
                                else
                                {
                                    var urlString = url.ToString();
                                    var substring = urlString.Substring(0, 2);
                                    var subname = urlString.Substring(3);
                                    var dirUrl = inDir + substring;
                                    string[] files = System.IO.Directory.GetFiles(dirUrl, "*.png");
                                    foreach(var file in files)
                                    {
                                        if (file.Contains(subname))
                                        {
                                            if (System.IO.File.Exists(textureName))
                                                System.IO.File.Delete(textureName);

                                            File.Copy(file, textureName);
                                        }
                                    }
                                }

                                var newTemp = temp.Replace("{ANIM_NAME}", item.Name);
                                File.WriteAllText($"{outAnim}\\{item.Name}\\anims.html", newTemp);
                            }
                            catch (Exception e)
                            {
                                var err = e;

                            }
                        }
                    }

                    #endregion

                    #region get Font
                    if (item.Type.Equals("cc.BitmapFont"))
                    {
                        if (itemFont.SpriteFrameID == null || itemFont.SpriteFrameID.Count == 0)
                            continue;

                        var nameFont = itemFont.Name;
                        var nameSprite = "";
                        var uidSprite = "";
                        ContentSpriteFrame fntConfig = new ContentSpriteFrame();
                        foreach (var sprite in allItems)
                        {
                            if (!sprite.First.First.ToString().Equals("cc.SpriteFrame"))
                                continue;
                            var spriteFame = sprite.ToObject<CocosObjSpriteFrame>();

                            var content = spriteFame.Content;
                            uidSprite = content.texture;
                            nameSprite = content.Name;
                            if (!nameFont.Equals(nameSprite))
                                continue;
                            fntConfig = spriteFame.Content;

                            if (!Assets.ContainsKey(uidSprite))
                                continue;

                            try
                            {
                                string outFont = outDir + "font";
                                Directory.CreateDirectory($"{outFont}\\{itemFont.Name}");

                                var textureName = $"{outFont}\\{itemFont.Name}\\{itemFont.Name}" + ".png";
                                var url = Assets[uidSprite];
                                if (url.ToString().StartsWith("https"))
                                {
                                    wc.DownloadFile(Assets[uidSprite], textureName);
                                }
                                else
                                {
                                    var urlString = url.ToString();
                                    var substring = urlString.Substring(0, 2);
                                    var subname = urlString.Substring(3);
                                    var dirUrl = inDir + substring;
                                    string[] files = System.IO.Directory.GetFiles(dirUrl, "*.png");
                                    foreach (var file in files)
                                    {
                                        if (file.Contains(subname))
                                        {
                                            if (System.IO.File.Exists(textureName))
                                                System.IO.File.Delete(textureName);

                                            File.Copy(file, textureName);
                                        }
                                    }
                                }

                                var imageWidth = content.OriginalSize[0].ToObject<int>();
                                var imageHeight = content.OriginalSize[1].ToObject<int>();
                                var xpos = content.Rect[0].ToObject<int>() + content.Offset[0].ToObject<int>();
                                var ypos = content.Rect[1].ToObject<int>() + content.Offset[1].ToObject<int>();
                                Bitmap img = new Bitmap(textureName);
                                if (content.Rotated == 1)
                                {
                                    var tempSize = imageWidth;
                                    imageWidth = imageHeight;
                                    imageHeight = tempSize;
                                }
                                if (img.Size.Width != imageWidth && img.Size.Height != imageHeight)
                                {
                                    Bitmap bmpCrop = img.Clone(new Rectangle(xpos, ypos, imageWidth, imageHeight), img.PixelFormat);
                                    img.Dispose();
                                    if (System.IO.File.Exists(textureName))
                                        System.IO.File.Delete(textureName);
                                    bmpCrop.Save(textureName);
                                    bmpCrop.Dispose();
                                }
                                var Lenght = itemFont._fntConfig.FontDictionary.Count;
                                var newTemp = tempFont.Replace("{FONT_NAME}", itemFont.Name).Replace("{FONT_SIZE}", itemFont.FontSize.ToString()).Replace("{FONT_COUNT}", Lenght.ToString())
                                    .Replace("{COMMON_HEIGHT}", itemFont._fntConfig.CommonHeight.ToString()).Replace("{WIDTH}", imageWidth.ToString()).Replace("{HEIGHT}", imageHeight.ToString());
                                foreach (var c in itemFont._fntConfig.FontDictionary)
                                {
                                    string tempChar = "\nchar id={f_id} x={f_x} y={f_y} width={f_w} height={f_h} xoffset={f_xo} yoffset={f_yo} xadvance={f_xa} page=0 chnl=0 letter=\"{f_l}\"";
                                    var convertChar = c.Value.ToObject<CharConfig>();
                                    var namechar = int.Parse(c.Key);
                                    string convertASICC = (Convert.ToChar((int)c.Key[0])).ToString();
                                    char s = (char)namechar;
                                    tempChar = tempChar.Replace("{f_id}", namechar.ToString()).Replace("{f_x}", convertChar.Rect.x.ToString())
                                        .Replace("{f_y}", convertChar.Rect.y.ToString()).Replace("{f_w}", convertChar.Rect.Width.ToString())
                                        .Replace("{f_h}", convertChar.Rect.Height.ToString()).Replace("{f_xo}", convertChar.xOffset.ToString())
                                        .Replace("{f_yo}", convertChar.yOffset.ToString()).Replace("{f_xa}", convertChar.xAdvance.ToString()).Replace("{f_l}", s.ToString());

                                    newTemp = String.Concat(newTemp, tempChar);
                                }
                                newTemp = String.Concat(newTemp, "\n\nkernings count=0");
                                File.WriteAllText($"{outFont}\\{itemFont.Name}\\{itemFont.Name}.fnt", newTemp);
                            }
                            catch (Exception e)
                            {
                                var err = e;

                            }
                        }
                    }

                    #endregion

                    #region get Atlas
                    if (item.Type.Equals("cc.SpriteAtlas"))
                    {
                        if (itemAtlas.SpriteFrames == null || itemAtlas.SpriteFrames.Count == 0)
                            continue;
                        var listSpriteAtlas = new List<CocosObjSpriteFrame>();
                        foreach (var atlas in itemAtlas.SpriteFrames)
                        {
                            var nameSubSprite = atlas.Key.ToString();
                            if (ListSpriteFrame.Count == 0)
                                continue;

                            var fileUid = "";
                            List<GetCheckUID> listGetUID = new List<GetCheckUID>();
                            var listSpriteFrame = ListSpriteFrame.Where(x => x.Content.Name == nameSubSprite).Select(x => x.Content.texture).ToList();

                            foreach (var totalSprite in listSpriteFrame)
                            {
                                var total = ListSpriteFrame.Where(x => x.Content.texture == totalSprite).ToList();
                                if (total.Count == itemAtlas.SpriteFrames.Count)
                                {
                                    fileUid = total[0].Content.texture;
                                    break;
                                }
                            }

                            foreach (var spr in ListSpriteFrame)
                            {
                                if (spr.Content.Name.Equals(nameSubSprite))
                                {
                                    listSpriteAtlas = ListSpriteFrame.Where(x => x.Content.texture == fileUid).ToList();
                                    break;
                                }
                            }
                            if (!Assets.ContainsKey(fileUid))
                                continue;

                            try
                            {
                                var outAtlas = outDir + "Atlas";
                                var textureName = "";
                                var outSprite = outDir + "Sprite";
                                var stringFolder = itemAtlas.Name.Replace(".plist", "");
                                var stringFileAtlas = itemAtlas.Name.Replace(".plist", ".atlas");
                                if (listSpriteAtlas.Count == 1)
                                {
                                    Directory.CreateDirectory($"{outSprite}");
                                    textureName = $"{outSprite}\\{listSpriteAtlas[0].Content.Name}.png";
                                }
                                else
                                {
                                    Directory.CreateDirectory($"{outAtlas}\\{stringFolder}");
                                    textureName = $"{outAtlas}\\{stringFolder}\\{stringFolder}.png";
                                }

                                var url = Assets[fileUid];
                                if (url.ToString().StartsWith("https"))
                                {
                                    wc.DownloadFile(Assets[fileUid], textureName);
                                }
                                else
                                {
                                    var urlString = url.ToString();
                                    var substring = urlString.Substring(0, 2);
                                    var subname = urlString.Substring(3);
                                    var dirUrl = inDir + substring;
                                    string[] files = System.IO.Directory.GetFiles(dirUrl, "*.png");
                                    foreach (var file in files)
                                    {
                                        if (file.Contains(subname))
                                        {
                                            if (System.IO.File.Exists(textureName))
                                                System.IO.File.Delete(textureName);

                                            File.Copy(file, textureName);
                                        }
                                    }
                                }
                                var imgW = 0; var imgH = 0;
                                using (var img = Image.FromFile(textureName))
                                {
                                    imgH = img.Height;
                                    imgW = img.Width;
                                }
                                var sizeFile = imgW + "," + imgH;
                                var file1 = Path.Combine($"{outAtlas}\\{stringFolder}\\{itemAtlas.Name}");
                                var file2 = Path.Combine($"{outAtlas}\\{stringFolder}\\{stringFileAtlas}");

                                var headerFile = "<?xml version=\"1.0\" encoding=\"UTF - 8\"?>\n<!DOCTYPE plist PUBLIC \" -//Apple Computer//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd \">\n" +
                                    "<plist version=\"1.0\">\n<dict>\n<key>metadata</key>\n<dict>\n<key>format</key>\n<integer>3</integer>\n<key>realTextureFileName</key>\n<string>" + stringFolder + ".png</string>\n" +
                                    "<key>size</key>\n<string>" + sizeFile + "</string>\n<key>textureFileName</key>\n<string>" + stringFolder + ".png</string>\n</dict>\n<key>frames</key>\n<dict>\n";
                                var bodyFile = "";
                                var footerFile = "\n</dict>\n</dict>\n</plist>";

                                var headAtlas = stringFolder + "\nsize: " + sizeFile + "\nfilter: Linear,Linear\nrepeat: none\n";
                                var bodyAtlas = "";
                                foreach (var spr1 in listSpriteAtlas)
                                {
                                    var nameSprite = spr1.Content.Name.ToString();
                                    var rotated = spr1.Content.Rotated == 0 ? false : true;
                                    bodyFile += "<key>" + nameSprite + ".png</key>\n<dict>\n<key>aliases</key>\n<array/>\n<key>spriteOffset</key>\n" +
                                        "<string>{" + spr1.Content.Offset[0].ToString() + "," + spr1.Content.Offset[1].ToString() + "}</string>\n<key>spriteSize</key>\n<string>{" + spr1.Content.OriginalSize[0].ToString() + "," + spr1.Content.OriginalSize[1].ToString() + "}</string>\n" +
                                        "<key>spriteSourceSize</key>\n<string>{" + spr1.Content.OriginalSize[0].ToString() + "," + spr1.Content.OriginalSize[1].ToString() + "}</string>\n<key>textureRect</key>\n" +
                                        "<string>{{" + spr1.Content.Rect[0].ToString() + "," + spr1.Content.Rect[1].ToString() + "},{" + spr1.Content.Rect[2].ToString() + "," + spr1.Content.Rect[3].ToString() + "}}</string>\n<key>textureRotated</key>\n<" + rotated + "/>\n</dict>\n";

                                    bodyAtlas += nameSprite + "\n\trotate: " + rotated + "\n\txy: " + spr1.Content.Rect[0].ToString() + "," + spr1.Content.Rect[1].ToString() +
                                        "\n\tsize: " + spr1.Content.Rect[2].ToString() + "," + spr1.Content.Rect[3].ToString() + "\n\torig: " +
                                        spr1.Content.OriginalSize[0].ToString() + "," + spr1.Content.OriginalSize[1].ToString() +
                                        "\n\toffset: " + spr1.Content.Offset[0].ToString() + "," + spr1.Content.Offset[1].ToString() + "\n\tindex: -1\n";
                                }
                                var stringPlist = headerFile + bodyFile + footerFile;
                                File.WriteAllText(file1, stringPlist);

                                //file atlas
                                var stringAtlas = headAtlas + bodyAtlas;
                                File.WriteAllText(file2, stringAtlas);

                                ListSpriteFrame.RemoveAll(x => x.Content.texture == fileUid);
                            }
                            catch (Exception e)
                            {
                                var err = e;

                            }
                        }

                    }

                    #endregion
                }
            }

            #region getSpriteFrame
            var count = 0;
            var fileUidSprite = "";
            List<string> fileUidOld = new List<string>();
            foreach (var spr in ListSpriteFrame)
            {
                var checkUID = fileUidOld.Where(x => x == spr.Content.texture).FirstOrDefault();
                if (checkUID != null) continue;
                fileUidSprite = spr.Content.texture;
                var list = ListSpriteFrame.Where(x => x.Content.texture == fileUidSprite).ToList();
                if (!Assets.ContainsKey(fileUidSprite))
                    continue;
                fileUidOld.Add(fileUidSprite);
                try
                {
                    var textName = "UnKnow" + count;
                    var outUnKnow = outDir + "UnKnow";
                    var outSprite = outDir + "Sprite";
                    var textureName = "";
                    Directory.CreateDirectory($"{outSprite}");
                    count++;
                    var stringFolder = textName.Replace(".plist", "");
                    if (list.Count == 1)
                    {
                        Directory.CreateDirectory($"{outSprite}");
                        textureName = $"{outSprite}\\{list[0].Content.Name}.png";
                    }
                    else
                    {
                        Directory.CreateDirectory($"{outUnKnow}\\{textName}");
                        textureName = $"{outUnKnow}\\{textName}\\{textName}.png";
                    }

                    var url = Assets[fileUidSprite];
                    if (url.ToString().StartsWith("https"))
                    {
                        wc.DownloadFile(Assets[fileUidSprite], textureName);
                    }
                    else
                    {
                        var urlString = url.ToString();
                        var substring = urlString.Substring(0, 2);
                        var subname = urlString.Substring(3);
                        var dirUrl = inDir + substring;
                        string[] files = System.IO.Directory.GetFiles(dirUrl, "*.png");
                        foreach (var file in files)
                        {
                            if (file.Contains(subname))
                            {
                                if (System.IO.File.Exists(textureName))
                                    System.IO.File.Delete(textureName);

                                File.Copy(file, textureName);
                            }
                        }
                    }

                    var imgW = 0; var imgH = 0;
                    using (var img = Image.FromFile(textureName))
                    {
                        imgH = img.Height;
                        imgW = img.Width;
                    }
                    var sizeFile = imgW + "," + imgH;
                    var file1 = Path.Combine($"{outUnKnow}\\{stringFolder}\\{textName}.plist");
                    var file2 = Path.Combine($"{outUnKnow}\\{stringFolder}\\{textName}.atlas");

                    var headerFile = "<?xml version=\"1.0\" encoding=\"UTF - 8\"?>\n<!DOCTYPE plist PUBLIC \" -//Apple Computer//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd \">\n" +
                        "<plist version=\"1.0\">\n<dict>\n<key>metadata</key>\n<dict>\n<key>format</key>\n<integer>3</integer>\n<key>realTextureFileName</key>\n<string>" + stringFolder + ".png</string>\n" +
                        "<key>size</key>\n<string>" + sizeFile + "</string>\n<key>textureFileName</key>\n<string>" + stringFolder + ".png</string>\n</dict>\n<key>frames</key>\n<dict>\n";
                    var bodyFile = "";
                    var footerFile = "\n</dict>\n</dict>\n</plist>";
                    var headAtlas = stringFolder + "\nsize: " + sizeFile + "\nfilter: Linear,Linear\nrepeat: none\n";
                    var bodyAtlas = "";
                    foreach (var spr1 in list)
                    {
                        var nameSprite = spr1.Content.Name.ToString();
                        var rotated = spr1.Content.Rotated == 0 ? false : true;
                        bodyFile += "<key>" + nameSprite + ".png</key>\n<dict>\n<key>aliases</key>\n<array/>\n<key>spriteOffset</key>\n" +
                            "<string>{" + spr1.Content.Offset[0].ToString() + "," + spr1.Content.Offset[1].ToString() + "}</string>\n<key>spriteSize</key>\n<string>{" + spr1.Content.OriginalSize[0].ToString() + "," + spr1.Content.OriginalSize[1].ToString() + "}</string>\n" +
                            "<key>spriteSourceSize</key>\n<string>{" + spr1.Content.OriginalSize[0].ToString() + "," + spr1.Content.OriginalSize[1].ToString() + "}</string>\n<key>textureRect</key>\n" +
                            "<string>{{" + spr1.Content.Rect[0].ToString() + "," + spr1.Content.Rect[1].ToString() + "},{" + spr1.Content.Rect[2].ToString() + "," + spr1.Content.Rect[3].ToString() + "}}</string>\n<key>textureRotated</key>\n<" + rotated + "/>\n</dict>\n";

                        bodyAtlas += nameSprite + "\n\trotate: " + rotated + "\n\txy: " + spr1.Content.Rect[0].ToString() + "," + spr1.Content.Rect[1].ToString() +
                            "\n\tsize: " + spr1.Content.Rect[2].ToString() + "," + spr1.Content.Rect[3].ToString() + "\n\torig: " +
                            spr1.Content.OriginalSize[0].ToString() + "," + spr1.Content.OriginalSize[1].ToString() +
                            "\n\toffset: " + spr1.Content.Offset[0].ToString() + "," + spr1.Content.Offset[1].ToString() + "\n\tindex: -1\n";
                    }
                    var stringPlist = headerFile + bodyFile + footerFile;
                    File.WriteAllText(file1, stringPlist);
                    //file atlas
                    var stringAtlas = headAtlas + bodyAtlas;
                    File.WriteAllText(file2, stringAtlas);
                }
                catch (Exception e)
                {
                    var err = e;

                }
            }

            #endregion
        }
    }
}
