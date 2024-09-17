using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;

namespace SentinelOS.GUI
{
    class Resources
    {
        [ManifestResourceStream(ResourceName = "SentinelOS.Dependencies.cursor.bmp")] private static byte[] cursor;
        [ManifestResourceStream(ResourceName = "SentinelOS.Dependencies.osicon.bmp")] private static byte[] osIcon;
        [ManifestResourceStream(ResourceName = "SentinelOS.Dependencies.file.bmp")] private static byte[] file;
        [ManifestResourceStream(ResourceName = "SentinelOS.Dependencies.folder.bmp")] private static byte[] folder;
        [ManifestResourceStream(ResourceName = "SentinelOS.Dependencies.network.bmp")] private static byte[] network;
        [ManifestResourceStream(ResourceName = "SentinelOS.Dependencies.background.bmp")] private static byte[] background;
        public static Bitmap cursorBitmap = new Bitmap(cursor);
        public static Bitmap osIconBitmap = new Bitmap(osIcon);
        public static Bitmap fileBitmap = new Bitmap(file);
        public static Bitmap folderBitmap = new Bitmap(folder);
        public static Bitmap networkBitmap = new Bitmap(network);
        public static Bitmap backgroundBitmap = new Bitmap(background);
    }
}
