using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AzureBasicNet6.Models
{
    public class BlobFile
    {
        public BlobFile()
        {
            CreatedDate = DateTime.Now;
        }

        public string? Name { get; set; }
        public string? FileUrl { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Please Enter Description")]
        public IFormFile File { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
