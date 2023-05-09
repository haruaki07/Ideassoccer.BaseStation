using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ideassoccer.BaseStation.UI.Models
{
    public class RobotMessage
    {
        public string Command { get; set; }
        public int Kes { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public RobotMessage(string command, int kes, float x, float y,float z) {
            Command = command;
            Kes = kes;
            X = x;
            Y = y;
            Z = z;
        }
    }
}
