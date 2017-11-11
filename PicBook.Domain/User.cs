namespace PicBook.Domain
{
    public class User : Entity
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string UserIdentifier { get; set; }
    }
}