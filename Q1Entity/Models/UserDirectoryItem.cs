namespace Q1Entity
{
    public class UserDirectoryItem
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int DirectoryItemId { get; set; }
        public DirectoryItem DirectoryItem { get; set; }
    }
}
