using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace E_Tickets.Models;

public partial class Actor
{
    public int Id { get; set; }
    [Required]
    [MinLength(3, ErrorMessage = "Minimum length must be 3")]
    [MaxLength(50, ErrorMessage = "Maximum length must be 20")]
    public string? FirstName { get; set; }
    [Required]
    [MinLength(3, ErrorMessage = "Minimum length must be 3")]
    [MaxLength(50, ErrorMessage = "Maximum length must be 20")]
    public string? LastName { get; set; }
    [Required]
    public string? Bio { get; set; }
    [ValidateNever]
    public string? ProfilePicture { get; set; }
    [ValidateNever]
    public string? News { get; set; }
    [ValidateNever]
    public ICollection<ActorMovie> ActorMovies { get; } = new List<ActorMovie>();
}
