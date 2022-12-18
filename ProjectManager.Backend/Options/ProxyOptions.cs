namespace ProjectManager.Backend.Options; 

public class ProxyOptions : OptionsFromConfiguration {
    public override string Position { get; set; } = "Proxy";

    public bool Enable { get; set; }
    public string Url { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Domain { get; set; }
    public string Host { get; set; }
}