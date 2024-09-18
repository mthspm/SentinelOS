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

namespace SentinelOS.GUI
{
    /// <summary>
    /// Represents the User Interface of the SentinelOS
    /// </summary>
    class UserInterface
    {
        private Canvas canvas;
        private bool showContextMenu = false;
        private int contextMenuX = 0;
        private int contextMenuY = 0;
        private int highlightedIndex = -1;
        private StartMenu startMenu;
        private WindowManager windowManager;
        private List<DirectoryEntry> directoryContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInterface"/> class.
        /// </summary>
        public UserInterface(Canvas canvas)
        {
            this.canvas = canvas;
            this.startMenu = new StartMenu(canvas);
            this.windowManager = new WindowManager(canvas);
            this.directoryContent = VFSManager.GetDirectoryListing(DirectoryManager.CurrentPath);
        }

        private void DrawUserInterface()
        {
            //canvas.Clear(Color.Aqua);
            DrawBackground();
            DrawTaskbar();
            DrawHourAndDate();
            DrawDesktopContents();
            HandleMouseHover();
            HandleContextMenu();
            startMenu.HandleDrawStartMenu();
            windowManager.Run();
            DrawCursor((int)MouseManager.X, (int)MouseManager.Y); // Draw cursor at the end to be on top of everything
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

        private void HandleMouseInput()
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
            var selectedItem = directoryContent[highlightedIndex];
            if (selectedItem.mEntryType == DirectoryEntryTypeEnum.Directory)
            {
                DirectoryManager.CurrentPath = selectedItem.mFullPath;
                Refresh();
            }
            else if (selectedItem.mEntryType == DirectoryEntryTypeEnum.File)
            {
                var notepad = new Notepad(canvas, 100, 100, 600, 400, "Notepad");
                notepad.Initialize(selectedItem.mFullPath);
                windowManager.AddWindow(notepad);
            }
        }

        //private void HandleCreateItem(Action<string> createAction, string name)
        //{
        //    string itemName = HandleFileNomination(name);
        //    if (itemName != null)
        //    {
        //        createAction(itemName);
        //        Refresh();
        //    }
        //}

        private void HandleCreateItem(Action<string> createAction, string name)
        {
            var nominationWindow = new NominationWindow(canvas, 100, 100, 300, 100, "NominationWindow", name, (itemName) =>
            {
                createAction(itemName);
                Refresh();
            });

            windowManager.AddWindow(nominationWindow);
            nominationWindow.Initialize();
        }

        private string HandleFileNomination(string nameBase)
        {
            string fileName = nameBase;
            bool isNaming = true;
            NameInputHandler:
                if (isNaming)
                {
                    DrawNominationWindow(fileName);
                    var keyInfo = Console.ReadKey(true);
                    switch (keyInfo)
                    {
                        case { Key: ConsoleKey.Enter }:
                            isNaming = false;
                            break;
                        case { Key: ConsoleKey.Escape }:
                            return null;
                        case { Key: ConsoleKey.Backspace } when fileName.Length > 0:
                            fileName = fileName.Substring(0, fileName.Length - 1);
                            break;
                        case { KeyChar: not '\0' }:
                            fileName += keyInfo.KeyChar;
                            break;
                    }
                    if (isNaming)
                    {
                    goto NameInputHandler;
                    }
                }
            return fileName;
        }

        private void Refresh()
        {
            directoryContent = VFSManager.GetDirectoryListing(DirectoryManager.CurrentPath);
            DrawUserInterface();
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

            for (int i = 0; i < directoryContent.Count; i++)
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
                    0 => "Criar Pasta",
                    1 => "Criar Arquivo",
                    2 => "Atualizar",
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
                        var folderName = "New Folder";
                        HandleCreateItem(DirectoryManager.CreateDir, folderName);
                        break;
                    case 1:
                        var fileName = "New File";
                        HandleCreateItem(FileManager.CreateEmptyFile, fileName);
                        break;
                    case 2:
                        Refresh();
                        break;
                }

                showContextMenu = false;
            }
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

        public void DrawNominationWindow(string fileName)
        {
            int windowWidth = 300;
            int windowHeight = 100;
            int windowX = (canvas.Mode.Columns - windowWidth) / 2;
            int windowY = (canvas.Mode.Rows - windowHeight) / 2;

            canvas.DrawFilledRectangle(new Pen(Color.White), windowX, windowY, windowWidth, windowHeight);
            canvas.DrawRectangle(new Pen(Color.Black), windowX, windowY, windowWidth, windowHeight);
            canvas.DrawString("Nome do Arquivo/Pasta:", PCScreenFont.Default, new Pen(Color.Black), windowX + 10, windowY + 10);

            canvas.DrawFilledRectangle(new Pen(Color.White), windowX + 10, windowY + 40, windowWidth - 20, 20);
            canvas.DrawString(fileName, PCScreenFont.Default, new Pen(Color.Black), windowX + 10, windowY + 40);

            canvas.Display(); // NEED TO REMOVE THIS
        }

        public void DrawDesktopContents()
        {
            int startY = 50;
            int startX = 30;

            for (int i = 0; i < directoryContent.Count; i++)
            {
                var item = directoryContent[i];
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
