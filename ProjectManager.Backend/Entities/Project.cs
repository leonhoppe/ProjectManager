namespace ProjectManager.Backend.Entities; 

public class Project {
    public string ProjectId { get; set; }
    public string OwnerId { get; set; }
    public string Name { get; set; }
    public int Port { get; set; }
    public string ContainerName { get; set; }
    public int ProxyId { get; set; } = -1;
    public int CertificateId { get; set; } = -1;
}

public class ProjectEdit {
    public string Name { get; set; }
}