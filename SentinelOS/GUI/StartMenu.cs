using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics.Fonts;
using MouseManager = Cosmos.System.MouseManager;
using MouseState = Cosmos.System.MouseState;
using SentinelOS.Resources;

namespace SentinelOS.GUI
{
    public class StartMenu
    {
        private Canvas canvas;
        private bool showStartMenu = false;
        private int startMenuX = 0;
        private int startMenuY = 0;
        private int highlightedIndex = -1;
        private List<string> menuOptions;

        public StartMenu(Canvas canvas)
        {
            this.canvas = canvas;
            this.startMenuX = 0;
            this.startMenuY = canvas.Mode.Rows - 60;
            this.menuOptions = new List<string>
                    {
                        "Shutdown",
                        "Restart",
                        "About System",
                        "Ethernet Connection",
                        "Disk Management",
                        "Task Manager",
                        "Terminal"
                    };
        }

        public void HandleDrawStartMenu()
        {
            if (showStartMenu)
            {
                int width = 200;
                int optionHeight = 30;

                for (int i = 0; i < menuOptions.Count; i++)
                {
                    Pen backgroundPen = (i == highlightedIndex) ? new Pen(Color.LightGray) : new Pen(Color.Gray);
                    Pen textPen = new Pen(Color.Black);

                    canvas.DrawFilledRectangle(backgroundPen, startMenuX, startMenuY - i * optionHeight, width, optionHeight);
                    canvas.DrawString(menuOptions[i], PCScreenFont.Default, textPen, startMenuX + 5, startMenuY - i * optionHeight + 5);
                }
            }
        }

        public void HandleMouseInput()
        {
            if (MouseManager.MouseState == MouseState.Left && MouseManager.X >= 0 && MouseManager.X <= 35 && MouseManager.Y >= 690 && MouseManager.Y <= 728)
            {
                showStartMenu = !showStartMenu;
            }

            if (showStartMenu)
            {
                int width = 200;
                int optionHeight = 30;
                int yOffset = 30;

                if (MouseManager.X >= startMenuX && MouseManager.X <= startMenuX + width &&
                    MouseManager.Y >= startMenuY - menuOptions.Count * optionHeight + yOffset && MouseManager.Y <= startMenuY + yOffset)
                {
                    int relativeY = startMenuY - (int)MouseManager.Y + yOffset;

                    highlightedIndex = relativeY / optionHeight;

                    if (MouseManager.MouseState == MouseState.Left && highlightedIndex >= 0 && highlightedIndex < menuOptions.Count)
                    {
                        HandleMenuSelection(highlightedIndex);
                        showStartMenu = false;
                    }
                }
                else
                {
                    highlightedIndex = -1;
                }
            }
        }

        private void HandleMenuSelection(int index)
        {
            switch (menuOptions[index])
            {
                case "Control Panel":
                    // Control Panel
                    break;
                case "Ethernet Connection":
                    // Ethernet Connection
                    break;
                case "Disk Management":
                    var diskManager = new DiskWindow(canvas, 50, 50, canvas.Mode.Columns - 200, canvas.Mode.Rows - 200, "Disk Manager");
                    WindowManager.AddWindow(diskManager);
                    diskManager.Initialize();
                    break;
                    break;
                case "Task Manager":
                    // Task Manager
                    break;
                case "Restart":
                    SystemCalls.Restart();
                    break;
                case "Shutdown":
                    SystemCalls.Shutdown();
                    break;
                case "Terminal":
                    // Command Prompt
                    break;
            }
        }
    }
}
