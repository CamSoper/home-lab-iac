using System;
using System.Collections.Generic;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public class HomeAssistant : ContainerConfigBase
{
    public HomeAssistant(String name, Pulumi.Config config, Provider provider) : base(name, config, provider)
    {
        ContainerArgs.Image = "homeassistant/home-assistant:" + (config.Get("homeAssistantImageTag") ?? "latest");
        var configPath = config.Require("serverBasePath") + "/hass/config";
        var snapshotsPath = config.Require("serverBasePath") + "/hass/media/snapshots";
        ContainerArgs.Mounts = new List<ContainerMountArgs> {
            new ContainerMountArgs {
                Type = "bind",
                Source = configPath,
                Target = "/config"
            },
            new ContainerMountArgs {
                Type = "bind",
                Source = "/etc/localtime",
                Target = "/etc/localtime",
                ReadOnly = true
            },
            new ContainerMountArgs {
                Type = "bind",
                Source = "/var/run/docker.sock",
                Target = "/var/run/docker.sock"
            },
            new ContainerMountArgs {
                Type = "bind",
                Source = snapshotsPath,
                Target = "/media/snapshots"
            }
        };
        HostDirectories.Add(configPath);
        HostDirectories.Add(snapshotsPath);
        ContainerArgs.NetworkMode = "host";
        ContainerArgs.Restart = "unless-stopped";
        ContainerArgs.Privileged = true;
    }
}