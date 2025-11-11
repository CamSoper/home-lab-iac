using System;
using System.Collections.Generic;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public class NetAlertX : ContainerConfigBase
{
    public NetAlertX(String name, Pulumi.Config config, Provider provider) : base(name, config, provider)
    {
        ContainerArgs.Image = "ghcr.io/jokob-sk/netalertx:latest";
        ContainerArgs.NetworkMode = "host";
        ContainerArgs.Restart = "unless-stopped";
        ContainerArgs.Mounts = new List<ContainerMountArgs> {
            new ContainerMountArgs {
                Source = config.Require("serverBasePath") + "/netalertx/config",
                Target = "/app/config",
                Type = "bind"
            },
            new ContainerMountArgs {
                Source = config.Require("serverBasePath") + "/netalertx/db",
                Target = "/app/db",
                Type = "bind"
            }
        };
        ContainerArgs.Tmpfs = new Dictionary<string, string> {
            { "/app/api", "" }
        };
        ContainerArgs.Dns = new List<string> {
            "192.168.15.1"
        };
        ContainerArgs.Envs = new List<string> {
            "PUID=200",
            "PGID=300",
            "TZ=America/Chicago",
            "PORT=20211"
        };
    }
}
