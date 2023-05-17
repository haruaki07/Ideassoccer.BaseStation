namespace Ideassoccer.BaseStation.UI.Models
{
    public class State
    {
        public State(string value) { Value = value; Info = State.GetInfo(value); }

        public string Value { get; private set; }
        public string Info { get; private set; }

        public static State Start { get => new("s"); }
        public static State Stop { get => new("S"); }
        public static State Retry { get => new("r"); }

        public static string GetInfo(string state)
        {
            if (state == "s") return "Start";
            if (state == "S") return "Stop";
            if (state == "r") return "Retry";
            return "";
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
