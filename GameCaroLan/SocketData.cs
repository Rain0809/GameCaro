using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCarobyhuy
{
    [Serializable]
    public class SocketData
    {
       
        private int commad;
        private Point point;
        
        public int Commad { get => commad; set => commad = value; }
        public Point Point { get => point; set => point = value; }
        public string Message { get => message; set => message = value; }

        public SocketData(int commad, string message, Point point)
        {
                this.Commad = commad;   
                this.Point = point;
                this.Message = message;
        }
        private string message;
    }
    public enum SocketCommad
    {
        SEND_POINT,
        NOTIFY,
        NEW_GAME,
        END_GAME,
        TIME_OUT,
        UNDO,
        QUIT

    }

}
