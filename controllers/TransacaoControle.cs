using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("transaction")]
public class TransacaoControle : ControllerBase
{
    private readonly TransacaoRepositorio _transacaoRepositorio;

    public TransacaoControle(TransacaoRepositorio transacaoRepositorio)
    {
        _transacaoRepositorio = transacaoRepositorio;
    }

    // Rota de listagem de transações do sistema
    [HttpGet]
    public List<Transacao> Selecionar()
    {
        return _transacaoRepositorio.GetTransacoes();
    }

    // Rota de cadastro de transação do sistema
    [HttpPost]
    public IActionResult Adicionar([FromBody] Transacao t)
    {
        if (t.Tipo != "despesa" && t.Tipo != "receita")
        {
            return BadRequest(new { message = "É possível adicionar apenas transações que sejam receita ou despesa." });
        }

        if (t.Valor <= 0)
        {
            return BadRequest(new { message = "O valor precisa ser maior que 0." });
        }

        // Obtém o usuário pelo ID
        var usuario = _transacaoRepositorio.GetUsuarioPeloId(t.Pessoa);

        if (usuario == null)
        {
            return BadRequest(new { message = "Usuário não encontrado." });
        }

        // Validação do usuário menor de 18 anos que não pode adicionar receita
        if (usuario.Idade < 18 && t.Tipo == "receita")
        {
            return BadRequest(new { message = "Usuário precisa ter pelo menos 18 anos para adicionar transação do tipo receita." });
        }

        var obj = _transacaoRepositorio.AddTransacao(t);

        return Created(string.Empty, obj);
    }


    // Rota de remoção de transação do sistema
    [HttpDelete("{pessoa}")]
    public IActionResult Deletar(int pessoa)
    {
        var usuario = _transacaoRepositorio.GetUsuarioPeloId(pessoa);

        if (usuario == null)
        {
            return NotFound(new { message = "Usuário não encontrado." });
        }

        _transacaoRepositorio.DelTransacao(pessoa);
        return Ok(new { message = "Transação removida com sucesso." });
    }

    // Rota para obter os totais de transações (receitas, despesas e saldo)
    [HttpGet("totais")]
    public IActionResult ObterTotaisTodosUsuarios()
    {
        var resposta = _transacaoRepositorio.GetTotaisTodosUsuarios();
        
        return Ok(resposta);
    }

}