using System.Drawing;

namespace PokerFish.Scraber
{
    public static class ImageMappingColors
    {
        private static Color BlackActive = Color.FromArgb(255, 100, 100, 100);
        private static Color BlackPassive = Color.FromArgb(255, 53, 53, 53);
        public static Color White = Color.FromArgb(255, 130, 130, 130);
                
        public static bool IsWhite(Color pix)
        {

            return pix.R > BlackActive.R && pix.B > BlackActive.B && pix.G > BlackActive.G;   
            
            return pix.R > BlackPassive.R && pix.B > BlackPassive.B && pix.G > BlackPassive.G;
        }
    }
    
    public static class ConfigImageStrings
    {
        public static string CharacterConfigString = 
            "ADCDEFGHIJKLMNOPQESTUVWXZ .,_-%$#/<>®@ abcdefghijklmnopqrstuvwxyzöü1234567890=?!";
    }
}