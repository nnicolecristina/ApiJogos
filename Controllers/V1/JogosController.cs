using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using apicatalogojogos.InputModel;
using apicatalogojogos.Services;
using apicatalogojogos.ViewModel;
using ApiCatalogoJogos.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogoJogos.Controllers.V1
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class JogosController : ControllerBase
    {
        private readonly IJogoService _jogoService;

        public JogosController(IJogoService jogoService)
        {
            _jogoService = jogoService;
        }

        /// <summary>
        /// Buscar todos os jogos de forma paginada
        /// </summary>
        /// <remarks>
        /// Não é possivel retornar os jogos sem paginação
        /// </remarks>
        /// <param name="pagina">Indica qual pagina está sendo consultada, mínimo 1</param>
        /// <param name="quantidade">Indica a quantidade de registros por pagina, mínimo 1 e máximo 50</param>
        /// <response code="200">Retorna a lista de jogos</response>
        /// <response code="204">Caso não houver jogos cadastrados</response>
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JogoViewModel>>> Obter([FromQuery, Range(1, int.MaxValue)] int pagina = 1, [FromQuery, Range(1, 50)] int quantidade =5)
        {
            var jogos = await _jogoService.Obter(pagina, quantidade);

            if (jogos.Count() == 0)
                return NoContent();

            return Ok(jogos);
        }

        /// <summary>
        /// Buscar um jogo por seu Id
        /// </summary>
        /// <param name="idJogo">Id do jogo a ser buscado</param>
        /// <response code="200">Retorna o jogo filtrado</response>
        /// <response code="204">Caso não houver jogos cadastrados</response>
        [HttpGet("{idJogo:guid}")]
        public async Task<ActionResult<JogoViewModel>> Obter([FromRoute] Guid idJogo)
        {
            var jogo = await _jogoService.Obter(idJogo);

            if (jogo == null)
                return NoContent();

            return Ok(jogo);
        }

        /// <summary>
        /// Cadastra um novo jogo
        /// </summary>
        /// <param name="jogoInputModel">Dados necessarios do jogo a ser cadastrado</param>
        /// <response code="200">Jogo cadastrado</response>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<JogoViewModel>> InserirJogo([FromBody]JogoInputModel jogoInputModel)
        {
            try
            {
                var jogo = await _jogoService.Inserir(jogoInputModel);
                return Ok();
            }
            catch(JogoJaCadastradoException ex)
            {
                return UnprocessableEntity("Já existe um jogo com este nome para esta produtora");
            }
        }

        /// <summary>
        /// Faz a alteração do jogo encontrado pelo Id
        /// </summary>
        /// <param name="idJogo">Id do jogo a ser alterado</param>
        /// <param name="jogoInputModel">Busca o modelo de dados a serem alterados</param>
        /// <response code="200">Atualiza o cadastro do jogo</response>
        /// <response code="204">Caso não tiver jogo cadastrado com este Id</response>
        [HttpPut("{idJogo:guid}")]
        public async Task<ActionResult> AtualizarJogo([FromRoute]Guid idJogo, [FromBody] JogoInputModel jogoInputModel)
        {
            try
            {
                await _jogoService.Atualizar(idJogo, jogoInputModel);
                return Ok();
            }
            catch (JogoNaoCadastradoException ex)
            {
                return NotFound("Não existe este jogo");
            }
        }

        /// <summary>
        /// Faz a alteração apenas no preço do jogo
        /// </summary>
        /// <param name="idJogo">Id do jogo a ser alterado</param>
        /// <param name="preco">Preço do jogo</param>
        /// <response code="200">Preço é alterado com sucesso</response>
        [HttpPatch("{idJogo:guid}/preco/{preco:double}")]
        public async Task<ActionResult> AtualizarJogo([FromRoute] Guid idJogo, [FromRoute] double preco)
        {
            try
            {
                await _jogoService.Atualizar(idJogo, preco);
                return Ok();
            }
            catch (JogoNaoCadastradoException ex)
            {
                return NotFound("Não existe este jogo");
            }
        }

        /// <summary>
        /// Deleta o registro do jogo
        /// </summary>
        /// <param name="idJogo">Id do jogo a ser deletado</param>
        /// <response code="200">Jogo deletado com sucesso</response>
        [HttpDelete("{idJogo:guid}")]
        public async Task<ActionResult> ApagarJogo([FromRoute] Guid idJogo)
        {
            try
            {
                await _jogoService.Remover(idJogo);

                return Ok();
            }
            catch (JogoNaoCadastradoException ex)
            {
                return NotFound("Não existe este jogo");
            }
        }
    }
}