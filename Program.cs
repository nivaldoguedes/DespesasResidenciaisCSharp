// Cria e configura o objeto `WebApplicationBuilder`, que é usado para configurar os serviços e o pipeline de solicitação HTTP.
var builder = WebApplication.CreateBuilder(args);

// Configura os serviços necessários para a aplicação. 
// `AddControllers` adiciona suporte para controllers MVC, o que permite criar endpoints de API e renderizar views.
builder.Services.AddControllers();

// Obtém a string de conexão para o banco de dados a partir da configuração do aplicativo.
// Se a string de conexão 'DefaultConnection' não for encontrada, uma exceção é lançada para indicar que a configuração está ausente.
var stringDeConexao = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("A string de conexão 'DefaultConnection' não foi encontrada.");

// Registra o repositório `UsuarioRepositorio` como um serviço singleton no contêiner de injeção de dependência.
// Um singleton garante que apenas uma instância do repositório será criada e usada em toda a aplicação.
builder.Services.AddScoped<UsuarioRepositorio>(sp => new UsuarioRepositorio(stringDeConexao));
builder.Services.AddScoped<TransacaoRepositorio>(sp => new TransacaoRepositorio(stringDeConexao));


// Constrói o objeto `WebApplication` com base nas configurações fornecidas e no contêiner de serviços configurado.
var app = builder.Build();

// Mapeia os controllers para os endpoints disponíveis na aplicação.
// Isso faz com que os controllers sejam acessíveis através das rotas definidas nas suas classes de controller.
app.MapControllers();

// Adiciona um evento que será executado após a inicialização
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine("Aplicação iniciada!");
});

// Inicia a aplicação e começa a escutar as solicitações HTTP.
app.Run();