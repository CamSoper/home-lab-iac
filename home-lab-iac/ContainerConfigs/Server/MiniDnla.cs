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
        var videoPath = "/mnt/raid/Videos";
        var musicPath = "/mnt/raid/Music";
        var picturesPath = "/mnt/raid/Pictures";
        ContainerArgs.Mounts = new List<ContainerMountArgs> {
            new ContainerMountArgs {
                Source = videoPath,
                Target = "/opt/Videos",
                Type = "bind"
            },
            new ContainerMountArgs {
                Source = musicPath,
                Target = "/opt/Music",
                Type = "bind"
            },
            new ContainerMountArgs {
                Source = picturesPath,
                Target = "/opt/Pictures",
                Type = "bind"
            }
        };
        HostDirectories.Add(videoPath);
        HostDirectories.Add(musicPath);
        HostDirectories.Add(picturesPath);
        ContainerArgs.Restart = "unless-stopped";
        ContainerArgs.NetworkMode = "host";
    }
}
