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
        var dataPath = config.Require("serverBasePath") + "/nodered/data";

        ContainerArgs.Image = "nodered/node-red:" + (config.Get("nodeRedImageTag") ?? "latest");
        ContainerArgs.Envs = Output.Create(new[] { "TZ=America/Chicago" });
        ContainerArgs.NetworkMode = "host";
        ContainerArgs.Restart = "unless-stopped";
        ContainerArgs.Mounts = new List<ContainerMountArgs>{
            new ContainerMountArgs {
                Type = "bind",
                Source = dataPath,
                Target = "/data"
            }
        };
        ContainerArgs.Init = true;
        EnsureHostPath(dataPath);
    }
}
