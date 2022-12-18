using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Options;
using ProjectManager.Backend.Options;

namespace ProjectManager.Backend.Apis; 

public interface IDockerApi {
    public Task<string> CreateContainer(string image, int port, string hostVolumePath, string name);
    public Task StartContainer(string containerId);
    public Task StopContainer(string containerId);
    public Task DeleteContainer(string containerId);
    public Task<bool> IsContainerStarted(string containerId);
}

public sealed class DockerApi : IDockerApi {
    private readonly DockerClient _client;

    public DockerApi(IOptions<GeneralOptions> options) {
        _client = new DockerClientConfiguration(new Uri(options.Value.DockerPath)).CreateClient();
    }
    
    public async Task<string> CreateContainer(string image, int port, string hostVolumePath, string name) { 
        await _client.Images.CreateImageAsync(new() {
            FromImage = image
        }, null, new Progress<JSONMessage>());
        
        var container = await _client.Containers.CreateContainerAsync(new CreateContainerParameters {
            Image = image,
            ExposedPorts = new Dictionary<string, EmptyStruct> {
                { "8090/tcp", default }
            },
            HostConfig = new HostConfig {
                PortBindings = new Dictionary<string, IList<PortBinding>> {
                    { "8090/tcp", new List<PortBinding> { new() { HostPort = port.ToString() } } }
                },
                Binds = new List<string> {
                    $"{hostVolumePath}:/pb_data"
                }
            },
            Name = name
        });
        return container.ID;
    }

    public async Task StartContainer(string containerId) {
        await _client.Containers.StartContainerAsync(containerId, new());
    }

    public async Task StopContainer(string containerId) {
        await _client.Containers.StopContainerAsync(containerId, new());
    }

    public async Task DeleteContainer(string containerId) {
        await _client.Containers.RemoveContainerAsync(containerId, new());
    }

    public async Task<bool> IsContainerStarted(string containerId) {
        var containers = await _client.Containers.ListContainersAsync(new());
        var container = containers.SingleOrDefault(c => c.ID == containerId);
        return container != null;
    }
}