using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using SentinelOS.Resources.Managers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelOS.GUI.Windows
{
    public class EthernetWindow : Window
    {
        private List<string> ethernetInfo;

        public EthernetWindow(Canvas canvas, int x, int y, int width, int height, string name)
            : base(canvas, x, y, width, height, name)
        {
            ethernetInfo = EthernetManager.GetInfo();
        }

        public override void Initialize()
        {
            windowState = WindowState.Running;
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
            DrawEthernetWindow();
        }

        public override void Run()
        {
            Draw();
            HandleMouseInput();
            HandleKeyPress();
        }

        private void DrawEthernetWindow()
        {
            int yOffset = 30;
            int xOffset = 5;
            int yPosition = windowY + yOffset;

            Pen penBlack = new Pen(Color.Black);
            Pen penGray = new Pen(Color.Gray);
            Pen backgroundColor = new Pen(Color.LightSkyBlue);
            canvas.DrawFilledRectangle(backgroundColor, windowX, windowY, windowWidth, windowHeight);
            DrawTitleBar(Color.Gray);

            foreach (var line in ethernetInfo)
            {
                canvas.DrawString(line, PCScreenFont.Default, penBlack, windowX + xOffset, yPosition);
                yPosition += yOffset;
            }
            
        }
    }
}
