using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace PokerFish.Scraber
{
    public static class ImageBuilder
    {
        public static Trie ParseConfigImages()
        {
            List<Bitmap> imgs = new List<Bitmap>();
            Dictionary<Bitmap, string> dict = new Dictionary<Bitmap, string>();
            string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890 .,_-%$#/<>®@=?!'";
            string[] values = letters.Select(c => c.ToString()).ToArray();
            
            
            var img = System.Drawing.Image.FromFile("manyletters.png");
            var bitmap = new Bitmap(img);

            var letterNum = 0;
            var lastLetterX = 0;
            var y = 0;
            
            string trieString = "";

            int width = 0;
            for (int i = 0; i < bitmap.Width; i++)
            {
                if (bitmap.GetPixel(i, 0) == Color.FromArgb(255, 0, 38, 255))
                {
                    PixelFormat format = bitmap.PixelFormat;
                    Bitmap cloneBitmap = bitmap.Clone(new Rectangle(i-width,0,width,bitmap.Height), format);

                    dict.Add(cloneBitmap, values[letterNum]);
                    imgs.Add(cloneBitmap);

                    trieString = "";
                    letterNum++;
                    width = 0;
                    
                    continue;
                }
                
                int colN = 0;
                for (int j = 0; j < bitmap.Height; j++)
                { 
                    var pix = bitmap.GetPixel(i, j);

                    if (j == 0 && i == 0)
                    {
                        if (bitmap.GetPixel(i,j).R > 20)
                        {
                            Console.WriteLine(bitmap.GetPixel(i,j));   
                        }
                    }
                    if (ImageMappingColors.IsWhite(pix))
                    {
                        bitmap.SetPixel(i, j,Color.FromArgb(255, 130,130,130));
                        colN++;

                    }
                    else
                    {
                        bitmap.SetPixel(i, j, Color.FromArgb(255, 15,15,15));
                        
                    }
                }

                trieString += colN + ",";
                width++;
            }
            bitmap.Save("letterssss.png", ImageFormat.Png);
        
            var ggg = System.Drawing.Image.FromFile("newName.png");
            var gggg = new Bitmap(ggg);
            
            Trie trie = new Trie(imgs, dict);

            return trie;
        }

        public static void TEST()
        {

        }
    }
}