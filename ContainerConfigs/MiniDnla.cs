using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Pulumi;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public class MiniDnla : ContainerConfigBase
{
    public MiniDnla(Pulumi.Config config, Provider provider) : base(config, provider)
    {
        Name = "minidlna";
        Image = "geekduck/minidlna:" + (config.Get("miniDnlaImageTag") ?? "latest");
        Mounts = new List<ContainerMountArgs>
        {
            new ContainerMountArgs
            {
                Source = "/mnt/raid/Videos",
                Target = "/opt/Videos",
                Type = "bind"
            },
            new ContainerMountArgs
            {
                Source = "/mnt/raid/Music",
                Target = "/opt/Music",
                Type = "bind"
            },
            new ContainerMountArgs
            {
                Source = "/mnt/raid/Pictures",
                Target = "/opt/Pictures",
                Type = "bind"
            }
        };
        Restart = "unless-stopped";
        NetworkMode = "host";
    }
}
