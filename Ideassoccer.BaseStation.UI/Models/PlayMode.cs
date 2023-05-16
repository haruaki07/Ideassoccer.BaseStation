namespace Ideassoccer.BaseStation.UI.Models
{
    public class PlayMode
    {
        public PlayMode(string value) { Value = value; Info = PlayMode.GetInfo(value); }

        public string Value { get; private set; }
        public string Info { get; private set; }

        public static PlayMode RKickOff { get => new("k"); }
        public static PlayMode LKickOff { get => new("K"); }
        public static PlayMode RCorner { get => new("c"); }
        public static PlayMode LCorner { get => new("C"); }
        public static PlayMode DropBall { get => new("N"); }

        public static string GetInfo(string state)
        {
            if (state == "k") return "Right Kick-off";
            if (state == "K") return "Left Kick-off";
            if (state == "c") return "Right Corner Kick";
            if (state == "C") return "Left Corner Kick";
            if (state == "N") return "Drop Ball";
            return "";
        }

        public override string ToString()
        {
            return Value;
        }
    }

}
