namespace Autenticacion.DataService.Models
{
    public class Post : BaseEntity
    {
        public string Body { get; set; }
        public List<Comment> Comments { get; set; }
        public Post()
        {
            Comments = new List<Comment>();
        }
    }
}
