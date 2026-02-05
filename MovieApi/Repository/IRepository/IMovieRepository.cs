using MovieApi.Models;

namespace MovieApi.Repository.IRepository;

public interface IMovieRepository
{
    ICollection<Movie> GetMovies();
    ICollection<Movie> GetMoviesByCategory(int categoryId);
    IEnumerable<Movie> SearchMovies(string title);

    Movie GetMovie(int idMovie);

    bool MovieExists(int idMovie);

    bool MovieExists(string titleMovie);

    bool MovieCreate(Movie movie);

    bool MovieUpdate(Movie movie);

    bool MovieDelete(Movie movie);

    bool MovieSaveChanges();
}