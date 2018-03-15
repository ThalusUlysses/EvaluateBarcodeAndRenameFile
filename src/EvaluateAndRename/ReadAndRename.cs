using System;
using System.Drawing;
using System.IO;

namespace ConsoleApp1
{
    class ReadAndRename
    {
        string _directory;
        string _notFoundDirectory;
        string _duplicateDirectory;
        string _tempDirectory;
        RotateFlipType _rotation;
        public ReadAndRename(string directory, int rotation)
        {
            _directory = directory;
            _notFoundDirectory = Path.Combine(directory, "NotProcessed");
            _tempDirectory = Path.Combine(directory, "Temp");
            _duplicateDirectory = Path.Combine(directory, "Duplicate");

            SetRotation(rotation);
;        }

        private void SetRotation(int rot)
        {
            switch (rot)
            {
                case 0:
                    _rotation = RotateFlipType.RotateNoneFlipNone;
                    return;
                case 90:
                    _rotation = RotateFlipType.Rotate90FlipNone;
                    return;
                case 180:
                    _rotation = RotateFlipType.Rotate180FlipNone;
                    return;
                case 270:
                    _rotation = RotateFlipType.Rotate270FlipNone;
                    return;
            }
            throw new ArgumentException($"Invalid rotation value {rot}");
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

        public Tuple<string,int> TryRename(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);

            if(!fi.Exists)
            {
                return new Tuple<string, int>($"File {filePath} does not exist", 1);
            }

            try
            {
                Bitmap map = (Bitmap)Image.FromFile(filePath);

                map.RotateFlip(RotateFlipType.Rotate270FlipNone);

                var heightHalf = map.Height / 2;
                var widthHalf = map.Width / 2;

                map = CropImage(map, new Rectangle(0, 0, widthHalf, heightHalf));

                StoreAsTemp($"Current{fi.Extension}", map);

                var t = Spire.Barcode.BarcodeScanner.ScanOne(map);
                
                if (string.IsNullOrEmpty(t) || t.Length != 7)
                {
                    return StoreAsNotFound(fi.FullName);
                }

                var fileName = $"{t}{fi.Extension}";

                return StoreAsFound(fi.FullName, fileName);
            }
            catch (Exception ex)
            {
                return new Tuple<string, int>($"Fatal {ex}", 1);
            }
        }

        private Tuple<string,int> StoreAsDuplicate(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            if (!Directory.Exists(_duplicateDirectory))
            {
                Directory.CreateDirectory(_duplicateDirectory);
            }

            fi.CopyTo(Path.Combine(_duplicateDirectory, fi.Name));
            fi.Delete();

            return new Tuple<string, int>($"Barcode duplicate found for {fi.Name}", 2);
        }

        private Tuple<string,int> StoreAsFound(string filePath, string newName)
        {
            FileInfo fi = new FileInfo(filePath);

            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }

            var pth = Path.Combine(_directory, newName);
            FileInfo newFi = new FileInfo(pth);
            if (newFi.Exists)
            {
                return StoreAsDuplicate(filePath);
            }

            fi.CopyTo(pth);
            fi.Delete();

            return new Tuple<string, int>($"Renamed image from {fi.Name} to {newFi.Name}", 0);
        }

        private void StoreAsTemp(string filePath, Bitmap map)
        {
            FileInfo fi = new FileInfo(filePath);
            if (!Directory.Exists(_tempDirectory))
            {
                Directory.CreateDirectory(_tempDirectory);
            }
            map.Save(Path.Combine(_tempDirectory, fi.Name));
        }

        private Tuple<string,int> StoreAsNotFound(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            if (!Directory.Exists(_notFoundDirectory))
            {
                Directory.CreateDirectory(_notFoundDirectory);
            }
            
            fi.CopyTo(Path.Combine(_notFoundDirectory, fi.Name));
            fi.Delete();

            return new Tuple<string, int>($"Barcode Not Found on image {fi.Name}", 1);
        }
    }
}
