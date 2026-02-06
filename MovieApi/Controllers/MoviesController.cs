using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApi.Models;
using MovieApi.Models.Dtos;
using MovieApi.Repository.IRepository;

namespace MovieApi.Controllers
{
    [Route("api/v{version:apiVersion}/movies")]
    [ApiController]
    [ApiVersion("2.0")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieRepository _movieRepo;
        private readonly IMapper _mapper;

        public MoviesController(IMovieRepository movieRepo, IMapper mapper)
        {
            _movieRepo = movieRepo;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        [ResponseCache(Duration = 20)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetMovies()
        {
            var moviesList = _movieRepo.GetMovies();
            var movieDto = new List<MovieDto>();

            foreach (var list in moviesList)
            {
                movieDto.Add(_mapper.Map<MovieDto>(list));
            }

            return Ok(movieDto);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "GetMovie")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetMovie(int id)
        {
            var movieItem = _movieRepo.GetMovie(id);
            if (movieItem == null)
            {
                return NotFound();
            }

            var itemMovieDto = _mapper.Map<MovieDto>(movieItem);
            return Ok(itemMovieDto);
        }

        [AllowAnonymous]
        [HttpGet("GetMoviesByCategory/{categoryId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetMoviesByCategory(int categoryId)
        {
            var moviesList = _movieRepo.GetMoviesByCategory(categoryId);
            if (moviesList == null)
            {
                return NotFound();
            }

            var movieItemDto = new List<MovieDto>();
            foreach (var movie in moviesList)
            {
                movieItemDto.Add(_mapper.Map<MovieDto>(movie));
            }

            return Ok(movieItemDto);
        }

        [AllowAnonymous]
        [HttpGet("SearchMovies")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SearchMovies(string query)
        {
            try
            {
                var result = _movieRepo.SearchMovies(query);
                if (result.Any())
                {
                    return Ok(result);
                }

                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Consumes("multipart/form-data", "application/x-www-form-urlencoded")]
        [ProducesResponseType(201, Type = typeof(MovieDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateMovie([FromForm] CreateMovieDto createMovieDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (createMovieDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_movieRepo.MovieExists(createMovieDto.Title))
            {
                ModelState.AddModelError("", "Movie already exists");
                return StatusCode(404, ModelState);
            }

            var movie = _mapper.Map<Movie>(createMovieDto);

            // Upload File → wwwroot/images
            if (createMovieDto.Image != null)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(createMovieDto.Image.FileName);
                string imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                Directory.CreateDirectory(imagesFolder);
                string locationPath = Path.Combine(imagesFolder, fileName);

                using (var fileStream = new FileStream(locationPath, FileMode.Create))
                {
                    createMovieDto.Image.CopyTo(fileStream);
                }

                var baseUrl =
                    $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value?.TrimEnd('/')}";
                movie.ImageUrl = baseUrl + "/images/" + fileName;
                movie.LocationImageUrl = Path.Combine("wwwroot", "images", fileName);
            }
            else
            {
                movie.ImageUrl = "https://placehold.co/600x400";
            }

            _movieRepo.MovieCreate(movie);
            return CreatedAtRoute("GetMovie", new { id = movie.Id }, movie);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}", Name = "UpdateMovie")]
        [Consumes("multipart/form-data", "application/x-www-form-urlencoded")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateMovie(int id, [FromForm] UpdateMovieDto updateMovieDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (updateMovieDto == null || id != updateMovieDto.Id)
            {
                return BadRequest(ModelState);
            }

            var movie = _movieRepo.GetMovie(id);
            if (movie == null)
            {
                return NotFound("No movie found with id");
            }

            // Actualizar propiedades desde el DTO
            _mapper.Map(updateMovieDto, movie);

            // Upload File → wwwroot/images
            if (updateMovieDto.Image != null)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(updateMovieDto.Image.FileName);
                string imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                Directory.CreateDirectory(imagesFolder);
                string locationPath = Path.Combine(imagesFolder, fileName);

                using (var fileStream = new FileStream(locationPath, FileMode.Create))
                {
                    updateMovieDto.Image.CopyTo(fileStream);
                }

                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value?.TrimEnd('/')}";
                movie.ImageUrl = baseUrl + "/images/" + fileName;
                movie.LocationImageUrl = Path.Combine("wwwroot", "images", fileName);
            }

            if (!_movieRepo.MovieUpdate(movie))
            {
                ModelState.AddModelError("", $"Movie update failed: {movie.Title}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}", Name = "DeleteMovie")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult DeleteMovie(int id)
        {
            if (!_movieRepo.MovieExists(id))
            {
                return NotFound();
            }

            var movie = _movieRepo.GetMovie(id);
            if (!_movieRepo.MovieDelete(movie))
            {
                ModelState.AddModelError("", $"Movie delete failed: {movie.Title}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}