namespace Shared.Options;

public class SmtpEmailOptions
{
    public string Host { get; set; }
    public string Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string FromEmail { get; set; }
}