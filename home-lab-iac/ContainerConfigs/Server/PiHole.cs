using System;
using System.Collections.Generic;
using Pulumi;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public class PiHole : ContainerConfigBase
{
    public PiHole(String name, Pulumi.Config config, Provider provider) : base(name, config, provider)
    {
        ContainerArgs.Image = "pihole/pihole:" + (config.Get("piHoleImageTag") ?? "latest");
        ContainerArgs.NetworkMode = "bridge";
        ContainerArgs.Restart = "unless-stopped";
        ContainerArgs.Envs = new InputList<string>
        {
            "TZ=America/Chicago",
            Output.Format($"FTLCONF_webserver_api_password={config.RequireSecret("piholeWebPassword").Apply(p => p.ToString())}"),
            "FTLCONF_dns_listeningMode=ALL"
        };
        var piholePath = config.Require("serverBasePath") + "/pihole";
        ContainerArgs.Mounts = new List<ContainerMountArgs> {
            new ContainerMountArgs {
                Source = piholePath,
                Target = "/etc/pihole",
                Type = "bind"
            }
        };
        HostDirectories.Add(piholePath);
        ContainerArgs.Ports = new List<ContainerPortArgs> {
            new ContainerPortArgs {
                Internal = 80,
                External = 8080
            },
            new ContainerPortArgs
            {
                Internal = 443,
                External = 443
            },
            new ContainerPortArgs
            {
                Internal = 53,
                External = 53,
                Protocol = "udp"
            },
        };
        ContainerArgs.Capabilities = new ContainerCapabilitiesArgs
        {
            Adds = new List<string> {
                "CAP_NET_ADMIN",
            }
        };
    }
}
