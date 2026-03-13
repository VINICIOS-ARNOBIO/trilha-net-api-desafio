using Microsoft.AspNetCore.Mvc;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly OrganizadorContext _context;

        public TarefaController(OrganizadorContext context)
        {
            _context = context;
        }

        // LEITURA (Read): Busca um registro específico pelo ID
        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            // O Entity Framework busca no banco usando o método Find
            Tarefa tarefa = _context.Tarefas.Find(id);

            if (tarefa == null)
                return NotFound(); // Retorna 404 caso não exista

            return Ok(tarefa); // Retorna 200 OK com os dados da tarefa
        }

        // LEITURA (Read): Busca todos os registros do banco
        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {
            // Select(x => x) retorna todos os objetos da tabela
            List<Tarefa> tarefas = _context.Tarefas.Select(x => x).ToList();

            if(tarefas == null)
                return NotFound();

            return Ok(tarefas);
        }

        // LEITURA (Read): Busca registros que contenham parte do título informado
        [HttpGet("ObterPorTitulo")]
        public IActionResult ObterPorTitulo(string titulo)
        {
            // LINQ traduzindo a busca para um "LIKE" do SQL
            List<Tarefa> tarefas = _context.Tarefas.Where(x => x.Titulo.Contains(titulo)).ToList();

            if (tarefas == null)
                return NotFound();

            return Ok(tarefas);
        }

        // LEITURA (Read): Busca registros por uma data específica
        [HttpGet("ObterPorData")]
        public IActionResult ObterPorData(DateTime data)
        {
            var tarefa = _context.Tarefas.Where(x => x.Data.Date == data.Date);

            if (tarefa == null)
                return NotFound();

            return Ok(tarefa);
        }

        // LEITURA (Read): Busca registros baseados no Status (Enum)
        [HttpGet("ObterPorStatus")]
        public IActionResult ObterPorStatus(EnumStatusTarefa status)
        {
            var tarefa = _context.Tarefas.Where(x => x.Status == status);

            if (tarefa == null)
                return NotFound();

            return Ok(tarefa);
        }

        // CRIAÇÃO (Create): Insere um novo registro no banco de dados
        [HttpPost]
        public IActionResult Criar(Tarefa tarefa)
        {
            // Validação simples de regra de negócio
            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            // Entity Framework adiciona o objeto e SaveChanges persiste no SQL
            _context.Add(tarefa);
            _context.SaveChanges();

            // Retorna o status 201 Created e o local onde o recurso pode ser consultado
            return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
        }

        // ATUALIZAÇÃO (Update): Substitui os dados de um registro existente
        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, Tarefa tarefa)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound();

            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            // Mapeia os dados recebidos para o objeto que veio do banco
            tarefaBanco.Descricao = tarefa.Descricao;
            tarefaBanco.Titulo = tarefa.Titulo;
            tarefaBanco.Status = tarefa.Status;
            tarefaBanco.Data = tarefa.Data;

            // O EF rastreia as mudanças e o Update prepara a atualização
            _context.Update(tarefaBanco);
            _context.SaveChanges();

            return Ok(tarefaBanco);
        }

        // REMOÇÃO (Delete): Apaga um registro do banco de dados
        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound();

            // O EF remove o registro da tabela correspondente
            _context.Remove(tarefaBanco);
            _context.SaveChanges();

            // Retorna 204 No Content, pois o recurso não existe mais
            return NoContent();
        }
    }
}
