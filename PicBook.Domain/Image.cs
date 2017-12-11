namespace PicBook.Domain
{
    public class Image : Entity
    {
        public string ImageIdentifier { get; set; }
        public string ImageURL { get; set; }
        public string Name { get; set; }
        public string UserIdentifier { get; set; }
        public bool Remote { get; set; }
        public bool IsArchived { get; set; }
        public bool PublicToAll { get; set; }
    }
}