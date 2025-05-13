namespace Tutorial9.Model.DTO;

public class PostBookingDTO
{
    public int BookingId { get; set; }
    public int GuestId { get; set; }
    public string EmployeeNumber { get; set; }
    public List<PostAttractionDTO> Attractions { get; set; }
}

public class PostAttractionDTO
{
    public string Name { get; set; }
    public int Amount { get; set; }
}