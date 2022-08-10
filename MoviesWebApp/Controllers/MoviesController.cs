using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesWebApp.Models;
using MoviesWebApp.ViewModels;

namespace MoviesWebApp.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.ToListAsync();
            return View(movies);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new ViewModels.MovieFormViewModel()
            {
                Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync()
            };
            return View("MovieForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieFormViewModel model)
        {
            var files = Request.Form.Files;
            if (!files.Any())
            {
                model.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Please, select movie poster!");
                return View(model);
            }

            var poster = files.FirstOrDefault();
            var allowedExtenstions = new List<string> { ".jpg", ".png" };

            if (!allowedExtenstions.Contains(Path.GetExtension(poster.FileName).ToLower()))
            {
                model.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                ModelState.AddModelError("Poster", "only .PNG, .JPG Images are allowed!");
                return View("MovieForm", model);
            }

            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                return View(model);
            }

            if(poster.Length > 1048576)
            {
                model.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Poster can be more than one 1 MB!");
                return View(model);

            }

            //convert image into byte array
            using var dataStream = new MemoryStream();
            await poster.CopyToAsync(dataStream);

            var movie = new Movie
            {
                Title = model.Title,
                GenreId = model.GenreId,
                Year = model.Year,
                Rate = model.Rate,
                Storyline = model.Storyline,
                Poster = dataStream.ToArray()
            };

            _context.Movies.Add(movie);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return NotFound();

            var viewModel = new ViewModels.MovieFormViewModel()
            {
                Id = movie.MovieId,
                Title = movie.Title,
                GenreId=movie.GenreId,
                Year = movie.Year,
                Rate = movie.Rate,
                Storyline= movie.Storyline,
                Poster= movie.Poster,
                Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync()
            };

            return View("MovieForm", viewModel);
        }
    }
}
