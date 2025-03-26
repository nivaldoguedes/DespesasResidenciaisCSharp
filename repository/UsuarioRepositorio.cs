using MySql.Data.MySqlClient;

public class UsuarioRepositorio
{
    private readonly string? _stringDeConexao;

    public UsuarioRepositorio(string stringDeConexao)
    {
        _stringDeConexao = stringDeConexao;
    }

    public List<Usuario> GetUsuarios()
    {
        // Cria uma variável chamada usuarios do tipo List<Usuario>
        List<Usuario> usuarios = [];

        // Configurar a conexão
        using var conexao = new MySqlConnection(_stringDeConexao);

        // Realizar a conexão
        conexao.Open();

        // Criar comando SQL para seleção de todos os usuarios
        using var comandoSQL = new MySqlCommand("SELECT * FROM usuarios", conexao);

        // Executar comando SQL e armazenar todos os registros
        using var registros = comandoSQL.ExecuteReader();

        // Laço de repetição
        while (registros.Read())
        {
            // Adicionar cada linha da tabela na variável usuarios
            usuarios.Add(new Usuario
            {
                Id = registros.GetInt32("id"),
                Nome = registros.GetString("nome"),
                Idade = registros.GetInt32("idade")
            });
        }
        return usuarios;
    }

    public bool GetUsuarioPeloId(int id)
    {
        // Configurar a conexão
        using var conexao = new MySqlConnection(_stringDeConexao);

        // Realizar a conexão
        conexao.Open();

        // Criar comando SQL para verificar a existência do usuário
        using var comandoSQL = new MySqlCommand("SELECT COUNT(*) FROM usuarios WHERE id = @id", conexao);

        // Especificar o valor do parâmetro id
        comandoSQL.Parameters.AddWithValue("@id", id);

        // Executar comando SQL e retornar o resultado
        int contador = Convert.ToInt32(comandoSQL.ExecuteScalar());
        
        if (contador == 0) {
            return false;
        }
        else {
            return true;
        }
    }

    // Método para cadastrar
    public Usuario AddUsuario(Usuario u)
    {
        // Configurar a conexão
        using var conexao = new MySqlConnection(_stringDeConexao);

        // Realizar a conexão
        conexao.Open();

        // SQL
        string comandoSQL = "INSERT INTO usuarios (nome, idade) VALUES (@nome, @idade);";
        comandoSQL += "SELECT LAST_INSERT_ID();";

        // Variável contendo a ação SQL
        using var comando = new MySqlCommand(comandoSQL, conexao);

        // Informar os parâmetros (nome e idade)
        comando.Parameters.AddWithValue("@nome", u.Nome);
        comando.Parameters.AddWithValue("@idade", u.Idade);

        // ExecuteScalar -> Retorna a primeira linha/coluna
        int codigoGerado = Convert.ToInt32(comando.ExecuteScalar());

        // Especificar o id gerado no objeto u
        u.Id = codigoGerado;

        // Retorno
        return u;
    }

    // Método para remover usuário cadastrado
    public void DelUsuario(int id)
    {
        // Configurar a conexão
        using var conexao = new MySqlConnection(_stringDeConexao);

        // Realizar a conexão
        conexao.Open();

        // Criar comando SQL para remover usuarios
        using var comandoSQL = new MySqlCommand("DELETE FROM usuarios WHERE id = @id", conexao);

        // Especificar o valor do parâmetro id
        comandoSQL.Parameters.AddWithValue("@id", id);

        // Executar comando SQL
        comandoSQL.ExecuteNonQuery();
    }

    public void DelTransacao(int pessoa)
    {
        // Configurar a conexão
        using var conexao = new MySqlConnection(_stringDeConexao);

        // Realizar a conexão
        conexao.Open();

        // Criar comando SQL para remover transações
        using var comandoSQL = new MySqlCommand("DELETE FROM transacoes WHERE pessoa = @pessoa", conexao);

        // Especificar o valor do parâmetro pessoa
        comandoSQL.Parameters.AddWithValue("@pessoa", pessoa);

        // Executar comando SQL
        comandoSQL.ExecuteNonQuery();
    }
}