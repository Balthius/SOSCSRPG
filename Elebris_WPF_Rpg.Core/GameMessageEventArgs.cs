namespace Elebris_WPF_Rpg.Core
{

    public class GameMessageEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public GameMessageEventArgs(string message)
        {
            Message = message;
        }
    }
}
