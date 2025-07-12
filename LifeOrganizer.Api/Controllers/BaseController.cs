using LifeOrganizer.Api.Extensions;
using LifeOrganizer.Business.DTOs;
using LifeOrganizer.Business.Services;
using LifeOrganizer.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LifeOrganizer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<TEntity, TDto> : ControllerBase
        where TEntity : BaseEntity
        where TDto : BaseEntityDto
    {
        private readonly IGenericService<TEntity, TDto> _genericService;

        public BaseController(IGenericService<TEntity, TDto> genericService)
        {
            _genericService = genericService;
        }

        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<TDto>>> GetAll(CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var entities = await _genericService.GetAllAsync(userId, cancellationToken);
            return Ok(entities);
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var entity = await _genericService.GetByIdAsync(id, userId, cancellationToken);
            if (entity == null)
                return NotFound();

            return Ok(entity);
        }

        [HttpPost]
        public virtual async Task<ActionResult<TDto>> Create(TDto dto, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            dto.UserId = userId;
            dto.CreatedBy = userId.ToString();
            dto.CreatedOn = DateTime.UtcNow;
            await _genericService.AddAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(Guid id, TDto dto, CancellationToken cancellationToken)
        {
            if (id != dto.Id)
                return BadRequest("ID in URL and body do not match.");
            var userId = User.GetUserId();
            var existing = await _genericService.GetByIdAsync(id, userId, cancellationToken);
            if (existing == null)
                return NotFound();

            dto.UserId = userId;
            dto.CreatedBy = existing.CreatedBy;
            dto.CreatedOn = existing.CreatedOn;
            dto.UpdatedBy = User.GetUserId().ToString();
            dto.UpdatedOn = DateTime.UtcNow;
            await _genericService.UpdateAsync(dto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var dto = await _genericService.GetByIdAsync(id, userId, cancellationToken);
            if (dto == null)
                return NotFound();
                
            await _genericService.RemoveAsync(dto, cancellationToken);
            return NoContent();
        }
    }
}
