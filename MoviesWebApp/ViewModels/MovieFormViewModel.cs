﻿using MoviesWebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace MoviesWebApp.ViewModels
{
    public class MovieFormViewModel
    {
        [Required, StringLength(250)]
        public string Title { get; set; }
        public int Year { get; set; }
        [Range(1, 10)]
        public float Rate { get; set; }
        [Required, StringLength(2500)]
        public string Storyline { get; set; }
        [Display(Name = "Select Poster")]
        public byte[] Poster { get; set; }
        [Display(Name = "Genre")]
        public byte GenreId { get; set; }
        public IEnumerable<Genre> Genres{ get; set; }

    }
}