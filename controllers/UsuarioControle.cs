using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("user")]
public class UsuarioControle : ControllerBase
{
    private readonly UsuarioRepositorio _usuarioRepositorio;

    public UsuarioControle(UsuarioRepositorio usuarioRepositorio)
    {
        _usuarioRepositorio = usuarioRepositorio;
    }

    // Rota de listagem de usuários do sistema
    public List<Usuario> Selecionar()
    {
        return _usuarioRepositorio.GetUsuarios();
    }

    // Rota de cadastro de usuário do sistema
    [HttpPost]
    public IActionResult Adicionar([FromBody] Usuario u)
    {
        if (u.Nome == "")
        {
            return BadRequest(new { mensagem = "O nome é obrigatório para cadastrar o usuário." });
        }
        else if (u.Idade < 0 || u.Idade > 100)
        {
            return BadRequest(new { message = "Idade inválida. Só é permitido idades entre 0 e 100 anos." });
        }
        else
        {
            var obj = _usuarioRepositorio.AddUsuario(u);
            return Created(string.Empty, obj);
        }
    }

    // Rota de remoção de usuário do sistema
    [HttpDelete("{id}")]
    public IActionResult Deletar(int id)
    {
        if (_usuarioRepositorio.GetUsuarioPeloId(id))
        {
            {
                _usuarioRepositorio.DelUsuario(id);
                _usuarioRepositorio.DelTransacao(id);
                return Ok(new { message = "Usuário e suas transações removidos com sucesso." });
            }
        }
        else
        {
            return NotFound(new { message = "Usuário não encontrado." });
        }
    }
}