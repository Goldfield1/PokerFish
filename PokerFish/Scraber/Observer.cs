using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using PokerFish.WindowHandling;

namespace PokerFish.Scraber
{    
    public class Scraber
    {
        [DllImport("user32.dll")]  
        static extern IntPtr GetForegroundWindow();  
        
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int Width, int Height, bool Repaint);
        
        [StructLayout(LayoutKind.Sequential)]  
        public struct RECT  
        {
            public int Left;        // x position of upper-left corner  
            public int Top;         // y position of upper-left corner  
            public int Right;       // x position of lower-right corner  
            public int Bottom;      // y position of lower-right corner  
        }        
        
        [DllImport("user32.dll")]  
        [return: MarshalAs(UnmanagedType.Bool)]  
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        private RECT rect;
        private bool tablesActive;
        private Trie trie;
        
        public Scraber()
        {
            trie = PokerFish.Scraber.ImageBuilder.ParseConfigImages();
            
            IntPtr tableHandle = GetPokerTableHandle();
            MoveWindow(tableHandle, -6, 0, 660, 400, true);
            SetForegroundWindow(tableHandle);
            
            rect = new RECT();
            var tablesActive = GetWindowRect(tableHandle, out rect);
        }

        public void Start()
        {
            if (rect.Right == rect.Left)
            {
                Console.WriteLine("No tables");
                return;
            }

            rect.Left += 7;
            rect.Right -= 7;
            rect.Bottom -= 7;
            
            Point[] pivotPoints =
            {
                new Point(97, 273), 
                new Point(117, 135),
                new Point(341, 92),
                new Point(528, 135),
                new Point(548, 273),
                new Point(304, 354)
            };
            // offset is -3 compared to pivot
            // get turn offset i y - 30, R > 160 B < 75

            var names = new string[] {null, null, null, null, null, null};
            while (true)
            {
                var img = ScreenUtils.ScreenUtils.CaptureScreen(rect.Left, rect.Top, rect.Right, rect.Bottom);

                var x = 74;
                var allNamesFound = true;
                for (int i = 0; i < 6; i++)
                {
                    var p = pivotPoints[i];

                    if (names[i] != null)
                    {
                        
                        continue;
                    }
                    allNamesFound = false;
                    
                    if (i >= 3)
                    {
                        x = 0;
                    }
                
                    var hasTurnPix = img.GetPixel(p.X, p.Y - 20);
                    var inRoundPix = img.GetPixel(p.X, p.Y - 30);
                    
                    if (hasTurnPix.R > 102)
                    {
                        continue;    
                    }
                    
                    //var textImg = ScreenUtils.ScreenUtils.CaptureScreen(rect.Left+p.X-x, rect.Top+p.Y -11, rect.Left+p.X-x+74, rect.Top+p.Y -2);
                    //textImg.Save("test.png", ImageFormat.Png);
                    var textImg = img.Clone(new Rectangle(p.X-x, p.Y -11,74,9), img.PixelFormat);
                    
                    //textImg.Save(i+"name.png",ImageFormat.Png);
                    
                    // detect if player is making an action, so their name does not show
                    var found = false;
                    for (int j = 0; j < 40; j+=2)
                    {
                        var pp = textImg.GetPixel(20 + j, 1);
                        if (pp.B - pp.R > 50)
                        {
                            found = true;
                            break;
                        }                   
                    }
                    if (found)
                    {
                        continue;
                    }

                    string match = "";
                    if (inRoundPix.R > 160 && inRoundPix.B < 75)
                    {
                        match = trie.FindStringMatch(textImg);
                        if (match != "")
                        {
                            names[i] = match;  
                            Console.WriteLine(match);
                        }
                    }
                }

                if (allNamesFound)
                {
                    break;
                }
                Thread.Sleep(200);
            }
            
            Console.WriteLine("-------");
            foreach (var name in names)
            {
                Console.WriteLine(name);    
            }
        }
        
        private IntPtr GetPokerTableHandle()
        {
            IntPtr tableHandle = IntPtr.Zero;
            foreach(KeyValuePair<IntPtr, string> window in OpenWindowGetter.GetOpenWindows())
            {
                IntPtr handle = window.Key;
                string title = window.Value;
                if (title.IndexOf("Hold'em") != -1 && (title.IndexOf("USD") != -1))
                {
                    tableHandle = handle;
                }
            }
            
            return tableHandle;
        }
    }
}