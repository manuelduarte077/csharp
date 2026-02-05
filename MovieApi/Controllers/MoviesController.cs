using System.Collections;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MovieApi.Models;
using MovieApi.Models.Dtos;
using MovieApi.Repository.IRepository;

namespace MovieApi.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieRepository _movieRepo;
        private readonly IMapper _mapper;

        public MoviesController(IMovieRepository movieRepo, IMapper mapper)
        {
            _movieRepo = movieRepo;
            _mapper = mapper;
        }

        [HttpGet]
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


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(MovieDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateMovie([FromBody] CreateMovieDto createMovieDto)
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
            if (!_movieRepo.MovieCreate(movie))
            {
                ModelState.AddModelError("", $"Movie create failed: {movie.Title}");
                return StatusCode(404, ModelState);
            }

            return CreatedAtRoute("GetMovie", new { id = movie.Id }, movie);
        }

        [HttpPatch("{id:int}", Name = "UpdateMovie")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateMovie(int id, [FromBody] MovieDto movieDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (movieDto == null || id != movieDto.Id)
            {
                return BadRequest(ModelState);
            }

            var movieExists = _movieRepo.GetMovie(id);
            if (movieExists == null)
            {
                return NotFound($"No movie found with id");
            }

            var movie = _mapper.Map<Movie>(movieDto);
            if (!_movieRepo.MovieUpdate(movie))
            {
                ModelState.AddModelError("", $"Movie update failed: {movie.Title}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteMovie")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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