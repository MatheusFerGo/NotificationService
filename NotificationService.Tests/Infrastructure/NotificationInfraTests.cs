using System;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NotificationService.Infrastructure.Adapters;
using NotificationService.Infrastructure.Configurations;
using Xunit;

namespace NotificationService.Tests.Infrastructure;

public class EmailSenderAdapterTests
{
    private readonly Mock<ILogger<EmailSenderAdapter>> _loggerMock;
    private readonly IOptions<AzureEmailSettings> _optionsSettings;
    private readonly AzureEmailSettings _settings;

    public EmailSenderAdapterTests()
    {
        _loggerMock = new Mock<ILogger<EmailSenderAdapter>>();

        _settings = new AzureEmailSettings
        {
            SenderAddress = "donotreply@fiapx.com",
            ConnectionString = "endpoint=https://teste.communication.azure.com/;accesskey=teste"
        };

        _optionsSettings = Options.Create(_settings);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldReturnTrue_WhenAzureSucceeds()
    {
        var emailClientMock = new Mock<EmailClient>();

        var operationMock = new Mock<EmailSendOperation>();
        operationMock.SetupGet(x => x.Id).Returns("op-12345"); // Simulando um ID de sucesso

        emailClientMock
            .Setup(x => x.SendAsync(
                It.IsAny<WaitUntil>(),
                It.IsAny<EmailMessage>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(operationMock.Object); // Retorna a operação "fake"

        var sut = new EmailSenderAdapter(emailClientMock.Object, _loggerMock.Object, _optionsSettings);

        var result = await sut.SendEmailAsync("teste@teste.com", "Assunto", "Corpo");

        Assert.True(result);
    }

    [Fact(DisplayName = "Deve retornar FALSE e logar erro quando o Azure lançar RequestFailedException")]
    public async Task SendEmailAsync_ShouldReturnFalse_WhenAzureThrowsRequestFailedException()
    {
        var emailClientMock = new Mock<EmailClient>();

        emailClientMock
            .Setup(x => x.SendAsync(
                It.IsAny<WaitUntil>(),
                It.IsAny<EmailMessage>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException("Falha na API do Azure ACS")); // Força o erro específico

        var sut = new EmailSenderAdapter(emailClientMock.Object, _loggerMock.Object, _optionsSettings);

        var result = await sut.SendEmailAsync("teste@teste.com", "Assunto", "Corpo");

        Assert.False(result);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact(DisplayName = "Deve retornar FALSE e logar erro quando ocorrer uma Exception genérica")]
    public async Task SendEmailAsync_ShouldReturnFalse_WhenGenericExceptionOccurs()
    {
        var emailClientMock = new Mock<EmailClient>();

        emailClientMock
            .Setup(x => x.SendAsync(
                It.IsAny<WaitUntil>(),
                It.IsAny<EmailMessage>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Erro de rede inesperado")); // Força um erro genérico

        var sut = new EmailSenderAdapter(emailClientMock.Object, _loggerMock.Object, _optionsSettings);

        var result = await sut.SendEmailAsync("teste@teste.com", "Assunto", "Corpo");

        Assert.False(result);
    }
}
