using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Pulumi;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public class OpenVpn : ContainerConfigBase
{
    public OpenVpn(string name, Pulumi.Config config, Provider provider, string basePath) : base(name, config, provider)
    {
        /*
                version: '2'
        services:
        openvpn:
            cap_add:
            - NET_ADMIN
            image: openvpn/openvpn-as
            container_name: openvpn
            ports:
            - "943:943"
            - "1194:1194/udp"
            restart: always
            volumes:
            - ./openvpn:/openvpn
            */

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