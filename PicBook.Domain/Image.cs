namespace PicBook.Domain
{
    public class Image : Entity
    {
        public string ImageIdentifier { get; set; }
        public string Name { get; set; }
        public string UserIdentifier { get; set; }
    }
}