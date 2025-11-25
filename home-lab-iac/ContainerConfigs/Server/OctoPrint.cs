using System;
using System.Collections.Generic;
using Pulumi;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public class OctoPrint : ContainerConfigBase
{
    public OctoPrint(String name, Pulumi.Config config, Provider provider) : base(name, config, provider)
    {
        var octoPrintPath = config.Require("serverBasePath") + "/octoprint";

        ContainerArgs.Image = "octoprint/octoprint:" + (config.Get("octoprintImageTag") ?? "latest");
        ContainerArgs.Mounts = new List<ContainerMountArgs> {
            new ContainerMountArgs {
                Type = "bind",
                Source = octoPrintPath,
                Target = "/octoprint",
            }
        };
        ContainerArgs.Ports = new List<ContainerPortArgs>
        {
            new ContainerPortArgs {
                External = 80,
                Internal = 80
            }
        };
        ContainerArgs.Devices = new InputList<ContainerDeviceArgs> {
            new ContainerDeviceArgs {
                HostPath = "/dev/ttyACM0",
                ContainerPath = "/dev/ttyACM0",
                Permissions = "rwm"
            },
            new ContainerDeviceArgs {
                HostPath = "/dev/video0",
                ContainerPath = "/dev/video0",
                Permissions = "rwm"
            }
        };
        ContainerArgs.Restart = "unless-stopped";
        ContainerArgs.NetworkMode = "bridge";
        EnsureHostPath(octoPrintPath);
    }
}
