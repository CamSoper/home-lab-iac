using System;
using System.Collections.Generic;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public class Mqtt : ContainerConfigBase
{
    public Mqtt(String name, Pulumi.Config config, Provider provider) : base(name, config, provider)
    {
        ContainerArgs.Image = "eclipse-mosquitto:" + (config.Get("mqttImageTag") ?? "latest");
        ContainerArgs.Mounts = new List<ContainerMountArgs> {
            new ContainerMountArgs {
                Source = config.Require("serverBasePath") + "/mqtt/mosquitto",
                Target = "/mosquitto",
                Type = "bind"
            }
        };
        ContainerArgs.Restart = "unless-stopped";
        ContainerArgs.NetworkMode = "host";
    }
}
