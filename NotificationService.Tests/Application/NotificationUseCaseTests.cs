using System.Threading.Tasks;
using Moq;
using NotificationService.Application.DTOs;
using NotificationService.Application.Services;
using NotificationService.Domain.Interfaces;
using Xunit;

namespace NotificationService.Tests.Application;

public class NotificationUseCaseTests
{
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly NotificationUseCase _sut; 

    public NotificationUseCaseTests()
    {
        _emailSenderMock = new Mock<IEmailSender>();

        _sut = new NotificationUseCase(_emailSenderMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldSendSuccessEmail_WhenFlagIsY()
    {
        var videoEvent = new VideoEvent(
            "123", "Matheus", "teste@teste.com", "v-1", "Video Teste", "success", "Y", "N"
        );

        await _sut.ExecuteAsync(videoEvent);

        _emailSenderMock.Verify(x => x.SendEmailAsync(
            "teste@teste.com",
            It.Is<string>(subject => subject.Contains("sucesso!")),
            It.Is<string>(body => body.Contains("foi processado com sucesso e já está disponível"))
        ), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldSendErrorEmail_WhenFlagIsY()
    {
        var videoEvent = new VideoEvent(
            "123", "Matheus", "teste@teste.com", "v-1", "Video Teste", "error", "N", "Y"
        );

        await _sut.ExecuteAsync(videoEvent);

        _emailSenderMock.Verify(x => x.SendEmailAsync(
            "teste@teste.com",
            It.Is<string>(subject => subject.Contains("Falha")),
            It.Is<string>(body => body.Contains("apresentou um erro durante o processamento"))
        ), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotSendEmail_WhenEntitySaysNo()
    {
        var videoEvent = new VideoEvent(
            "123", "Matheus", "teste@teste.com", "v-1", "Video Teste", "success", "N", "Y"
        );

        await _sut.ExecuteAsync(videoEvent);

        _emailSenderMock.Verify(x => x.SendEmailAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()
        ), Times.Never);
    }
}