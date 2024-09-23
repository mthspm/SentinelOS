using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using MouseManager = Cosmos.System.MouseManager;
using MouseState = Cosmos.System.MouseState;
using Cosmos.System.Graphics.Fonts;
using Cosmos.System.FileSystem.Listing;
using Cosmos.System.FileSystem.VFS;
using SentinelOS.Resources;
using System.Reflection.Emit;
using SentinelOS.Windows;

namespace SentinelOS.GUI
{
    /// <summary>
    /// Class that handles the User Graphical Interface of the SentinelOS
    /// </summary>
    class UserInterface
    {
        private readonly Canvas canvas;
        private bool showContextMenu = false;
        private int contextMenuX = 0;
        private int contextMenuY = 0;
        private int highlightedIndex = -1;
        private StartMenu startMenu;
        private List<DirectoryEntry> desktopDirectoryEntries;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInterface"/> class.
        /// </summary>
        public UserInterface(Canvas canvas)
        {
            this.canvas = canvas;
            this.startMenu = new StartMenu(canvas);
            WindowManager.Initialize();
            this.desktopDirectoryEntries = DirectoryManager.GetDirectoryEntries(Paths.Desktop);
        }

        public void DrawUserInterface()
        {
            //canvas.Clear(Color.Aqua);
            DrawBackground();
            DrawTaskbar();
            DrawHourAndDate();
            DrawDesktopContents();
            HandleMouseHover();
            HandleContextMenu();
            startMenu.HandleDrawStartMenu();
        }

        public void Run()
        {
            DrawUserInterface();
            HandleMouseInput();
        }

        private void DrawBackground()
        {
            canvas.DrawImage(Resources.backgroundBitmap, 0, 0);
        }

        public void HandleMouseInput()
        {
            if (MouseManager.MouseState == MouseState.Right && MouseManager.Y < 690) // Outside the taskbar
            {
                showContextMenu = true;
                contextMenuX = (int)MouseManager.X;
                contextMenuY = (int)MouseManager.Y;
            }

            if (MouseManager.MouseState == MouseState.Left)
            {
                if (showContextMenu && (MouseManager.X >= contextMenuX && MouseManager.X <= contextMenuX + 150 && MouseManager.Y >= contextMenuY && MouseManager.Y <= contextMenuY + 60))
                {
                    HandleContextMenuSelection((int)MouseManager.X, (int)MouseManager.Y);
                }
                if(highlightedIndex != -1)
                {
                    HandleSysFileExecution();
                }
                showContextMenu = false;
            }

            startMenu.HandleMouseInput();

        }

        private void HandleSysFileExecution()
        {
            var selectedItem = desktopDirectoryEntries[highlightedIndex];
            if (selectedItem.mEntryType == DirectoryEntryTypeEnum.Directory)
            {
                var explorer = new Explorer(canvas, 100, 100, 800, 600, "Explorer");
                explorer.Initialize(selectedItem.mFullPath);
                WindowManager.AddWindow(explorer);
            }
            else if (selectedItem.mEntryType == DirectoryEntryTypeEnum.File)
            {
                var notepad = new Notepad(canvas, 100, 100, 600, 400, "Notepad");
                notepad.Initialize(selectedItem.mFullPath);
                WindowManager.AddWindow(notepad);
            }
        }

        private void Refresh()
        {
            desktopDirectoryEntries = DirectoryManager.GetDirectoryEntries(Paths.Desktop);
        }

        public void DrawTaskbar()
        {
            canvas.DrawFilledRectangle(new Pen(Color.Black), 0, 690, 1280, 30);
            canvas.DrawImage(Resources.osIconBitmap, 0, 690);
        }

        public void DrawCursor(int x, int y)
        {
            canvas.DrawImageAlpha(Resources.cursorBitmap, x, y);
        }

