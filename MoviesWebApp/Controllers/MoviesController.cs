using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesWebApp.Models;
using MoviesWebApp.ViewModels;
using NToastNotify;

namespace MoviesWebApp.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;
        public MoviesController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.OrderByDescending(x => x.Rate).ToListAsync();
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

            _toastNotification.AddSuccessToastMessage("Movie Created Successfully");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MovieFormViewModel model)
        {
            if (model.Id == null)
                return BadRequest();

            var movie = await _context.Movies.FindAsync(model.Id);

            if (movie == null)
                return NotFound();


            var files = Request.Form.Files;
            if (files.Any())
            {
                var poster = files.FirstOrDefault();
                var allowedExtenstions = new List<string> { ".jpg", ".png" };
                using var dataStream = new MemoryStream();
                await poster.CopyToAsync(dataStream);

                model.Poster = dataStream.ToArray();

                if (!allowedExtenstions.Contains(Path.GetExtension(poster.FileName).ToLower()))
                {
                    model.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "only .PNG, .JPG Images are allowed!");
                    return View("MovieForm", model);
                }

                if (!ModelState.IsValid)
                {
                    model.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                    return View("MovieForm", model);
                }

                if (poster.Length > 1048576)
                {
                    model.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Poster size can not be more than one 1 MB!");
                    return View("MovieForm", model);
                }

                movie.Poster = model.Poster;
            }

            movie.Title = model.Title;
            movie.GenreId = model.GenreId;
            movie.Year = model.Year;
            movie.Rate = model.Rate;
            movie.Storyline = model.Storyline;

            _context.SaveChanges();

            _toastNotification.AddSuccessToastMessage("Movie Updated Successfully");
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var movie = await _context.Movies.Include(m => m.Genre).SingleOrDefaultAsync(m => m.MovieId == id);

            if(movie == null)
                return NotFound();

            return View(movie);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return NotFound();

            _context.Movies.Remove(movie);
            _context.SaveChanges();

            return Ok();
        }
    }
}
