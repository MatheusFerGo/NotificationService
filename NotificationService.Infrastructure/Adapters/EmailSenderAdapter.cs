using Azure;
using Azure.Communication.Email;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NotificationService.Infrastructure.Adapters;

public class EmailSenderAdapter : IEmailSender
{
    private readonly EmailClient _emailClient;
    private readonly ILogger<EmailSenderAdapter> _logger;
    private readonly AzureEmailSettings _settings;

    public EmailSenderAdapter(
        EmailClient emailClient,
        ILogger<EmailSenderAdapter> logger,
        IOptions<AzureEmailSettings> settings)
    {
        _emailClient = emailClient;
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            _logger.LogInformation("Iniciando disparo de e-mail via Azure ACS para {To}", to);

            var emailMessage = new EmailMessage(
                senderAddress: _settings.SenderAddress,
                content: new EmailContent(subject)
                {
                    PlainText = body,
                    Html = $@"
                        <html>
                            <body style='font-family: sans-serif;'>
                                <h2 style='color: #2c3e50;'>Notificação FIAP X</h2>
                                <p>{body.Replace("\n", "<br>")}</p>
                                <hr />
                                <small style='color: #7f8c8d;'>Este é um e-mail automático, por favor não responda.</small>
                            </body>
                        </html>"
                },
                recipients: new EmailRecipients(new List<EmailAddress> { new EmailAddress(to) })
            );

            EmailSendOperation emailSendOperation = await _emailClient.SendAsync(
                WaitUntil.Completed,
                emailMessage);

            _logger.LogInformation("E-mail enviado com sucesso. ID da Operação: {Id}", emailSendOperation.Id);
            return true;
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Erro de requisição ao Azure Communication Services para {To}", to);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao processar envio de e-mail para {To}", to);
            return false;
        }
    }
}