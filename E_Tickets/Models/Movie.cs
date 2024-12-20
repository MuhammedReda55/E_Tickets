using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace E_Tickets.Models;

public partial class Movie
{
    public int Id { get; set; }
    [Required]
    [MinLength(3, ErrorMessage = "Minimum length must be 3")]
    [MaxLength(50, ErrorMessage = "Maximum length must be 50")]
    public string? Name { get; set; }

    public string? Description { get; set; }
    [Range(0, 100000)]
    public decimal? Price { get; set; }
    [ValidateNever]
    public string? ImgUrl { get; set; }
    [ValidateNever]
    public string? TrailerUrl { get; set; }
    [Required]
    public DateTime? StartDate { get; set; }
    [Required]
    public DateTime? EndDate { get; set; }
    [ValidateNever]
    public int? MovieStatus { get; set; }

    public int Views { get; set; } = 0;

    [Required]
    public int? CinemaId { get; set; }
    [Required]
    public int? CategoryId { get; set; }
    [ValidateNever]
    public virtual Category? Category { get; set; }
    [ValidateNever]
    public virtual Cinema? Cinema { get; set; }
    [ValidateNever]
    public ICollection<ActorMovie> ActorMovies { get; set; } = new List<ActorMovie>();

    internal IIncludableQueryable<Movie, object> Include(Func<object, object> value)
    {
        throw new NotImplementedException();
    }
}
