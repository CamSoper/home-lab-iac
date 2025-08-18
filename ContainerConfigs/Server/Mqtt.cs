using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Pulumi;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public class Mqtt : ContainerConfigBase
{
    public Mqtt(String name, Pulumi.Config config, Provider provider, string basePath) : base(name, config, provider)
    {
        ContainerArgs.Image = "eclipse-mosquitto:" + (config.Get("mqttImageTag") ?? "latest");
        ContainerArgs.Mounts = new List<ContainerMountArgs>
        {
            new ContainerMountArgs
            {
                Source = basePath + "/mqtt/mosquitto",
                Target = "/mosquitto",
                Type = "bind"
            }
        };
        ContainerArgs.Restart = "unless-stopped";
        ContainerArgs.NetworkMode = "host";
    }
}
