using System;
using System.Collections.Generic;
using Pulumi;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public class NodeRed : ContainerConfigBase
{
    public NodeRed(String name, Pulumi.Config config, Provider provider) : base(name, config, provider)
    {
        ContainerArgs.Image = "nodered/node-red:" + (config.Get("nodeRedImageTag") ?? "latest");
        ContainerArgs.Envs = Output.Create(new[] { "TZ=America/Chicago" });
        ContainerArgs.NetworkMode = "host";
        ContainerArgs.Restart = "unless-stopped";
        var dataPath = config.Require("serverBasePath") + "/nodered/data";
        ContainerArgs.Mounts = new List<ContainerMountArgs>{
            new ContainerMountArgs {
                Type = "bind",
                Source = dataPath,
                Target = "/data"
            }
        };
        HostDirectories.Add(dataPath);
        ContainerArgs.Init = true;
    }
}
