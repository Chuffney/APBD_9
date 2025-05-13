namespace Tutorial9.Model.DTO;

public class BookingDTO
{
    public DateTime date { get; set; }
    public GuestDTO guest { get; set; }
    public EmployeeDTO employee { get; set; }
    public List<AttractionDTO> attractions { get; set; }
}