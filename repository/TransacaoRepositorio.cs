using MySql.Data.MySqlClient;

public class TransacaoRepositorio
{
    private readonly string? _stringDeConexao;

    public TransacaoRepositorio(string stringDeConexao)
    {
        _stringDeConexao = stringDeConexao;
    }

    public List<Transacao> GetTransacoes()
    {
        // Cria uma variável chamada transações do tipo List<Transacao>
        List<Transacao> transacoes = [];

        // Configurar a conexão
        using var conexao = new MySqlConnection(_stringDeConexao);

        // Realizar a conexão
        conexao.Open();

        // Criar comando SQL para seleção de todas as transações
        using var comandoSQL = new MySqlCommand("SELECT * FROM transacoes", conexao);

        // Executar comando SQL e armazenar todos os registros
        using var registros = comandoSQL.ExecuteReader();

        // Laço de repetição
        while (registros.Read())
        {
            // Adicionar cada linha da tabela na variável transações
            transacoes.Add(new Transacao
            {
                Id = registros.GetInt32(registros.GetOrdinal("id")),
                Descricao = registros.GetString(registros.GetOrdinal("descricao")),
                Valor = registros.GetDecimal(registros.GetOrdinal("valor")),
                Tipo = registros.GetString(registros.GetOrdinal("tipo")),
                Pessoa = registros.GetInt32(registros.GetOrdinal("pessoa"))

            });
        }
        return transacoes;
    }

    public Usuario? GetUsuarioPeloId(int id)
    {
        // Configurar a conexão
        using var conexao = new MySqlConnection(_stringDeConexao);

        // Realizar a conexão
        conexao.Open();

        // Criar comando SQL para verificar a existência do usuário
        using var comandoSQL = new MySqlCommand("SELECT * FROM usuarios WHERE id = @id", conexao);

        // Especificar o valor do parâmetro id
        comandoSQL.Parameters.AddWithValue("@id", id);

        // Executar comando SQL e armazenar todos os registros
        using var reader = comandoSQL.ExecuteReader();

        if (reader.Read()) // Se encontrou um usuário
        {
            return new Usuario
            {
                Id = reader.GetInt32("id"),
                Nome = reader.GetString("nome"),
                Idade = reader.GetInt32("idade")
            };
        }
        return null; // Se não encontrar, retorna null
    }

    // Método para cadastrar
    public Transacao AddTransacao(Transacao t)
    {
        // Configurar a conexão
        using var conexao = new MySqlConnection(_stringDeConexao);

        // Realizar a conexão
        conexao.Open();

        // SQL
        string comandoSQL = "INSERT INTO transacoes (descricao, valor, tipo, pessoa) VALUES (@descricao, @valor, @tipo, @pessoa);";
        comandoSQL += "SELECT LAST_INSERT_ID();";

        // Variável contendo a ação SQL
        using var comando = new MySqlCommand(comandoSQL, conexao);

        // Informar os parâmetros (descrição, valor, tipo e pessoa)
        comando.Parameters.AddWithValue("@descricao", t.Descricao);
        comando.Parameters.AddWithValue("@valor", t.Valor);
        comando.Parameters.AddWithValue("@tipo", t.Tipo);
        comando.Parameters.AddWithValue("@pessoa", t.Pessoa);

        // ExecuteScalar -> Retorna a primeira linha/coluna
        int codigoGerado = Convert.ToInt32(comando.ExecuteScalar());

        // Especificar o id gerado no objeto t
        t.Id = codigoGerado;

        // Retorno
        return t;
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

    public class TotalTransacao
    {
        public int UsuarioId { get; set; }
        public string? NomeUsuario { get; set; }
        public decimal TotalReceitas { get; set; }
        public decimal TotalDespesas { get; set; }
        public decimal Saldo { get; set; }
    }

    public object GetTotaisTodosUsuarios()
    {
        // Configurar a conexão
        using var conexao = new MySqlConnection(_stringDeConexao);
        conexao.Open();

        // Criar e iniciar a lista do tipo TotalTransacao
        List<TotalTransacao> totaisUsuarios = new List<TotalTransacao>();

        // Criar comando SQL para obter os dados
        using var comandoSQL = new MySqlCommand(@"
        SELECT u.Id, u.Nome, 
               IFNULL(SUM(CASE WHEN t.tipo = 'receita' THEN t.valor END), 0) AS TotalReceitas,
               IFNULL(SUM(CASE WHEN t.tipo = 'despesa' THEN t.valor END), 0) AS TotalDespesas
        FROM usuarios u
        LEFT JOIN transacoes t ON u.Id = t.pessoa
        GROUP BY u.Id, u.Nome", conexao);

        // Executar comando SQL e armazenar os registros
        using var reader = comandoSQL.ExecuteReader();

        while (reader.Read())
        {
            var totalUsuario = new TotalTransacao
            {
                UsuarioId = reader.GetInt32(reader.GetOrdinal("Id")),
                NomeUsuario = reader.GetString(reader.GetOrdinal("Nome")),
                TotalReceitas = reader.GetDecimal(reader.GetOrdinal("TotalReceitas")),
                TotalDespesas = reader.GetDecimal(reader.GetOrdinal("TotalDespesas")),
                Saldo = reader.GetDecimal(reader.GetOrdinal("TotalReceitas")) -
                        reader.GetDecimal(reader.GetOrdinal("TotalDespesas"))
            };
            totaisUsuarios.Add(totalUsuario);
        }

        // Criar lista de objetos anônimos com os totais por pessoa
        List<object> pessoas = new List<object>();

        foreach (var t in totaisUsuarios)
        {
            pessoas.Add(new
            {
                Id = t.UsuarioId,
                Nome = t.NomeUsuario,
                Receita = t.TotalReceitas,
                Despesa = t.TotalDespesas,
                Saldo = t.TotalReceitas - t.TotalDespesas
            });
        }

        decimal totalReceita = totaisUsuarios.Sum(t => t.TotalReceitas);
        decimal totalDespesa = totaisUsuarios.Sum(t => t.TotalDespesas);
        decimal totalSaldo = totalReceita - totalDespesa;

        return new
        {
            Pessoas = pessoas,
            TotalReceita = totalReceita,
            TotalDespesa = totalDespesa,
            TotalSaldo = totalSaldo
        };
    }
}