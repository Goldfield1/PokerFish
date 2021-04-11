using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace PokerFish.Scraber
{
    // TODO
    // Using Bitmaps in dict is bad for performance, this could be changed to HASH,
    // but since it is only used to map chars once, it doesn't hurt too bad.
    public class Trie
    {
        public TrieNode Root;

        public Trie(List<Bitmap> images, Dictionary<Bitmap, string> dict)
        {
            Root = ConstructNode(images, -1, dict);
        }
        
        // this method finds where the image starts, since it's better two give findStringMatch an image with max 2 padding at end
        public string FindStringMatch(Bitmap image)
        {
            var w = 1;
            // images a given in full length with padding, find where the word starts 
            while (true)
            {
                // if one of these pixels are white, name has begun    
                if (image.GetPixel(image.Width-w,0).R > 36 || image.GetPixel(image.Width-w,2).R > 36 || image.GetPixel(image.Width-w,4).R > 36 || image.GetPixel(image.Width-w,6).R > 36 || image.GetPixel(image.Width-w,8).R > 36)
                {
                    break;
                }

                w++;
            }
            // leave 1 pixel of padding to recognize some characters 
            if (w > 1)    
            {
                image = image.Clone(new Rectangle(0,0,image.Width-w+2,image.Height), image.PixelFormat);
            }

            return findStringMatch(image);
        }
        
        private string findStringMatch(Bitmap image)
        {
            var offsetNoMatch = 0;
            var matchingString= "";    
            var offset = image.Width;
            var width = 1;
            
            while (offset - width >= 0)
            {
                var format = image.PixelFormat;
                
                var subimage = image.Clone(new Rectangle(offset-width,0,width,image.Height), format);
                var b = image.Clone(new Rectangle(0,0,image.Width,image.Height), format);

                if (offsetNoMatch == 9 || (offset-width == 0 && offsetNoMatch > width-1))    
                {
                    var fill = "<>";
                    if (offset == image.Width)
                    {
                        fill = "";
                    }
                    return findStringMatch(image.Clone(new Rectangle(0,0,offset-1,image.Height), format)) + fill + matchingString;
                }
                
                // j is a special case since it overlaps
                if (subimage.Width > 3 && subimage.GetPixel(subimage.Width-2, 0).R > 100 && subimage.GetPixel(subimage.Width-2, 8).R > 100)
                {
                    offsetNoMatch = 0;
                    width = 1;
                    matchingString = "j" + matchingString;
                    image.SetPixel(offset-4, 8, Color.FromArgb(255,15,15,15));
                    offset -= 3;
                    continue;
                }
                
                // f is a special case since it overlaps, and pixels overlap 
                // due to this, overlapping pixels has to be removed sequentially to see if next character needs theme or not
                if (offset-width-2 > -1 && image.GetPixel(offset-width, 0).R > 100 && image.GetPixel(offset-width-2, 0).R > 100)
                {
                    var subimage1 = new Bitmap(image);
                    var subimage2 = new Bitmap(image);
                    var subimage3 = new Bitmap(image);

                    var subimages = new List<Bitmap>();
                    subimage1.SetPixel(offset-width,0, Color.FromArgb(255,15,15,15) );
                    subimage1.SetPixel(offset-width,2, Color.FromArgb(255,15,15,15) );
                    subimages.Add(subimage1);
                    
                    subimage2.SetPixel(offset-width,0, Color.FromArgb(255,15,15,15) );
                    subimages.Add(subimage2);
                    
                    subimage3.SetPixel(offset-width,2, Color.FromArgb(255,15,15,15) );
                    subimages.Add(subimage3);
                    
                    Tuple<Bitmap, string> m = null;
                    for (int i = 0; i < 3; i++)
                    {
                        subimage = subimages[i].Clone(new Rectangle(offset-width,0,width,image.Height), subimages[i].PixelFormat);
                        m = FindMatch(subimage);
                        if (m != null)
                        {
                            matchingString = "f" + m.Item2 + matchingString;
                            offset -= m.Item1.Width;
                            offset -= 3;
                            offsetNoMatch = 0;
                            width = 1;
                            break;
                        }
                    }
                    if (m != null)
                    {
                        continue;
                    }
                }
                
                var match = FindMatch(subimage);
                if (match == null)
                {
                    width++;
                    offsetNoMatch++;
                    continue;
                }

                offsetNoMatch = 0;
                width = 1;
                matchingString = match.Item2 + matchingString;

                if (matchingString[0] == ' ')
                {
                    // double space means name is over
                    if (matchingString.Length > 1 && matchingString[1] == ' ')
                    {
                        return matchingString.Substring(2, matchingString.Length - 2);       
                    }
                    
                    // to fix trailing space of 1 pixel, which messes up other matches
                    if (offset == image.Width)
                    {
                        return findStringMatch(image.Clone(new Rectangle(0,0,offset-1,image.Height), format)) + matchingString;   
                    }
                }

                offset -= match.Item1.Width;
            }
            
            // delete cases where name is long, so one leading space survives
            if (matchingString != "" && matchingString[0] == ' ')
            {
                matchingString = matchingString.Substring(1, matchingString.Length - 1);      
            }
            return matchingString;
        }
        
        public void Traverse(TrieNode node, int depth, Dictionary<Bitmap,string> dict)
        {
            if (node?.Children == null)
            {
                return;   
            }
            
            var children = node.Children;

            if (node.Residents != null)
            {
                foreach (var res in node.Residents)
                {
                    if (res != null)
                    {
                        Console.WriteLine("{0} {1}",depth,dict[res]);        
                    }
                }
            }
            
            if (children != null)
            {
                foreach (var c in children)
                {
                    Traverse(c, depth+1, dict);
                }    
            }
        }

        private TrieNode ConstructNode(List<Bitmap> images, int depth, Dictionary<Bitmap, string> dict)
        {
            TrieNode node = new TrieNode();
            
            if (images.Count <= 1)
            {
                if (images.Count != 0)
                {
                    node.Residents.Add(images[0]);
                    node.ResidentKeys = new List<string>() {dict[images[0]]};
                }
                return node;
            }
                
            List<Bitmap>[] child_images = new List<Bitmap>[9];
            var found = false;
            foreach (var image in images)
            {
                //There are no more ‘characters’ left 
                if (image.Width == depth + 1)
                {
                    node.Residents.Add(image);
                    node.ResidentKeys.Add(dict[image]);
                    
                }
                else
                {
                    found = true;
                    if (child_images[columnPixelCount(image, depth + 1)] == null)
                    {
                        child_images[columnPixelCount(image, depth + 1)] = new List<Bitmap>();      
                    }
                    child_images[columnPixelCount(image, depth + 1)].Add(image);   
                }
            }

            for (int i = 0; i < 9; i++)
            {
                if (child_images[i] != null)
                {
                    node.Children[i] = ConstructNode(child_images[i], depth + 1, dict);     
                }
            }
            return node;
        }

        public Tuple<Bitmap,string> FindMatch(Bitmap matchAgainst)
        {
            var curNode = Root;
            var offset = 0;
            while (curNode != null && offset <= matchAgainst.Width)
            {
                // at the end of trie
                if (offset == matchAgainst.Width)
                {
                    if (curNode.Residents.Count > 0)
                    {
                        var c = 0;
                        foreach (var res in curNode.Residents)
                        {
                            if (ExactMatch(matchAgainst, res))
                            {
                                return Tuple.Create(res, curNode.ResidentKeys[c]);   
                            }
                            c++;
                        } 
                    }
                    return null;
                }
                
                var count = columnPixelCount(matchAgainst, offset);
                var tmp = curNode.Children?[count];
                
                if (tmp == null && curNode.Residents.Count == 0)
                {
                    return null;
                }
                if (tmp == null)
                {
                    int c = 0;
                    if (curNode.Residents.Count == 1)
                    {
                        if (ExactMatch(matchAgainst, curNode.Residents[0]))
                        {
                            return Tuple.Create(curNode.Residents[0], curNode.ResidentKeys[0]);
                        }
                    }    
                    foreach (var res in curNode.Residents)
                    {
                        if (ExactMatch(matchAgainst, res))
                        {
                            return Tuple.Create(res, curNode.ResidentKeys[c]);   
                        }
                        c++;
                    }
                    return null;
                }
                
                curNode = tmp;
                offset++;    
            }
            return null;
        }
        
        private static int columnPixelCount(Bitmap image, int x)
        {
            int count = 0;
            for (int j = 0; j < image.Height; j++)
            {
                var pix = image.GetPixel(image.Width-x-1, j);
                if (ImageMappingColors.IsWhite(pix))
                {
                    count++;
                }            
            }

            return count;
        }
        
        
        private bool ExactMatch(Bitmap img, Bitmap match)
        {
            if (img.Size != match.Size)
            {
                return false;
            }
            
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    var pix = img.GetPixel(i, j);
                    if (ImageMappingColors.IsWhite(pix))
                    {
                        pix = ImageMappingColors.White;

                    } else
                    {
                        pix = Color.FromArgb(255, 15, 15, 15);
                    }
                    if (pix != match.GetPixel(i,j))
                    {
                        return false;
                    }    
                }   
            }
            return true;
        }
    }
}