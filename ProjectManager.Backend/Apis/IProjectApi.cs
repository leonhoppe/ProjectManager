using Microsoft.Extensions.Options;
using ProjectManager.Backend.Entities;
using ProjectManager.Backend.Options;

namespace ProjectManager.Backend.Apis; 

public interface IProjectApi {
    public Project[] GetProjects(string userId);
    public Project GetProject(string projectId);
    public Task<string> AddProject(string name, string ownerId);
    public bool EditProject(string projectId, string name);
    public Task DeleteProject(string projectId);
}

public class ProjectApi : IProjectApi {
    private readonly DatabaseContext _context;
    private readonly GeneralOptions _options;
    private readonly IDockerApi _docker;
    private readonly IProxyApi _proxy;

    public ProjectApi(DatabaseContext context, IOptions<GeneralOptions> options, IDockerApi docker, IProxyApi proxy) {
        _context = context;
        _options = options.Value;
        _docker = docker;
        _proxy = proxy;
    }
    
    public Project[] GetProjects(string userId) {
        return _context.Projects.Where(project => project.OwnerId == userId).ToArray();
    }

    public Project GetProject(string projectId) {
        return _context.Projects.SingleOrDefault(project => project.ProjectId == projectId);
    }

    public async Task<string> AddProject(string name, string ownerId) {
        if (name.Length > 255) return null;
        
        // Get available port
        var split = _options.PortRange.Split('-');
        var portRange = Enumerable.Range(int.Parse(split[0]), int.Parse(split[1]));
        var usedPorts = _context.Projects.Select(p => p.Port).ToArray();
        var port = portRange.FirstOrDefault(port => !usedPorts.Contains(port));
        if (port == 0) return null;

        var project = new Project {
            ProjectId = Guid.NewGuid().ToString(),
            OwnerId = ownerId,
            Name = name,
            Port = port
        };

        var container = await _docker.CreateContainer($"ghcr.io/muchobien/pocketbase:{_options.PocketBaseVersion}", port, _options.Root + project.ProjectId, $"{project.Name}_{project.ProjectId}");
        await _docker.StartContainer(container);
        project.ContainerName = container;
        
        var (proxyId, certificateId) = await _proxy.AddLocation(project.ProjectId, project.Port);
        project.ProxyId = proxyId;
        project.CertificateId = certificateId;
        
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return project.ProjectId;
    }

    public bool EditProject(string projectId, string name) {
        if (name.Length > 255) return false;
        var project = GetProject(projectId);
        if (project == null) return false;
        project.Name = name;
        _context.Projects.Update(project);
        _context.SaveChanges();
        return true;
    }

    public async Task DeleteProject(string projectId) {
        if (!_context.Projects.Any(project => project.ProjectId == projectId)) return;

        var project = _context.Projects.Single(project => project.ProjectId == projectId);
        await _docker.StopContainer(project.ContainerName);
        await _docker.DeleteContainer(project.ContainerName);

        await _proxy.RemoveLocation(project.ProxyId, project.CertificateId);

        Directory.Delete(_options.Root + projectId, true);
        
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
    }
}