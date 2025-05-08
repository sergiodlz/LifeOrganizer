using LifeOrganizer.Business.Services;
using LifeOrganizer.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LifeOrganizer.Api.Controllers
{
    [ApiController]
    [Route("api/subcategories")]
    public class SubcategoriesController : ControllerBase
    {
        private readonly IGenericService<Subcategory> _service;

        public SubcategoriesController(IGenericService<Subcategory> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subcategory>>> GetAll(CancellationToken cancellationToken)
        {
            var items = await _service.GetAllAsync(cancellationToken);
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Subcategory>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var item = await _service.GetByIdAsync(id, cancellationToken);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<Subcategory>> Create(Subcategory entity, CancellationToken cancellationToken)
        {
            await _service.AddAsync(entity, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Subcategory entity, CancellationToken cancellationToken)
        {
            if (id != entity.Id) return BadRequest("ID in URL and body do not match.");
            var existing = await _service.GetByIdAsync(id, cancellationToken);
            if (existing == null) return NotFound();
            await _service.UpdateAsync(entity, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _service.GetByIdAsync(id, cancellationToken);
            if (entity == null) return NotFound();
            // Soft delete: mark as deleted
            await _service.RemoveAsync(entity, cancellationToken);
            return NoContent();
        }
    }
}