        private void HandleMouseHover()
        {
            int startY = 50;
            highlightedIndex = -1;

            for (int i = 0; i < desktopDirectoryEntries.Count; i++)
            {
                if (MouseManager.X >= 50 && MouseManager.X <= 300 && MouseManager.Y >= startY && MouseManager.Y <= startY + 30)
                {
                    highlightedIndex = i;
                    break;
                }
                startY += 30;
            }
        }

        public void DrawHourAndDate()
        {
            DateTime now = DateTime.Now;
            string hour = now.Hour.ToString();
            string minute = now.Minute.ToString();
            string second = now.Second.ToString();
            string day = now.Day.ToString();
            string month = now.Month.ToString();
            string year = now.Year.ToString();

            Pen pen = new Pen(Color.White);
            canvas.DrawString(hour + ":" + minute + ":" + second, PCScreenFont.Default, pen, 1200, 690);
            canvas.DrawString(day + "/" + month + "/" + year, PCScreenFont.Default, pen, 1200, 705);
        }

        public void DrawContextMenu(int x, int y, int highlightedOption)
        {
            int width = 150;
            int optionHeight = 20;

            for (int i = 0; i < 3; i++)
            {
                Pen backgroundPen = (i == highlightedOption) ? new Pen(Color.LightGray) : new Pen(Color.Gray);
                Pen textPen = new Pen(Color.Black);

                canvas.DrawFilledRectangle(backgroundPen, x, y + i * optionHeight, width, optionHeight);

                string optionText = i switch
                {
                    0 => "New Folder",
                    1 => "New File",
                    2 => "Refresh",
                    _ => ""
                };

                canvas.DrawString(optionText, PCScreenFont.Default, textPen, x + 5, y + i * optionHeight + 5);
            }
        }

        private void HandleContextMenuSelection(int x, int y)
        {
            if (showContextMenu && x >= contextMenuX && x <= contextMenuX + 150 && y >= contextMenuY && y <= contextMenuY + 60)
            {
                int relativeY = y - contextMenuY;
                int optionIndex = relativeY / 20;
                switch (optionIndex)
                {
                    case 0:
                        HandleCreateItem(DirectoryManager.CreateDir, "New Folder");
                        break;
                    case 1:
                        HandleCreateItem(FileManager.CreateEmptyFile, "New File");
                        break;
                    case 2:
                        Refresh();
                        break;
                }
                showContextMenu = false;
            }
        }

        private void HandleCreateItem(Action<string> createAction, string defaultName)
        {
            var nominationWindow = new NominationWindow(canvas, 100, 100, 300, 100, "NominationWindow", defaultName, createAction);
            WindowManager.AddWindow(nominationWindow);
            nominationWindow.Initialize();
        }

        private void HandleContextMenu()
        {
            if (showContextMenu)
            {
                int width = 150;
                int optionHeight = 20;
                int menuHeight = 3 * optionHeight;

                if (MouseManager.X >= contextMenuX && MouseManager.X <= contextMenuX + width &&
                    MouseManager.Y >= contextMenuY && MouseManager.Y <= contextMenuY + menuHeight)
                {
                    int relativeY = (int)MouseManager.Y - contextMenuY;
                    int highlightedOption = relativeY / optionHeight;
                    DrawContextMenu(contextMenuX, contextMenuY, highlightedOption);
                }
                else
                {
                    DrawContextMenu(contextMenuX, contextMenuY, -1);
                }
            }
        }

        public void DrawDesktopContents()
        {
            int startY = 50;
            int startX = 30;

            for (int i = 0; i < desktopDirectoryEntries.Count; i++)
            {
                var item = desktopDirectoryEntries[i];
                Pen textPen;

                textPen = (i == highlightedIndex) ? new Pen(Color.MediumPurple) : new Pen(Color.White);

                canvas.DrawString(item.mFullPath, PCScreenFont.Default, textPen, startX, startY);
                //TODO : Draw Icons
                startY += 30;
                if (startY >= 690)
                {
                    startY = 50;
                    startX += 300;
                }
            }
        }
    }
}
