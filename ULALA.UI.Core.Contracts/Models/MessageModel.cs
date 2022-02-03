using System;
using System.Windows.Input;
using Windows.UI;

namespace ULALA.UI.Core.Contracts.Models
{
    [Flags]
    public enum MessageButtons
    {
        None = 0x0000,
        Ok = 0x0001,
        Cancel = 0x0002,
        Retry = 0x0004,
        Continue = 0x0008,
    }

    public class MessageModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string LTitle { get; set; }
        public string LMessage { get; set; }
        public string Icon { get; set; }
        public Color IconColor { get; set; }
        public MessageButtons Buttons { get; set; }
        public DialogAlert Alert { get; set; }
        public NotificationPosition Position { get; set; }

        public ICommand Command { get; set; }
    }
}
