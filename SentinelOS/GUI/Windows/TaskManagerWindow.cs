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
    public class TaskManagerWindow : Window
    {
        private List<OSTask> tasks;

        public TaskManagerWindow(Canvas canvas, int x, int y, int width, int height, string name)
            : base(canvas, x, y, width, height, name)
        {
            tasks = new List<OSTask>();
        }

        public override void Initialize()
        {
            windowState = WindowState.Running;
            CollectTasksInformation();
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
            DrawTaskWindow();
        }

        public override void Run()
        {
            Draw();
            HandleMouseInput();
            HandleKeyPress();
            PeriodicUpdateTask();
        }

        private void PeriodicUpdateTask()
        {
            CollectTasksInformation();
        }

        private void CollectTasksInformation()
        {
            this.tasks = TaskManager.ListTasks();
        }

        private void DrawTaskWindow()
        {
            int yOffset = 30;
            int xOffset = 5;
            int yPosition = windowY + yOffset;

            Pen penBlack = new Pen(Color.Black);
            Pen penGray = new Pen(Color.Gray);
            Pen backgroundColor = new Pen(Color.LightSkyBlue);
            canvas.DrawFilledRectangle(backgroundColor, windowX, windowY, windowWidth, windowHeight);
            DrawTitleBar(Color.Gray);

            foreach (var task in tasks)
            {
                string text = $"PID: {task.PID} | Name: {task.Name} | Parent PID: {task.ParentPID}";
                List<string> wrappedText = WrapTextToFitWindow(text);
                foreach (var line in wrappedText)
                {
                    canvas.DrawString(line, PCScreenFont.Default, penBlack, windowX + xOffset, yPosition);
                    yPosition += yOffset;
                }
            }
        }
    }
}
