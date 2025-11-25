using System;
using System.Collections.Generic;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public class Fing : ContainerConfigBase
{
    public Fing(String name, Pulumi.Config config, Provider provider) : base(name, config, provider)
    {
        var fingDataPath = config.Require("serverBasePath") + "/fingagent";

        ContainerArgs.Image = "fing/fing-agent:" + (config.Get("fingImageTag") ?? "latest");
        ContainerArgs.NetworkMode = "host";
        ContainerArgs.Restart = "always";
        ContainerArgs.Mounts = new List<ContainerMountArgs> {
            new ContainerMountArgs {
                Source = fingDataPath,
                Target = "/app/fingdata",
                Type = "bind"
            }
        };
        ContainerArgs.Capabilities = new ContainerCapabilitiesArgs {
            Adds = new List<string> {
                "CAP_NET_ADMIN"
            }
        };
        EnsureHostPath(fingDataPath);
    }
}
