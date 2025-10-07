using DMVConnect.Data.Models;

namespace DMVConnect.ViewModels.Users
{
    public class GetUserProfileVM
    {
        public User User { get; set; }
        public List<Post> Posts { get; set; }

        public List<Connection> Connections = new List<Connection>();
        public string ConnectionCountDisplay =>
            Connections.Count == 0 ? "No Connections" :
            Connections.Count == 1 ? "1 Connection" :
            $"{Connections.Count} Connections";
    }
}
