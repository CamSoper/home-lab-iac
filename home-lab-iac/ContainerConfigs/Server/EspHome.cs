using System;
using System.Collections.Generic;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public class EspHome : ContainerConfigBase
{
    public EspHome(String name, Pulumi.Config config, Provider provider) : base(name, config, provider)
    {
        ContainerArgs.Image = "esphome/esphome:" + (config.Get("esphomeImageTag") ?? "latest");
        ContainerArgs.NetworkMode = "host";
        ContainerArgs.Restart = "always";
        ContainerArgs.Mounts = new List<ContainerMountArgs>
        {
            new ContainerMountArgs
            {
                Type = "bind",
                Source = config.Require("baseVolumePath") + "/esphome/config",
                Target = "/config"
            },
            new ContainerMountArgs
            {
                Type = "bind",
                Source = "/etc/localtime",
                Target = "/etc/localtime",
                ReadOnly = true
            }
        };
        ContainerArgs.Init = true;
    }
}
