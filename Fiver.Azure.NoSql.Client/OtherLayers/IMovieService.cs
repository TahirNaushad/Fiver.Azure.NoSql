using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fiver.Azure.NoSql.Client.OtherLayers
{
    public interface IMovieService
    {
        Task<List<Movie>> GetMovies();
        Task<Movie> GetMovie(string id);
        Task AddMovie(Movie item);
        Task UpdateMovie(Movie item);
        Task DeleteMovie(string id);
        Task<bool> MovieExists(string id);
    }
}
