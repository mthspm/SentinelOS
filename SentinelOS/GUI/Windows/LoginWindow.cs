using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Drawing;
using Cosmos.System;
using SentinelOS.Resources;
using Console = System.Console;

namespace SentinelOS.GUI.Windows
{
    public class LoginWindow : Window
    {
        private string username = string.Empty;
        private string password = string.Empty;
        private bool isPasswordField = false;
        private bool isFadingOut = false;
        private bool isFadingIn = true;
        private int fadeStep = 0;
        private const int FadeSteps = 160;
        private string errorMessage = string.Empty;

        private readonly Rectangle loginButtonRect;
        private readonly Rectangle usernameFieldRect;
        private readonly Rectangle passwordFieldRect;
        private readonly Rectangle powerButtonRect;
        private readonly Rectangle powerOptionsRect;
        private bool showPowerOptions = false;
        private int highlightedPowerOption = -1;

        public LoginWindow(Canvas canvas, int x, int y, int width, int height, string name)
            : base(canvas, x, y, width, height, name)
        {
            int centerX = x + width / 2;
            int centerY = y + height / 2;

            loginButtonRect = new Rectangle(centerX - 100, centerY + 60, 200, 30);
            usernameFieldRect = new Rectangle(centerX - 100, centerY - 40, 200, 20);
            passwordFieldRect = new Rectangle(centerX - 100, centerY + 10, 200, 20);
            powerButtonRect = new Rectangle(x + width - 30, y + height - 30, 20, 20);
            powerOptionsRect = new Rectangle(x + width - 130, y + height - 90, 100, 60);
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

        private void EraseLastChar()
        {
            if (isPasswordField && password.Length > 0)
            {
                password = password.Substring(0, password.Length - 1);
            }
            else if (!isPasswordField && username.Length > 0)
            {
                username = username.Substring(0, username.Length - 1);
            }
        }

        private void InsertPressedKey(char keyChar)
        {
            if (isPasswordField)
            {
                password += keyChar;
            }
            else
            {
                username += keyChar;
            }
        }

        public override void HandleKeyPress()
        {
            if (!Console.KeyAvailable)
            {
                return;
            }
            var keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.Escape:
                    windowState = WindowState.ToClose;
                    break;
                case ConsoleKey.Tab:
                    isPasswordField = !isPasswordField;
                    break;
                case ConsoleKey.Enter:
                    ValidateLogin();
                    break;
                case ConsoleKey.Backspace:
                    EraseLastChar();
                    break;
                default:
                    if (!char.IsControl(keyInfo.KeyChar))
                    {
                        InsertPressedKey(keyInfo.KeyChar);
                    }
                    break;
            }
        }

        public override void HandleMouseInput()
        {
            HandleEssentialMouseInput();

            if (MouseManager.MouseState == MouseState.Left)
            {
                if (IsMouseOver(loginButtonRect))
                {
                    ValidateLogin();
                }
                else if (IsMouseOver(usernameFieldRect))
                {
                    isPasswordField = false;
                }
                else if (IsMouseOver(passwordFieldRect))
                {
                    isPasswordField = true;
                }
                else if (IsMouseOver(powerButtonRect))
                {
                    showPowerOptions = !showPowerOptions;
                }
                else if (showPowerOptions)
                {
                    if (IsMouseOver(new Rectangle(powerOptionsRect.X, powerOptionsRect.Y, powerOptionsRect.Width, 20)))
                    {
                        SystemCalls.Restart();
                    }
                    else if (IsMouseOver(new Rectangle(powerOptionsRect.X, powerOptionsRect.Y + 20, powerOptionsRect.Width, 20)))
                    {
                        SystemCalls.Shutdown();
                    }
                }
            }

            if (showPowerOptions)
            {
                if (IsMouseOver(new Rectangle(powerOptionsRect.X, powerOptionsRect.Y, powerOptionsRect.Width, 20)))
                {
                    highlightedPowerOption = 0;
                }
                else if (IsMouseOver(new Rectangle(powerOptionsRect.X, powerOptionsRect.Y + 20, powerOptionsRect.Width, 20)))
                {
                    highlightedPowerOption = 1;
                }
                else
                {
                    highlightedPowerOption = -1;
                }
            }
        }

