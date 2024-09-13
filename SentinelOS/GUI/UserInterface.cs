using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using IL2CPU.API.Attribs;
using Cosmos.Core.Memory;
using MouseManager = Cosmos.System.MouseManager;
using MouseState = Cosmos.System.MouseState;
using Cosmos.Core;
using Cosmos.System.Graphics.Fonts;
using Cosmos.System.FileSystem.Listing;
using Cosmos.System.FileSystem.VFS;
using SentinelOS.Resources;

namespace SentinelOS.GUI
{
    class UserInterface
    {
        private Canvas canvas;
        private bool showContextMenu = false;
        private int contextMenuX = 0;
        private int contextMenuY = 0;
        private int highlightedIndex = -1;
        private Notepad notepad;
        private List<DirectoryEntry> directoryContents;

        // Resources Setup
        [ManifestResourceStream(ResourceName = "SentinelOS.Dependencies.cursor.bmp")] private static byte[] cursor;
        [ManifestResourceStream(ResourceName = "SentinelOS.Dependencies.osicon.bmp")] private static byte[] osIcon;
        [ManifestResourceStream(ResourceName = "SentinelOS.Dependencies.file.bmp")] private static byte[] file;
        [ManifestResourceStream(ResourceName = "SentinelOS.Dependencies.folder.bmp")] private static byte[] folder;
        [ManifestResourceStream(ResourceName = "SentinelOS.Dependencies.network.bmp")] private static byte[] network;
        public static Bitmap cursorBitmap = new Bitmap(cursor);
        public static Bitmap osIconBitmap = new Bitmap(osIcon);
        public static Bitmap fileBitmap = new Bitmap(file);
        public static Bitmap folderBitmap = new Bitmap(folder);
        public static Bitmap networkBitmap = new Bitmap(network);

        // Public Constructor for the GUI
        public UserInterface(Canvas canvas)
        {
            this.canvas = canvas;
            this.notepad = new Notepad(canvas);
        }

        public void Initialize()
        {
            directoryContents = VFSManager.GetDirectoryListing(DirectoryManager.CurrentPath);
        }

        public void DrawUserInterface()
        {
            canvas.Clear(Color.Aqua);
            DrawTaskbar();
            DrawHourAndDate();
            DrawDirectoryContents(directoryContents);
            if (showContextMenu)
            {
                int relativeY = (int)MouseManager.Y - contextMenuY;
                int highlightedOption = relativeY / 20;
                DrawContextMenu(contextMenuX, contextMenuY, highlightedOption);
            }
            DrawCursor((int)MouseManager.X, (int)MouseManager.Y);
        }

        public void HandleMouseInput()
        {
            HandleMouseHover();

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
                else
                {
                    showContextMenu = false;
                    HandleSysFileExecution();
                }
            }
        }

        private void HandleSysFileExecution()
        {
            if (highlightedIndex != -1)
            {
                var selectedItem = directoryContents[highlightedIndex];
                if (selectedItem.mEntryType == DirectoryEntryTypeEnum.Directory)
                {
                    DirectoryManager.CurrentPath = selectedItem.mFullPath;
                    Refresh();
                }
                else if (selectedItem.mEntryType == DirectoryEntryTypeEnum.File)
                {
                    notepad.Initialize(selectedItem.mFullPath);
                } 
            }
        }

        private void HandleCreateItem(Action<string> createAction)
        {
            string itemName = HandleFileNomination();
            if (itemName != null)
            {
                createAction(itemName);
                Refresh();
            }
        }

        private string HandleFileNomination()
        {
            string fileName = "NewFile";
            bool isNaming = true;

            while (isNaming)
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
            }

            return fileName;
        }

        private void Refresh()
        {
            directoryContents = VFSManager.GetDirectoryListing(DirectoryManager.CurrentPath);
            DrawUserInterface();
        }

        public void DrawTaskbar()
        {
            canvas.DrawFilledRectangle(new Pen(Color.Black), 0, 690, 1280, 30);
            canvas.DrawImage(osIconBitmap, 0, 690);
        }

        public void DrawCursor(int x, int y)
        {
            canvas.DrawImageAlpha(cursorBitmap, x, y);
        }

        private void HandleMouseHover()
        {
            int startY = 50;
            highlightedIndex = -1;

            for (int i = 0; i < directoryContents.Count; i++)
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
                        HandleCreateItem(DirectoryManager.CreateDir);
                        break;
                    case 1:
                        HandleCreateItem(FileManager.CreateEmptyFile);
                        break;
                    case 2:
                        Refresh();
                        break;
                }

                showContextMenu = false;
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

            canvas.Display();
        }

        public void DrawDirectoryContents(List<DirectoryEntry> directoryContents)
        {
            int startY = 50;
            for (int i = 0; i < directoryContents.Count; i++)
            {
                var item = directoryContents[i];
                Pen textPen = (i == highlightedIndex) ? new Pen(Color.White) : new Pen(Color.LightBlue);
                canvas.DrawString(item.mFullPath, PCScreenFont.Default, textPen, 50, startY);
                //TODO : Draw sysFiles icons
                startY += 30;
            }
        }
    }
}
