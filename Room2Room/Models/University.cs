using System.ComponentModel.DataAnnotations;

namespace Room2Room.Models;

public class University
{
    [Key]
    public int Id { get; set; }
    public string Domain { get; set; }
    public string Name { get; set; }
}