using System.Net;
using System.Threading.Tasks;
using BannerWebApp.Models;
using BannerWebApp.Validation;
using Declarations.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BannerWebApp.Controllers
{
    [Route("api/[controller]")]
    [ValidateModel]
    public class BannersController : Controller
    {
        #region private members

        private readonly IBannersRepository _repository;

        #endregion

        #region API

        public BannersController(IBannersRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int take, int skip)
        {
            if (take <= 0 || skip < 0)
                return BadRequest();
            var existing = await _repository.List(take, skip);
            return Ok(existing);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var existing = await _repository.FindById(id);
            
            if (existing == null)
                return NotFound();
            return Ok(existing);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BannerModel banner)
        {
            var existing = await _repository.FindById(banner.Id);
            if (existing != null)
                return StatusCode((int)HttpStatusCode.Conflict);
            await _repository.Add(banner);
            return Ok(banner);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]BannerModel banner)
        {
            var existing = await _repository.Update(id, banner);

            if (existing == null)
                return NotFound();

            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repository.FindById(id);
            if (existing == null)
                return NotFound();
            await _repository.Delete(id);
            return Ok();
        }

        [HttpGet("render/{id}")]
        public async Task<IActionResult> Render(int id)
        {
            var existing = await _repository.FindById(id);
            if (existing == null)
                return NotFound();

            return new ContentResult
            {
                Content = existing.Html,
                ContentType = "text/html",
            };
        }

        #endregion
    }
}
