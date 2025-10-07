using DMVConnect.Data.Models;

namespace DMVConnect.ViewModels.Connections
{
    public class ConnectionVM
    {
        public List<Connection> Connections = new List<Connection>();

        public List<ConnectionRequest> ConnectionRequestSent = new List<ConnectionRequest>();
        public List<ConnectionRequest> ConnectionRequestReceived = new List<ConnectionRequest>();
    }
}
