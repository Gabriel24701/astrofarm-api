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
        public async Task<ActionResult<Produtor>> Cadastrar([FromBody] Produtor produtor)
        {
            if (await _context.Produtores.AnyAsync(p => p.Cpf == produtor.Cpf))
            {
                return BadRequest("CPF já cadastrado.");
            }

            produtor.DtCadastro = DateTime.Now;
            _context.Produtores.Add(produtor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProdutor), new { id = produtor.Id }, produtor);
        }

        // POST: api/Produtores/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest dadosLogin)
        {
            var produtor = await _context.Produtores
                .FirstOrDefaultAsync(p => p.Cpf == dadosLogin.Cpf && p.Senha == dadosLogin.Senha);

            if (produtor == null)
            {
                return Unauthorized(new { mensagem = "CPF ou senha inválidos." });
            }

            return Ok(produtor);
        }

        // GET: api/Produtores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Produtor>> GetProdutor(int id)
        {
            var produtor = await _context.Produtores.FindAsync(id);

            if (produtor == null)
            {
                return NotFound();
            }

            return Ok(produtor);
        }
    }

    public class LoginRequest
    {
        public string Cpf { get; set; }
        public string Senha { get; set; }
    }
}