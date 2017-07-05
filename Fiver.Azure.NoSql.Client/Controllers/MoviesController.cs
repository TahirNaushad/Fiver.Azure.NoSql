using Fiver.Azure.NoSql.Client.Lib;
using Fiver.Azure.NoSql.Client.Models.Movies;
using Fiver.Azure.NoSql.Client.OtherLayers;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fiver.Azure.NoSql.Client.Controllers
{
    [Route("movies")]
    public class MoviesController : BaseController
    {
        private readonly IMovieService service;

        public MoviesController(IMovieService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var model = await service.GetMovies();

            var outputModel = ToOutputModel(model);
            return Ok(outputModel);
        }

        [HttpGet("{id}", Name = "GetMovie")]
        public async Task<IActionResult> Get(string id)
        {
            var model = await service.GetMovie(id);
            if (model == null)
                return NotFound();

            var outputModel = ToOutputModel(model);
            return Ok(outputModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]MovieInputModel inputModel)
        {
            if (inputModel == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return Unprocessable(ModelState);

            var model = ToDomainModel(inputModel);
            await service.AddMovie(model);

            var outputModel = ToOutputModel(model);
            return CreatedAtRoute("GetMovie", new { id = outputModel.Id }, outputModel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody]MovieInputModel inputModel)
        {
            if (inputModel == null || id != inputModel.Id)
                return BadRequest();

            if (!await service.MovieExists(id))
                return NotFound();

            if (!ModelState.IsValid)
                return new UnprocessableObjectResult(ModelState);

            var model = ToDomainModel(inputModel);
            await service.UpdateMovie(model);

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePatch(
            string id, [FromBody]JsonPatchDocument<MovieInputModel> patch)
        {
            if (patch == null)
                return BadRequest();

            var model = await service.GetMovie(id);
            if (model == null)
                return NotFound();

            var inputModel = ToInputModel(model);
            patch.ApplyTo(inputModel);

            TryValidateModel(inputModel);
            if (!ModelState.IsValid)
                return new UnprocessableObjectResult(ModelState);

            model = ToDomainModel(inputModel);
            await service.UpdateMovie(model);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!await service.MovieExists(id))
                return NotFound();

            await service.DeleteMovie(id);

            return NoContent();
        }

        #region " Mappings "

        private MovieOutputModel ToOutputModel(Movie model)
        {
            return new MovieOutputModel
            {
                Id = model.Id,
                Title = model.Title,
                ReleaseYear = model.ReleaseYear,
                Summary = model.Summary,
                LastReadAt = DateTime.Now
            };
        }

        private List<MovieOutputModel> ToOutputModel(List<Movie> model)
        {
            return model.Select(item => ToOutputModel(item))
                        .ToList();
        }

        private Movie ToDomainModel(MovieInputModel inputModel)
        {
            return new Movie
            {
                Id = inputModel.Id,
                Title = inputModel.Title,
                ReleaseYear = inputModel.ReleaseYear,
                Summary = inputModel.Summary
            };
        }

        private MovieInputModel ToInputModel(Movie model)
        {
            return new MovieInputModel
            {
                Id = model.Id,
                Title = model.Title,
                ReleaseYear = model.ReleaseYear,
                Summary = model.Summary
            };
        }
        
        #endregion
    }
}
