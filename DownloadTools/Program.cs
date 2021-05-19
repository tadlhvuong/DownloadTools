using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadTools
{
    class Program
    {
        static void Main(string[] args)
        {
            //string DirectoryOutput = null;
            //string DirectoryFileJson = null;
            //string UrlDownload = null;
            //Console.Write("Directory Output: ");
            //DirectoryOutput = Console.ReadLine();
            //Console.Write("Directory File Json: ");
            //DirectoryFileJson = Console.ReadLine();
            //Console.Write("Directory URL Download: ");
            //UrlDownload = Console.ReadLine();


            //var readAtlas = new AtlasToPlist(@"D:\TEMP\NoHu\res\GateResCode\anim");
            //readAtlas.ReadFileAtlas(@"D:\TEMP\NoHu\res\GateResCode\anim\Icon-Candy-2.atlas");

            //var readJson = new JsonToAtlas(@"C:\Users\Admin\Desktop\jsontoatlas");
            //readJson.ReadFileJson3(@"C:\Users\Admin\Desktop\jsontoatlas\28.json");

            //var readPlist = new PlistToAtlas(@"D:\project\srcImg\b29total\source\Atlas\aladdin_bigwin");
            //readPlist.ReadFilePlist(@"D:\project\srcImg\b29total\source\Atlas\aladdin_bigwin\aladdin_bigwin.plist");

            //PList plist = new PList(@"D:\project\srcImg\b29total\source\Atlas\aladdin_bigwin\aladdin_bigwin.plist");


            //get source cocos creaotr

            //var dl = new Downloader(@"D:\project\srcImg\boc_v1", "https://bocvip.club/");
            //dl.DoDownloadJson(@"D:\project\srcImg\boc_v1\1.json");

            //get animation cocos creator

            var x = new CocosAnim(@"D:\project\srcImg\boc_v1\source\assets.json", @"D:\project\srcImg\boc_v1\source\template.html", @"D:\project\srcImg\boc_v1\source\template.fnt");
            //x.ExtractFolder(@"D:\project\srcImg\boc_v1\res\import", @"D:\project\srcImg\boc_v1\res\raw-assets\", @"D:\project\srcImg\boc_v1\res\test\", null);
             //x.ExtractFolder(@"D:\project\srcImg\boc_v1\res\test", @"D:\project\srcImg\boc_v1\res\raw-assets\", @"D:\project\srcImg\boc_v1\res\test\", null);   //test tung file json

            x.ExtractFolder(@"D:\project\srcImg\boc_v1\res\import", @"D:\project\srcImg\boc_v1\res\raw-assets\", @"D:\project\srcImg\boc_v1\res\test\", "a8d8yV/+pLraBLW/eUEAHA");   //test tung id texture

        }
    }
}
