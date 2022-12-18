namespace ProjectManager.Backend.Options;

public class GeneralOptions : OptionsFromConfiguration {
    public override string Position { get; set; } = "General";

    public string Database { get; set; }
    public int TokenTimeInDays { get; set; }
    public string PortRange { get; set; }
    public string DockerPath { get; set; }
    public string Root { get; set; }
    public string PocketBaseVersion { get; set; }
    public int MaxProjects { get; set; }
}