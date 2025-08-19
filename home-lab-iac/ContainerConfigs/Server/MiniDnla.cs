using System;
using System.Collections.Generic;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public class MiniDnla : ContainerConfigBase
{
    public MiniDnla(String name, Pulumi.Config config, Provider provider) : base(name, config, provider)
    {
        ContainerArgs.Image = "geekduck/minidlna:" + (config.Get("miniDnlaImageTag") ?? "latest");
        ContainerArgs.Mounts = new List<ContainerMountArgs> {
            new ContainerMountArgs {
                Source = "/mnt/raid/Videos",
                Target = "/opt/Videos",
                Type = "bind"
            },
            new ContainerMountArgs {
                Source = "/mnt/raid/Music",
                Target = "/opt/Music",
                Type = "bind"
            },
            new ContainerMountArgs {
                Source = "/mnt/raid/Pictures",
                Target = "/opt/Pictures",
                Type = "bind"
            }
        };
        ContainerArgs.Restart = "unless-stopped";
        ContainerArgs.NetworkMode = "host";
    }
}
