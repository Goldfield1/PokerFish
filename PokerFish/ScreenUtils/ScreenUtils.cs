using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]  
public struct RECT  
{
    public int Left;        // x position of upper-left corner  
    public int Top;         // y position of upper-left corner  
    public int Right;       // x position of lower-right corner  
    public int Bottom;      // y position of lower-right corner  
}

/*
var dum = System.Drawing.Image.FromFile("C:/Users/Simon/Desktop/but.png");
var dd = new Bitmap(dum);
searchBitmap.Save("img.jpg", ImageFormat.Jpeg); 
 */

namespace PokerFish.ScreenUtils
{
    public class ScreenUtils
    {
        public static Bitmap CaptureScreen(int x, int y, int xr, int yr)
        {
            var image = new Bitmap(xr - x, yr - y, PixelFormat.Format32bppArgb);
            var gfx = Graphics.FromImage(image);
            gfx.CopyFromScreen(x, y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
            return image;
        }
    }
}