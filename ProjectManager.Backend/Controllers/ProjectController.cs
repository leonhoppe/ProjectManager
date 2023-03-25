using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProjectManager.Backend.Apis;
using ProjectManager.Backend.Entities;
using ProjectManager.Backend.Options;
using ProjectManager.Backend.Security;

namespace ProjectManager.Backend.Controllers; 

[ApiController]
[Route("projects")]
public class ProjectController : ControllerBase {
    private readonly IProjectApi _projects;
    private readonly ITokenContext _context;
    private readonly IUserApi _users;
    private readonly IDockerApi _docker;
    private readonly ProxyOptions _options;

    public ProjectController(
        IProjectApi projects, 
        ITokenContext context, 
        IUserApi users, 
        IDockerApi docker,
        IOptions<ProxyOptions> options
        ) {
        _projects = projects;
        _context = context;
        _users = users;
        _docker = docker;
        _options = options.Value;
    }

    [Authorized]
    [HttpGet]
    public async Task<IActionResult> GetProjects() {
        var projects = _projects.GetProjects(_context.UserId);
        var running = new bool[projects.Length];
        for (int i = 0; i < projects.Length; i++)
            running[i] = await _docker.IsContainerStarted(projects[i].ContainerName);
        return Ok(new { projects, running });
    }

    [Authorized]
    [HttpGet("{projectId}")]
    public IActionResult GetProject(string projectId) {
        var project = _projects.GetProject(projectId);
        if (project == null) return NotFound();
        if (project.OwnerId != _context.UserId) return Unauthorized();
        return Ok(project);
    }

    [Authorized]
    [HttpPost]
    public async Task<IActionResult> AddProject([FromBody] ProjectEdit edit) {
        if (!_users.CanCreateProject(_context.UserId)) return Forbid();
        var projectId = await _projects.AddProject(edit.Name, _context.UserId);
        if (projectId == null) return BadRequest();
        return Ok(new { ProjectId = projectId });
    }

    [Authorized]
    [HttpDelete("{projectId}")]
    public async Task<IActionResult> DeleteProject(string projectId) {
        var project = _projects.GetProject(projectId);
        if (project == null) return NotFound();
        if (project.OwnerId != _context.UserId) return Unauthorized();
        await _projects.DeleteProject(projectId);
        return Ok();
    }

    [Authorized]
    [HttpPut("{projectId}")]
    public async Task<IActionResult> EditProject(string projectId, [FromBody] ProjectEdit edit) {
        var project = _projects.GetProject(projectId);
        if (project == null) return NotFound();
        if (project.OwnerId != _context.UserId) return Unauthorized();
        await _projects.EditProject(projectId, edit.Name, edit.Domain);
        return Ok();
    }

    [Authorized]
    [HttpGet("{projectId}/url")]
    public IActionResult GetProjectUrl(string projectId) {
        var project = _projects.GetProject(projectId);
        if (project == null) return NotFound();
        if (project.OwnerId != _context.UserId) return Unauthorized();
        if (_options.Enable) return Redirect($"https://{project.Domain}/_/");
        return Redirect($"http://{_options.Host}:{project.Port}/_/");
    }

    [Authorized]
    [HttpGet("{projectId}/url/string")]
    public IActionResult GetProjectUrlHead(string projectId) {
        var project = _projects.GetProject(projectId);
        if (project == null) return NotFound();
        if (project.OwnerId != _context.UserId) return Unauthorized();
        if (_options.Enable) return Ok(new {url = $"https://{project.Domain}/_/"});
        return Ok(new {url = $"http://{_options.Host}:{project.Port}/_/"});
    }

    [Authorized]
    [HttpGet("{projectId}/start")]
    public async Task<IActionResult> StartProject(string projectId) {
        var project = _projects.GetProject(projectId);
        if (project == null) return NotFound();
        if (project.OwnerId != _context.UserId) return Unauthorized();
        await _docker.StartContainer(project.ContainerName);
        return Ok();
    }
    
    [Authorized]
    [HttpGet("{projectId}/stop")]
    public async Task<IActionResult> StopProject(string projectId) {
        var project = _projects.GetProject(projectId);
        if (project == null) return NotFound();
        if (project.OwnerId != _context.UserId) return Unauthorized();
        await _docker.StopContainer(project.ContainerName);
        return Ok();
    }
    
    [Authorized]
    [HttpGet("{projectId}/status")]
    public async Task<IActionResult> ProjectStatus(string projectId) {
        var project = _projects.GetProject(projectId);
        if (project == null) return NotFound();
        if (project.OwnerId != _context.UserId) return Unauthorized();
        return Ok(new { Running = await _docker.IsContainerStarted(project.ContainerName) });
    }
}