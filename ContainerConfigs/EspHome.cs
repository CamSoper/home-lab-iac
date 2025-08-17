using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Pulumi;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public class EspHome : ContainerConfigBase
{
    public EspHome(Pulumi.Config config, Provider provider) : base(config, provider)
    {
        Name = "esphome";
        Image = "esphome/esphome:" + (config.Get("esphomeImageTag") ?? "latest");
        NetworkMode = "host";
        Restart = "always";
        Mounts = new List<ContainerMountArgs>
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
        Init = true;
    }
}
