using Microsoft.EntityFrameworkCore;
using MovieApi.Data;
using MovieApi.Models;
using MovieApi.Repository.IRepository;

namespace MovieApi.Repository;

public class MovieRepository : IMovieRepository
{
    private readonly ApplicationDbContext _dbContext;

    public MovieRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ICollection<Movie> GetMovies(
        int pageNumber,
        int pageSize
    )
    {
        return _dbContext.Movies.OrderBy(m => m.Title)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int GetMoviesCount()
    {
        return _dbContext.Movies.Count();
    }

    public ICollection<Movie> GetMoviesByCategory(int categoryId)
    {
        return _dbContext.Movies.Include(ca => ca.Category).Where(ca => ca.CategoryId == categoryId).ToList();
    }

    public IEnumerable<Movie> SearchMovies(string title)
    {
        IQueryable<Movie> query = _dbContext.Movies;
        if (!string.IsNullOrEmpty(title))
        {
            query = query.Where(m =>
                m.Title.Contains(title) || m.Description.Contains(title));
        }

        return query.ToList();
    }

    public Movie GetMovie(int idMovie)
    {
        return _dbContext.Movies.FirstOrDefault(m => m.Id == idMovie);
    }

    public bool MovieExists(int idMovie)
    {
        return _dbContext.Movies.Any(m => m.Id == idMovie);
    }

    public bool MovieExists(string nameMovie)
    {
        var movieExists = _dbContext.Movies.Any(c => c.Title.ToLower().Trim() == nameMovie.ToLower().Trim());
        return movieExists;
    }

    public bool MovieCreate(Movie movie)
    {
        movie.CreatedDate = DateTime.Now;
        _dbContext.Movies.Add(movie);
        return MovieSaveChanges();
    }

    public bool MovieUpdate(Movie movie)
    {
        movie.CreatedDate = DateTime.Now;
        _dbContext.Movies.Update(movie);

        return MovieSaveChanges();
    }

    public bool MovieDelete(Movie movie)
    {
        _dbContext.Movies.Remove(movie);
        return MovieSaveChanges();
    }

    public bool MovieSaveChanges()
    {
        return _dbContext.SaveChanges() >= 0;
    }
}