        private void ValidateLogin()
        {
            if (username == "admin" && password == "password")
            {
                isFadingOut = true;
                fadeStep = 0; // Reset fade step for fade-out
            }
            else
            {
                errorMessage = "Incorrect username or password.";
                username = string.Empty;
                password = string.Empty;
            }
        }

        public override void Draw()
        {
            if (isFadingOut)
            {
                fadeStep++;
                if (fadeStep > FadeSteps)
                {
                    windowState = WindowState.ToClose;
                    return;
                }
            }
            else if (isFadingIn)
            {
                fadeStep++;
                if (fadeStep > FadeSteps)
                {
                    isFadingIn = false;
                }
            }

            int alpha = isFadingOut ? 255 - (fadeStep * 255 / FadeSteps) : (isFadingIn ? fadeStep * 255 / FadeSteps : 255);
            Color backgroundColor = Color.FromArgb(alpha, Color.DarkSlateGray);
            Color textColor = Color.FromArgb(alpha, Color.White);
            Color buttonColor = Color.FromArgb(alpha, Color.Gray);

            int centerX = windowX + windowWidth / 2;
            int centerY = windowY + windowHeight / 2;

            // Draw background
            canvas.DrawFilledRectangle(new Pen(backgroundColor), windowX, windowY, windowWidth, windowHeight);

            // Draw logo
            canvas.DrawString("SentinelOS", PCScreenFont.Default, new Pen(textColor), centerX - 50, centerY - 100);

            // Draw username field
            canvas.DrawString("Username:", PCScreenFont.Default, new Pen(textColor), centerX - 100, centerY - 60);
            canvas.DrawRectangle(new Pen(textColor), centerX - 100, centerY - 40, 200, 20);
            canvas.DrawString(username, PCScreenFont.Default, new Pen(textColor), centerX - 95, centerY - 40);

            // Draw password field
            canvas.DrawString("Password:", PCScreenFont.Default, new Pen(textColor), centerX - 100, centerY - 10);
            canvas.DrawRectangle(new Pen(textColor), centerX - 100, centerY + 10, 200, 20);
            canvas.DrawString(new string('*', password.Length), PCScreenFont.Default, new Pen(textColor), centerX - 95, centerY + 10);

            // Draw login button
            canvas.DrawFilledRectangle(new Pen(buttonColor), centerX - 100, centerY + 60, 200, 30);
            canvas.DrawString("Login", PCScreenFont.Default, new Pen(textColor), centerX - 20, centerY + 65);

            // Draw error message
            if (!string.IsNullOrEmpty(errorMessage))
            {
                canvas.DrawString(errorMessage, PCScreenFont.Default, new Pen(Color.Red), centerX - 100, centerY + 100);
            }

            // Draw instructions
            canvas.DrawString("Press Enter to submit", PCScreenFont.Default, new Pen(textColor), centerX - 100, centerY + 140);

            // Draw pointer
            if (isPasswordField)
            {
                canvas.DrawString(">", PCScreenFont.Default, new Pen(textColor), centerX - 120, centerY + 10);
            }
            else
            {
                canvas.DrawString(">", PCScreenFont.Default, new Pen(textColor), centerX - 120, centerY - 40);
            }

            // Draw power button
            canvas.DrawFilledRectangle(new Pen(Color.Red), powerButtonRect.X, powerButtonRect.Y, powerButtonRect.Width, powerButtonRect.Height);
            canvas.DrawString("P", PCScreenFont.Default, new Pen(Color.White), powerButtonRect.X + 5, powerButtonRect.Y + 5);

            // Draw power options
            if (showPowerOptions)
            {
                for (int i = 0; i < 2; i++)
                {
                    Pen backgroundPen = i == highlightedPowerOption ? new Pen(Color.LightGray) : new Pen(Color.Gray);
                    Pen textPen = new Pen(Color.Black);

                    canvas.DrawFilledRectangle(backgroundPen, powerOptionsRect.X, powerOptionsRect.Y + i * 20, powerOptionsRect.Width, 20);

                    string optionText = i switch
                    {
                        0 => "Restart",
                        1 => "Shutdown",
                        _ => ""
                    };

                    canvas.DrawString(optionText, PCScreenFont.Default, textPen, powerOptionsRect.X + 5, powerOptionsRect.Y + i * 20 + 5);
                }
            }
        }

        public override void Run()
        {
            Draw();
            HandleMouseInput();
            HandleKeyPress();
        }
    }
}
