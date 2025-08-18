using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Pulumi;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public class HomeAssistant : ContainerConfigBase
{
    public HomeAssistant(String name, Pulumi.Config config, Provider provider, string basePath) : base(name, config, provider)
    {
        ContainerArgs.Image = "homeassistant/home-assistant:" + (config.Get("homeAssistantImageTag") ?? "latest");
        ContainerArgs.Mounts = new List<ContainerMountArgs>
        {
            new ContainerMountArgs
            {
                Type = "bind",
                Source = basePath + "/hass/config",
                Target = "/config"
            },
            new ContainerMountArgs
            {
                Type = "bind",
                Source = "/etc/localtime",
                Target = "/etc/localtime",
                ReadOnly = true
            },
            new ContainerMountArgs
            {
                Type = "bind",
                Source = "/var/run/docker.sock",
                Target = "/var/run/docker.sock"
            },
            new ContainerMountArgs
            {
                Type = "bind",
                Source = basePath + "/hass/media/snapshots",
                Target = "/media/snapshots"
            }
        };
        ContainerArgs.NetworkMode = "host";
        ContainerArgs.Restart = "unless-stopped";
        ContainerArgs.Privileged = true;
    }
}