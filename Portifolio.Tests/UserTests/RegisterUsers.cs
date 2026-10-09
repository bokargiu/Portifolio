namespace Portifolio.Tests;
using Portifolio.Server.DTOs.Users;
using System.Net;
using System.Net.Http.Json;
using Xunit;

public class RegisterUsers : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    public RegisterUsers(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }
    // Cenário 1: cadastro realizado com sucesso.
    [Fact]
    public async Task RegistrandoComSucesso()
    {
        var dto = NovoUsuario();

        using var response = await RegistrarAsync(dto);

        await AssertStatusAsync(response, HttpStatusCode.Created);
    }

    // Cenário 2: senha com menos de 8 caracteres.
    [Fact]
    public async Task RejeitandoSenhaMuitoCurta()
    {
        var dto = NovoUsuario(password: "Ab1!");

        using var response = await RegistrarAsync(dto);

        await AssertStatusAsync(response, HttpStatusCode.BadRequest);
    }

    // Cenário 3: senha sem pelo menos 3 categorias de caracteres.
    [Theory]
    [InlineData("abcdefgh")] // Apenas letras minúsculas
    [InlineData("ABCDEFGH")] // Apenas letras maiúsculas
    [InlineData("12345678")] // Apenas números
    [InlineData("Abcdefgh")] // Maiúsculas e minúsculas
    [InlineData("abcdef12")] // Minúsculas e números
    [InlineData("ABCDEF12")] // Maiúsculas e números
    public async Task RejeitandoSenhaSemComplexidadeSuficiente(string senha)
    {
        var dto = NovoUsuario(password: senha);

        using var response = await RegistrarAsync(dto);

        await AssertStatusAsync(response, HttpStatusCode.BadRequest);
    }

    // Cenário 4: nome informado no formato de e-mail.
    [Fact]
    public async Task RejeitandoNomeQueEhEmail()
    {
        var dto = NovoUsuario(
            name: $"usuario-{Guid.NewGuid():N}@example.com"
        );

        using var response = await RegistrarAsync(dto);

        await AssertStatusAsync(response, HttpStatusCode.BadRequest);
    }

    // Cenário 5: e-mail inválido.
    [Theory]
    [InlineData("email-invalido")]
    [InlineData("usuario@")]
    [InlineData("@example.com")]
    [InlineData("usuario.example.com")]
    public async Task RejeitandoEmailInvalido(string email)
    {
        var dto = NovoUsuario(email: email);

        using var response = await RegistrarAsync(dto);

        await AssertStatusAsync(response, HttpStatusCode.BadRequest);
    }

    // Cenário 6: e-mail já cadastrado.
    [Fact]
    public async Task RejeitandoEmailDuplicado()
    {
        var primeiroUsuario = NovoUsuario();

        using var primeiraResposta =
            await RegistrarAsync(primeiroUsuario);

        await AssertStatusAsync(
            primeiraResposta,
            HttpStatusCode.Created
        );

        var segundoUsuario = NovoUsuario(
            email: primeiroUsuario.Email
        );

        using var segundaResposta =
            await RegistrarAsync(segundoUsuario);

        await AssertStatusAsync(
            segundaResposta,
            HttpStatusCode.BadRequest
        );
    }

    // Cenário 7: nome já cadastrado.
    [Fact]
    public async Task RejeitandoNomeDuplicado()
    {
        var primeiroUsuario = NovoUsuario();

        using var primeiraResposta =
            await RegistrarAsync(primeiroUsuario);

        await AssertStatusAsync(
            primeiraResposta,
            HttpStatusCode.Created
        );

        var segundoUsuario = NovoUsuario(
            name: primeiroUsuario.Name
        );

        using var segundaResposta =
            await RegistrarAsync(segundoUsuario);

        await AssertStatusAsync(
            segundaResposta,
            HttpStatusCode.BadRequest
        );
    }

    // Cenário 8: nome com espaços nas extremidades.
    // O UserService deve remover os espaços antes de salvar.
    [Fact]
    public async Task NormalizandoEspacosDoNome()
    {
        var usuario = NovoUsuario(
            name: $"  Usuario-{Guid.NewGuid():N}  "
        );

        using var primeiraResposta =
            await RegistrarAsync(usuario);

        await AssertStatusAsync(
            primeiraResposta,
            HttpStatusCode.Created
        );

        // O nome normalizado não deve ser cadastrado novamente.
        var segundoUsuario = NovoUsuario(
            name: usuario.Name.Trim()
        );

        using var segundaResposta =
            await RegistrarAsync(segundoUsuario);

        await AssertStatusAsync(
            segundaResposta,
            HttpStatusCode.BadRequest
        );
    }

    // Cenário 9: senha com exatamente os critérios mínimos
    // de tamanho, maiúscula, minúscula e número.
    [Fact]
    public async Task AceitandoSenhaComTresCategorias()
    {
        var dto = NovoUsuario(password: "Test1234");

        using var response = await RegistrarAsync(dto);

        await AssertStatusAsync(response, HttpStatusCode.Created);
    }

    // Cenário 10: corpo JSON null.
    [Fact]
    public async Task RejeitandoUsuarioNulo()
    {
        using var response = await _client.PostAsJsonAsync(
            "/api/User",
            (TemplateUser?)null,
            TestContext.Current.CancellationToken
        );

        await AssertStatusAsync(
            response,
            HttpStatusCode.BadRequest
        );
    }

    private Task<HttpResponseMessage> RegistrarAsync(
        TemplateUser dto)
    {
        return _client.PostAsJsonAsync(
            "/api/User",
            dto,
            TestContext.Current.CancellationToken
        );
    }

    private static TemplateUser NovoUsuario(
        string? name = null,
        string? email = null,
        string password = "Test@123")
    {
        var identificador = Guid.NewGuid().ToString("N");

        return new TemplateUser
        {
            Name = name ?? $"Usuario-{identificador}",
            Email = email ?? $"usuario-{identificador}@example.com",
            Password = password
        };
    }

    private static async Task AssertStatusAsync(
        HttpResponseMessage response,
        HttpStatusCode esperado)
    {
        var corpo = await response.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken
        );

        Assert.True(
            response.StatusCode == esperado,
            $"Status esperado: {(int)esperado} ({esperado}). " +
            $"Recebido: {(int)response.StatusCode} ({response.StatusCode}). " +
            $"Resposta da API: {corpo}"
        );
    }
}
