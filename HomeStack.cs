// Program.cs
using System.Collections.Generic;
using Pulumi;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

class ContainerConfig
{
    public string Name { get; set; } = null!;
    public string Image { get; set; } = null!;
    public string[]? Envs { get; set; }
    public string? NetworkMode { get; set; }
    public string Restart { get; set; } = "unless-stopped";
    public List<ContainerMountArgs>? Mounts { get; set; }
    public Provider Provider = null!;
}

class HomeStack : Stack
{
    public HomeStack()
    {
        var config = new Pulumi.Config();
        var basePath = config.Require("baseVolumePath");

        var server = new Provider("server", new ProviderArgs
        {
            Host = config.Require("serverHost")
        });

        var containers = new List<ContainerConfig>
        {
            new ContainerConfig
            {
                Name = "nodered",
                Image = $"nodered/node-red:{config.Get("nodeRedImageTag") ?? "latest"}",
                Envs = new[] { "TZ=America/Chicago" },
                NetworkMode = "host",
                Mounts = new List<ContainerMountArgs>
                {
                    new ContainerMountArgs
                    {
                        Type   = "bind",
                        Source = $"{basePath}/nodered/data",
                        Target = "/data"
                    }
                },
                Provider = server
            }
        };

        foreach (var cfg in containers)
        {
            var container = new Container(cfg.Name, new ContainerArgs
            {
                Name        = cfg.Name,
                Image       = cfg.Image,
                Restart     = cfg.Restart,
                NetworkMode = cfg.NetworkMode!,
                Envs        = cfg.Envs!,
                Mounts      = cfg.Mounts!,
                Init        = true
            }, new CustomResourceOptions
            {
                Provider = cfg.Provider
            });
        }
    }
}
