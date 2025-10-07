namespace DMVConnect.ViewModels.Connections
{
    public class UserWithConnectionsCountVM
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public int ConnectionCount { get; set; }

        public string ConnectionCountDisplay =>
            ConnectionCount == 0 ? "No Connections" :
            ConnectionCount == 1 ? "1 Connection" :
            $"{ConnectionCount} Connections";
    }
}
