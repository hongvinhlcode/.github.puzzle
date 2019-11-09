using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project_big
{
    class SocketManage
    {
        public int Command { get; set; }
        public string Message { get; set; }
        public Point Point { get; set; }
        public SocketManage(int command, string message, Point point)
        {
            this.Command = command;
            this.Message = message;
            this.Point = point;
        }
    }

    public enum SocketCommand
    {
        SEND_POINT,
        NOTIFY,
        NEW_GAME,
        UNDO,
        REDO,
        EXIT
    }
}
