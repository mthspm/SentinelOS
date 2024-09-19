using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using SentinelOS.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelOS.Windows
{
    public class DiskWindow : Window
    {
        private List<string> disksInfo;

        public DiskWindow(Canvas canvas, int x, int y, int width, int height, string name)
            : base(canvas, x, y, width, height, name)
        {
            disksInfo = new List<string>();
        }

        public override void Initialize()
        {
            windowState = WindowState.Running;
            CollectDisksInformation();
        }

        public override void Initialize(string path)
        {
            // Not needed for this window
        }

        public override void CheckWindowStateChanges()
        {
            // Not yet implemented
        }

        public override void HandleKeyPress()
        {
            if (Console.KeyAvailable)
            {
                var keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.Escape:
                        windowState = WindowState.ToClose;
                        break;
                }
            }
        }

        public override void HandleMouseInput()
        {
            HandleEssentialMouseInput();
        }

        public override void Draw()
        {
            DrawDiskWindow();
        }

        public override void Run()
        {
            Draw();
            HandleMouseInput();
            HandleKeyPress();
        }

        private void CollectDisksInformation()
        {
            this.disksInfo.Clear();
            List<string> disksInfo = DiskManager.GetAllDisksInformation();
            foreach (var diskInfo in disksInfo)
            {
                this.disksInfo.Add(diskInfo);
            }
        }

        private void DrawDiskWindow()
        {
            int yOffset = 30;
            int xOffset = 5;
            int yPosition = windowY + yOffset;

            Pen penBlack = new Pen(Color.Black);
            Pen penGray = new Pen(Color.Gray);
            Pen penBlue = new Pen(Color.LightSkyBlue);
            canvas.DrawFilledRectangle(penBlue, windowX, windowY, windowWidth, windowHeight);
            DrawTitleBar(Color.Gray);

            foreach (var info in disksInfo)
            {
                canvas.DrawString(info, PCScreenFont.Default, penBlack, windowX + xOffset, yPosition);
                yPosition += yOffset;
            }
        }
    }
}
