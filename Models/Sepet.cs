using System.ComponentModel.DataAnnotations;

namespace EticaretProje.Models
{
    public class Sepet
    {
        [Key] 
        public int RecordId { get; set; }
        public string CartId { get; set; }
        public int AlbumId { get; set; }
        public int Count { get; set; }
        public System.DateTime DateCreated { get; set; }
        public virtual Album Album { get; set; }

    }
}
