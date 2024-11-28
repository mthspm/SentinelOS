using Cosmos.System.FileSystem.Listing;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using SentinelOS.Resources.Managers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Cosmos.Core.IOGroup;
using MouseManager = Cosmos.System.MouseManager;
using MouseState = Cosmos.System.MouseState;

namespace SentinelOS.GUI.Windows
{
    public class Explorer : Window
    {
        private List<DirectoryEntry> directoryContent;
        private int highlightedIndex = -1;
        private bool showContextMenu = false;
        private int contextMenuX = 0;
        private int contextMenuY = 0;
        private const int BackArrowSize = 20;
        private const int BackArrowPadding = 10;

        public Explorer(Canvas canvas, int x, int y, int width, int height, string name)
            : base(canvas, x, y, width, height, name)
        {
            directoryContent = new List<DirectoryEntry>();
        }

        private void UpdateDirectoryContent()
        {
            directoryContent = DirectoryManager.GetDirectoryEntries();
        }

        public override void Initialize()
        {
            windowState = WindowState.Running;
        }

        public override void Initialize(string path)
        {
            DirectoryManager.CurrentPath = path;
            UpdateDirectoryContent();
            windowState = WindowState.Running;
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
                        highlightedIndex = -1;
                        break;
                }
            }
        }

        public override void HandleMouseInput()
        {
            HandleEssentialMouseInput();

            if (MouseManager.MouseState == MouseState.Right && IsMouseOver(windowX, windowY, windowWidth, windowHeight))
            {
                showContextMenu = true;
                contextMenuX = (int)MouseManager.X;
                contextMenuY = (int)MouseManager.Y;
            }

            if (MouseManager.MouseState == MouseState.Left)
            {
                if (PreventDoubleClick()) return;
                int arrowX = windowX + BackArrowPadding;
                int arrowY = windowY + 40;
                if (IsMouseOver(arrowX, arrowY, BackArrowSize, BackArrowSize))
                {
                    DirectoryManager.MoveToParentDirectory();
                    UpdateDirectoryContent();
                }
                if (showContextMenu && MouseManager.X >= contextMenuX && MouseManager.X <= contextMenuX + 150 &&
                    MouseManager.Y >= contextMenuY && MouseManager.Y <= contextMenuY + 60)
                {
                    HandleContextMenuSelection((int)MouseManager.X, (int)MouseManager.Y);
                }
                if (highlightedIndex != -1)
                {
                    HandleItemSelection();
                }
                showContextMenu = false;
            }

            HandleMouseHover();
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
                        UpdateDirectoryContent();
                        break;
                }
                showContextMenu = false;
            }
        }

        private void HandleCreateItem(Action<string> createAction, string defaultName)
        {
            var nominationWindow = new NominationWindow(canvas, 100, 100, 300, 100, "NominationWindow", defaultName, createAction, UpdateDirectoryContent);
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

        public void DrawContextMenu(int x, int y, int highlightedOption)
        {
            int width = 150;
            int optionHeight = 20;

            for (int i = 0; i < 3; i++)
            {
                Pen backgroundPen = i == highlightedOption ? new Pen(Color.LightGray) : new Pen(Color.Gray);
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

        private void HandleItemSelection()
        {
            var selectedItem = directoryContent[highlightedIndex];
            if (selectedItem.mEntryType == DirectoryEntryTypeEnum.Directory)
            {
                DirectoryManager.CurrentPath = selectedItem.mFullPath;
                UpdateDirectoryContent();
            }
            else if (selectedItem.mEntryType == DirectoryEntryTypeEnum.File)
            {
                if (selectedItem.mName.EndsWith(".exe"))
                {
                    var cmd = new ConsoleWindow(canvas, 200, 200, 600, 400, "Terminal");
                    WindowManager.AddWindow(cmd);
                    cmd.Initialize();
                    cmd.WriteLine("Executing " + selectedItem.mFullPath);
                }
                else
                {
                    var notepad = new Notepad(canvas, 200, 200, 600, 400, "Notepad");
                    notepad.Initialize(selectedItem.mFullPath);
                    WindowManager.AddWindow(notepad);
                }
            }
        }

        private void HandleMouseHover()
        {
            int startY = windowY + 80;
            int startX = windowX + 10;
            highlightedIndex = -1;
            for (int i = 0; i < directoryContent.Count; i++)
            {
                var currentItem = directoryContent[i];
                var currentItemName = currentItem.mName;
                var currentItemNameSize = currentItemName.Length * 10;
                if (MouseManager.X >= startX && MouseManager.X <= startX + currentItemNameSize && MouseManager.Y >= startY && MouseManager.Y <= startY + 20)
                {
                    highlightedIndex = i;
                    break;
                }
                startY += 20;
            }
        }

        private void DrawBackButton(int x, int y)
        {
            Pen buttonPen = new Pen(Color.White);
            canvas.DrawFilledRectangle(buttonPen, x, y, BackArrowSize, BackArrowSize);
        }

        public override void Draw()
        {
            canvas.DrawFilledRectangle(new Pen(Color.DarkGray), windowX, windowY, windowWidth, windowHeight);
            canvas.DrawFilledRectangle(new Pen(Color.Gray), windowX, windowY, windowWidth, 30);
            DrawTitleBar(Color.Gray);

            int arrowX = windowX + BackArrowPadding;
            int arrowY = windowY + 40;
            DrawBackButton(arrowX, arrowY);

            string currentPath = DirectoryManager.CurrentPath;
            canvas.DrawString(currentPath, PCScreenFont.Default, new Pen(Color.White), windowX + 10, windowY + 10);

            canvas.DrawLine(new Pen(Color.White), windowX, windowY + 30, windowX + windowWidth, windowY + 30);

            DrawDirectoryEntries();
            HandleContextMenu();
        }

        private void DrawDirectoryEntries()
        {
            int startY = windowY + 80;
            int startX = windowX + 10;

            for (int i = 0; i < directoryContent.Count; i++)
            {
                var item = directoryContent[i];
                Pen textPen = i == highlightedIndex ? new Pen(Color.MediumPurple) : new Pen(Color.White);

                if (item.mEntryType == DirectoryEntryTypeEnum.Directory)
                {
                    canvas.DrawFilledRectangle(new Pen(Color.LightBlue), startX, startY, 16, 16);
                }
                else if (item.mEntryType == DirectoryEntryTypeEnum.File)
                {
                    canvas.DrawFilledRectangle(new Pen(Color.LightGreen), startX, startY, 16, 16);
                }
                canvas.DrawString(item.mName, PCScreenFont.Default, textPen, startX + 20, startY);
                startY += 20;
            }
        }

        public override void Run()
        {
            Draw();
            HandleMouseInput();
        }
    }
}