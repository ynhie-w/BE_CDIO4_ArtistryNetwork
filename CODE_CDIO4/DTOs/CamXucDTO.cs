using System.ComponentModel.DataAnnotations;

namespace CODE_CDIO4.DTOs
{
 
    public class CamXucDTO
    {
        public int Id { get; set; }
        public string Ten { get; set; } = string.Empty;
    }

    public class CamXucInsertDTO
    {
        [Required]
        public string Ten { get; set; } = string.Empty;
    }

    public class CamXucUpdateDTO
    {
        [Required]
        public string Ten { get; set; } = string.Empty;
    }
}
