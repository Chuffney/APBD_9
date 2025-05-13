using System.ComponentModel.DataAnnotations;

namespace Tutorial9.Model.DTO;

public class EmployeeDTO
{
    [MaxLength(100)]
    public string FirstName { get; set; }
    [MaxLength(100)]
    public string LastName { get; set; }
    
    [MaxLength(22)]
    public string EmployeeNumber { get; set; }
}