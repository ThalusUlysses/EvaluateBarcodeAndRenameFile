using CommandLine;
using CommandLine.Text;
using System;
using System.Drawing;
using System.IO;

namespace ConsoleApp1
{
    class ReadAndRename
    {
        string _directory;

        public ReadAndRename(string directory)
        {
            _directory = directory;
        }

        public Bitmap CropImage(Bitmap source, Rectangle section)
        {


            // An empty bitmap which will hold the cropped image
            Bitmap bmp = new Bitmap(section.Width, section.Height);

            Graphics g = Graphics.FromImage(bmp);

            // Draw the given area (section) of the source image
            // at location 0,0 on the empty bitmap (bmp)
            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);

            return bmp;
        }

        public Tuple<string,bool> TryRename(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);

            if(!fi.Exists)
            {
                return new Tuple<string,bool>($"File {filePath} does not exist", false);
            }

            try
            {
                Bitmap map = (Bitmap)Image.FromFile(filePath);

                map.RotateFlip(RotateFlipType.Rotate270FlipNone);

                var heightHalf = map.Height / 2;
                var widthHalf = map.Width / 2;

                if(!Directory.Exists("temp"))
                {
                    Directory.CreateDirectory("temp");
                }

                map = CropImage(map, new Rectangle(0, 0, widthHalf, heightHalf));
                map.Save("temp/Current.jpg");

                var t = Spire.Barcode.BarcodeScanner.ScanOne(map);

                if (string.IsNullOrEmpty(t))
                {
                    return new Tuple<string,bool>($"Barocde not dound for file {filePath}", false);
                }
                var fileName = $"{t}{fi.Extension}";

                var pth = Path.Combine(_directory, fileName);

                if(!Directory.Exists(_directory))
                {
                    Directory.CreateDirectory(_directory);
                }

                fi.CopyTo(pth, true);

                return new Tuple<string, bool>($"Renamed file {filePath} to {fileName}", true);
            }
            catch (Exception ex)
            {
                return new Tuple<string, bool>($"Fatal {ex}", false);
            }
        }
    }

    class CommandLineArgs
    {
        [Option('s', "source-dir", HelpText = "Specifiy directory to lookup images for")]
        public string SourceDirectory { get; set; }

        [Option('t', "target-dir", HelpText = "Specifiy directory to write images to")]
        public string TargetDirectory { get; set; }


        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }  
}
