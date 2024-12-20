using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace E_Tickets.Models;

public partial class Category
{
    public int Id { get; set; }
    [Required]
    [MinLength(3, ErrorMessage = "Minimum length must be 3")]
    [MaxLength(50, ErrorMessage = "Maximum length must be 50")]
    public string? Name { get; set; }
    [ValidateNever]

    public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
