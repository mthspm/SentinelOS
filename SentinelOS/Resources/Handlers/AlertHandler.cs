using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.System.Graphics;
using SentinelOS.Resources;
using SentinelOS.GUI.Windows;

namespace SentinelOS.Resources.Handlers
{
    /// <summary>
    /// The error handler class that contains methods to display alerts on the screen.
    /// </summary>
    public static class AlertHandler
    {
        private static Canvas Canvas;
        private static int center_x = 1280 / 2;
        private static int center_y = 720 / 2;

        /// <summary>
        /// Initializes the error handler with the specified canvas to display alerts on.
        /// </summary>
        /// <param name="canvas"></param>
        public static void Initialize(Canvas canvas)
        {
            Canvas = canvas;
        }

        /// <summary>
        /// Displays an alert window with the specified alert type and message.
        /// <para>See <see cref="AlertType"/> for available alert types.</para>
        /// </summary>
        /// <param name="alertType">The type of alert to display.</param>
        /// <param name="message">The message to display in the alert window.</param>
        public static void DisplayAlert(AlertType alertType, string message, int width=300, int height=100)
        {
            AlertWindow alertWindow = new AlertWindow(Canvas, center_x, center_y, width, height, "Alert", alertType, message);
            WindowManager.AddWindow(alertWindow);
            alertWindow.Initialize();
        }
    }
}
