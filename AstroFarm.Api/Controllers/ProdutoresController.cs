using AstroFarm.Api.Data;
using AstroFarm.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AstroFarm.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProdutoresController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Produtores/Cadastro
        [HttpPost("Cadastro")]
        public async Task<ActionResult<Produtor>> Cadastrar(Produtor produtor)
        {
            if (await _context.Produtores.AnyAsync(p => p.Cpf == produtor.Cpf))
            {
                return BadRequest("CPF já cadastrado.");
            }

            produtor.DtCadastro = DateTime.Now;
            _context.Produtores.Add(produtor);
            await _context.SaveChangesAsync();

            // Retorna 201 Created
            return CreatedAtAction(nameof(GetProdutor), new { id = produtor.Id }, produtor);
        }

        // POST: api/Produtores/Login
        [HttpPost("Login")]
        public async Task<ActionResult<Produtor>> Login([FromBody] string cpf)
        {
            var produtor = await _context.Produtores.FirstOrDefaultAsync(p => p.Cpf == cpf);

            if (produtor == null)
            {
                return Unauthorized("Produtor não encontrado. Verifique o CPF.");
            }

            return Ok(produtor);
        }

        // GET: api/Produtores/5 (Usado internamente pelo Cadastro para retornar os dados)
        [HttpGet("{id}")]
        public async Task<ActionResult<Produtor>> GetProdutor(int id)
        {
            var produtor = await _context.Produtores.FindAsync(id);

            if (produtor == null)
            {
                return NotFound();
            }

            return produtor;
        }
    }
}