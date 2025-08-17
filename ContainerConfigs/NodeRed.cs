using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Pulumi;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public class NodeRed : ContainerConfigBase
{
    public NodeRed(Pulumi.Config config, Provider provider) : base(config, provider)
    {
        Name = "nodered";
        Image = "nodered/node-red:" + (config.Get("nodeRedImageTag") ?? "latest");
        Envs = Output.Create(new[] { "TZ=America/Chicago" });
        NetworkMode = "host";
        Restart = "unless-stopped";
        Mounts = new List<ContainerMountArgs>
        {
            new ContainerMountArgs
            {
                Type = "bind",
                Source = config.Require("baseVolumePath") + "/nodered/data",
                Target = "/data"
            }
        };
        Init = true;
    }
}
