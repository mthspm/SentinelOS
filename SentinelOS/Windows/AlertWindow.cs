using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using Point = Cosmos.System.Graphics.Point;
using Console = System.Console;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelOS.Windows
{
    public class AlertWindow : Window
    {
        private AlertType alertType;
        private List<string> alertMessage;
        private Pen alertWindowColor;
        private bool? questionResult;
        private Rectangle confirmButton;
        private Rectangle denyButton;

        public AlertWindow(Canvas canvas, int x, int y, int width, int height, string name, AlertType alertType, string alertMessage)
            : base(canvas, x, y, width, height, name)
        {
            this.alertType = alertType;
            this.alertMessage = WrapTextToFitWindow(alertMessage);
            this.name = name + " - " + alertType.ExtendedToString();
        }

        public override void Initialize()
        {
            alertWindowColor = GetAlertWindowColor();
            if (alertType == AlertType.Question)
            {
                questionResult = null;
                InitializeButtons();
            }
            windowState = WindowState.Running;
        }

        public override void Initialize(string path)
        {
            // Not needed for this window
        }

        public override void CheckWindowStateChanges()
        {
            if (alertType == AlertType.Question)
            {
                if (isDragging || isMinimized || isMaximized)
                {
                    InitializeButtons();
                }
            }
        }

        public override void HandleKeyPress()
        {
            if (Console.KeyAvailable)
            {
                var keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.Enter:
                    case ConsoleKey.Escape:
                        windowState = WindowState.ToClose;
                        break;
                }
            }
        }

        public override void HandleMouseInput()
        {
            HandleEssentialMouseInput(); // Handle closing by clicking on the close button and dragging
            CheckWindowStateChanges();

            if (alertType == AlertType.Question)
            {
                var mouseState = MouseManager.MouseState;
                if (mouseState == MouseState.Left)
                {
                    var mouseX = (int)MouseManager.X;
                    var mouseY = (int)MouseManager.Y;

                    if (confirmButton.Contains(mouseX, mouseY))
                    {
                        questionResult = true;
                        windowState = WindowState.ToClose;
                    }
                    else if (denyButton.Contains(mouseX, mouseY))
                    {
                        questionResult = false;
                        windowState = WindowState.ToClose;
                    }
                }
            }
        }

        public override void Draw()
        {
            DrawAlertWindow();
            DrawTitleBar(Color.Gray);
            if (alertType == AlertType.Question)
            {
                DrawConfirmationButtons();
            }
        }

        public override void Run()
        {
            Draw();
            HandleMouseInput();
        }

        private void DrawAlertWindow()
        {
            int yOffSet = 15;
            int yPosition = windowY + yOffSet;
            canvas.DrawFilledRectangle(alertWindowColor, windowX, windowY, windowWidth, windowHeight);
            canvas.DrawRectangle(new Pen(Color.Black), windowX, windowY, windowWidth, windowHeight);
            foreach (string line in alertMessage)
            {
                canvas.DrawString(line, PCScreenFont.Default, new Pen(Color.Black), windowX + 5, yPosition);
                yPosition += yOffSet;
            }

        }

        private Pen GetAlertWindowColor()
        {
            switch (alertType)
            {
                case AlertType.Error:
                    return new Pen(Color.Red);
                case AlertType.Warning:
                    return new Pen(Color.Yellow);
                case AlertType.Success:
                    return new Pen(Color.Green);
                case AlertType.Question:
                    return new Pen(Color.AliceBlue);
                default:
                    return new Pen(Color.Gray);
            }
        }

        private void InitializeButtons()
        {
            int buttonWidth = 100;
            int buttonHeight = 30;
            int buttonY = windowY + windowHeight - buttonHeight - 10;

            confirmButton = new Rectangle(windowX + 10, buttonY, buttonWidth, buttonHeight);
            denyButton = new Rectangle(windowX + windowWidth - buttonWidth - 10, buttonY, buttonWidth, buttonHeight);
        }

        private void DrawConfirmationButtons()
        {
            DrawButton(confirmButton, Color.Green, Color.Black, "Confirmar");
            DrawButton(denyButton, Color.Red, Color.Black, "Cancelar");
        }
    }
}
