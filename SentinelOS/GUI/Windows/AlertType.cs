using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelOS.GUI.Windows
{
    public enum AlertType
    {
        Info,
        Warning,
        Error,
        Success,
        Question
    }

    public static class AlertTypeExtensions
    {
        /// <summary>
        /// ToString() extension method for AlertType enum
        /// </summary>
        /// <param name="alertType"></param>
        /// <returns></returns>
        public static string ExtendedToString(this AlertType alertType)
        {
            switch (alertType)
            {
                case AlertType.Info:
                    return "Info";
                case AlertType.Warning:
                    return "Warning";
                case AlertType.Error:
                    return "Error";
                case AlertType.Success:
                    return "Success";
                case AlertType.Question:
                    return "Question";
                default:
                    return "Unknown";
            }
        }
    }
}
