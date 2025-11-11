using System;
using System.Collections.Generic;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public class Fing : ContainerConfigBase
{
    public Fing(String name, Pulumi.Config config, Provider provider) : base(name, config, provider)
    {
        ContainerArgs.Image = "fing/fing-agent";
        ContainerArgs.NetworkMode = "host";
        ContainerArgs.Restart = "always";
        ContainerArgs.Mounts = new List<ContainerMountArgs> {
            new ContainerMountArgs {
                Source = config.Require("serverBasePath") + "/fingagent",
                Target = "/app/fingdata",
                Type = "bind"
            }
        };
        ContainerArgs.Ports = new List<ContainerPortArgs> {
            new ContainerPortArgs {
            Internal = 44444,
            External = 44444
            }
        };
        ContainerArgs.Capabilities = new ContainerCapabilitiesArgs {
            Adds = new List<string> {
                "NET_ADMIN"
            }
        };
    }
}
