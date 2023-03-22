namespace Proxifier.Models
{
    public class Process
    {
        public Process(int pid, ServerInfo serverInfo)
        {
            Pid = pid;
            ServerInfo = serverInfo;
        }

        public int Pid { get; }
        public ServerInfo ServerInfo { get; }
    }
}