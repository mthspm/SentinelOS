using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics.Fonts;
using MouseManager = Cosmos.System.MouseManager;
using MouseState = Cosmos.System.MouseState;
using SentinelOS.Resources;
using SentinelOS.GUI.Windows;
using SentinelOS.Resources.Managers;

namespace SentinelOS.GUI
{
    public class StartMenu
    {
        private readonly Canvas canvas;
        private readonly int startMenuX = 0;
        private readonly int startMenuY = 0;
        private readonly List<string> menuOptions;
        private bool showStartMenu = false;
        private int highlightedIndex = -1;

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
                        "Terminal",
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
            var centerX = canvas.Mode.Columns / 2;
            var centerY = canvas.Mode.Rows / 2;
            switch (menuOptions[index])
            {
                case "Restart":
                    var questionWindow = new AlertWindow(canvas, centerX, centerY, 300, 100, "Restart", AlertType.Question, "Are you sure you want to restart the system?");
                    questionWindow.SetCallback(SystemCalls.Restart);
                    WindowManager.AddWindow(questionWindow);
                    questionWindow.Initialize();
                    break;
                case "Shutdown":
                    var shutdownQuestion = new AlertWindow(canvas, centerX, centerY, 300, 100, "Shutdown", AlertType.Question, "Are you sure you want to shutdown the system?");
                    shutdownQuestion.SetCallback(SystemCalls.Shutdown);
                    WindowManager.AddWindow(shutdownQuestion);
                    shutdownQuestion.Initialize();
                    break;
                case "Disk Management":
                    var diskManager = new DiskWindow(canvas, 50, 50, canvas.Mode.Columns - 200, canvas.Mode.Rows - 200, "Disk Manager");
                    WindowManager.AddWindow(diskManager);
                    diskManager.Initialize();
                    break;
                case "About System":
                    var aboutWindow = new AboutWindow(canvas, centerX - 150, centerY - 150, 350, 500, "About System");
                    WindowManager.AddWindow(aboutWindow);
                    aboutWindow.Initialize();
                    break;
                case "Task Manager":
                    var taskWindow = new TaskManagerWindow(canvas, 50, 50, 500, 450, "Task Manager");
                    WindowManager.AddWindow(taskWindow);
                    taskWindow.Initialize();
                    break;
                case "Ethernet Connection":
                    var ethernetWindow = new EthernetWindow(canvas, 50, 50, 500, 450, "Ethernet Connection");
                    WindowManager.AddWindow(ethernetWindow);
                    ethernetWindow.Initialize();
                    break;
                case "Terminal":
                    var cmd = new ConsoleWindow(canvas, centerX - 100, centerY - 100, 450, 400, "Terminal");
                    WindowManager.AddWindow(cmd);
                    cmd.Initialize();
                    break;
            }
        }
    }
}
