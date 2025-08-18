using System.Collections.Generic;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public class OpenVpn : ContainerConfigBase
{
    public OpenVpn(string name, Pulumi.Config config, Provider provider, string basePath) : base(name, config, provider)
    {
        ContainerArgs.Image = "openvpn/openvpn-as:" + (config.Get("openVpnImageTag") ?? "latest");
        ContainerArgs.Capabilities = new ContainerCapabilitiesArgs
        {
            Adds = new[] { "NET_ADMIN" }
        };
        ContainerArgs.Ports = new List<ContainerPortArgs>
        {
            new ContainerPortArgs
            {
                External = 943,
                Internal = 943
            },
            new ContainerPortArgs
            {
                External = 1194,
                Internal = 1194,
                Protocol = "udp"
            }
        };
        ContainerArgs.Restart = "always";
        ContainerArgs.Volumes = new List<ContainerVolumeArgs>
        {
            new ContainerVolumeArgs
            {
                HostPath = basePath + "/openvpn/openvpn",
                ContainerPath = "/openvpn"
            }
        };
    }
}