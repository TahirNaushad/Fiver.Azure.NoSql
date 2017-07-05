using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fiver.Azure.NoSql.Client.OtherLayers
{
    public class MovieService : IMovieService
    {
        private readonly IAzureNoSqlRepository<Movie> repository;

        public MovieService(IAzureNoSqlRepository<Movie> repository)
        {
            this.repository = repository;
        }

        public async Task<List<Movie>> GetMovies()
        {
            return await this.repository.GetList();
        }

        public async Task<Movie> GetMovie(string id)
        {
            return await this.repository.GetItem(id);
        }

        public async Task AddMovie(Movie item)
        {
            await this.repository.Insert(item); 
        }

        public async Task UpdateMovie(Movie item)
        {
            await this.repository.Update(item.Id, item);
        }

        public async Task DeleteMovie(string id)
        {
            await this.repository.Delete(id);
        }

        public async Task<bool> MovieExists(string id)
        {
            return await this.repository.GetItem(id) != null;
        }
    }
}
