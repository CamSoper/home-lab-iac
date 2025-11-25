using System;
using System.Collections.Generic;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public class Mqtt : ContainerConfigBase
{
    public Mqtt(String name, Pulumi.Config config, Provider provider) : base(name, config, provider)
    {
        var mosquittoPath = config.Require("serverBasePath") + "/mqtt/mosquitto";

        ContainerArgs.Image = "eclipse-mosquitto:" + (config.Get("mqttImageTag") ?? "latest");
        ContainerArgs.Mounts = new List<ContainerMountArgs> {
            new ContainerMountArgs {
                Source = mosquittoPath,
                Target = "/mosquitto",
                Type = "bind"
            }
        };
        ContainerArgs.Restart = "unless-stopped";
        ContainerArgs.NetworkMode = "host";
        EnsureHostPath(mosquittoPath);
    }
}
