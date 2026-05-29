using AstroFarm.Api.Data;
using AstroFarm.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AstroFarm.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropriedadesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PropriedadesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Propriedades
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Propriedade>>> GetPropriedades()
        {
            return await _context.Propriedades.ToListAsync();
        }

        // GET: api/Propriedades/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Propriedade>> GetPropriedade(int id)
        {
            var propriedade = await _context.Propriedades.FindAsync(id);

            if (propriedade == null)
            {
                return NotFound();
            }

            return propriedade;
        }

        // POST: api/Propriedades
        [HttpPost]
        public async Task<ActionResult<Propriedade>> PostPropriedade(Propriedade propriedade)
        {
            propriedade.DtRegistro = DateTime.Now;
            _context.Propriedades.Add(propriedade);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPropriedade), new { id = propriedade.Id }, propriedade);
        }

        // PUT: api/Propriedades/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPropriedade(int id, Propriedade propriedade)
        {
            if (id != propriedade.Id)
            {
                return BadRequest();
            }

            _context.Entry(propriedade).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Propriedades/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePropriedade(int id)
        {
            var propriedade = await _context.Propriedades.FindAsync(id);
            if (propriedade == null)
            {
                return NotFound();
            }

            _context.Propriedades.Remove(propriedade);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}