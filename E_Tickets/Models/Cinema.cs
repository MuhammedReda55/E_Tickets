using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace E_Tickets.Models;

public partial class Cinema
{
    public int Id { get; set; }
    [Required]
    [MinLength(3, ErrorMessage = "Minimum length must be 3")]
    [MaxLength(50, ErrorMessage = "Maximum length must be 50")]
    public string? Name { get; set; }
    [ValidateNever]
    public string? Description { get; set; }
    [ValidateNever]
    public string? CinemaLogo { get; set; }
    [RegularExpression("Cairo|Alex|Mansoura")]
    public string? Address { get; set; }
    [ValidateNever]
    public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
