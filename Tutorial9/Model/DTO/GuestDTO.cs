using System.ComponentModel.DataAnnotations;

namespace Tutorial9.Model.DTO;

public class GuestDTO
{    
    [MaxLength(100)]
    public string FirstName { get; set; }
    [MaxLength(100)]
    public string LastName { get; set; }

    public DateTime DateOfBirth { get; set; }
}