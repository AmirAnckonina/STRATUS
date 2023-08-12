using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using System.Text;
using Utils.DTO;
using StratusApp.Settings;

namespace StratusApp.Services;
public class EmailService
{
    private readonly IConfiguration _configuration;
    private const string STRATUS_EMAIL = "erez6356458@gmail.com";
    private readonly string _apiKey;

    public EmailService(AppSettings appSettings)
    {
        _apiKey = appSettings.EmailSettings.ApiKey;
    }

    public async Task SendAlertsEmailAsync(string toEmail, List<AlertData> alerts)
    {
        string subject = "STRATUS ALERT";
        
        var client = new SendGridClient(_apiKey);
        var from = new EmailAddress(STRATUS_EMAIL, "Stratus");
        // TODO : get user email from DB
        var to = new EmailAddress(toEmail, "Example User");

        var tableHtml = GenerateTableHtml(alerts);

        var htmlContent = $@"
            <html>
                <body>
                    <h2>Alert Data Report</h2>
                    {tableHtml}
                </body>
            </html>";

        var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
        var response = await client.SendEmailAsync(msg);
    }

    private string GenerateTableHtml(List<AlertData> alertDataList)
    {
        var tableBuilder = new StringBuilder();
        tableBuilder.AppendLine("<table border='1'>");
        tableBuilder.AppendLine("<tr><th>Machine</th><th>Type</th><th>Creation Time</th><th>Percentage Usage</th></tr>");

        foreach (var alertData in alertDataList)
        {
            tableBuilder.AppendLine("<tr>");
            tableBuilder.AppendLine($"<td>{alertData.MachineId}</td>");
            tableBuilder.AppendLine($"<td>{alertData.Type}</td>");
            tableBuilder.AppendLine($"<td>{alertData.CreationTime}</td>");
            tableBuilder.AppendLine($"<td>{alertData.PercentageUsage}</td>");
            tableBuilder.AppendLine("</tr>");
        }

        tableBuilder.AppendLine("</table>");

        return tableBuilder.ToString();
    }
}
