namespace CODE_CDIO4.DTOs
{
    // ==================== DTOs ====================
    public class CapDoDTO
    {
        public int Id { get; set; }
        public string Ten { get; set; } = string.Empty;
        public int DiemTu { get; set; }
        public int DiemDen { get; set; }
    }

    public class CapDoInsertDTO
    {
        public string Ten { get; set; } = string.Empty;
        public int DiemTu { get; set; }
        public int DiemDen { get; set; }
    }

    public class CapDoUpdateDTO
    {
        public string Ten { get; set; } = string.Empty;
        public int DiemTu { get; set; }
        public int DiemDen { get; set; }
    }
}
