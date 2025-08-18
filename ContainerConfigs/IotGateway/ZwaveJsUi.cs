using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Pulumi;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public class ZwaveJsUi : ContainerConfigBase
{
    public ZwaveJsUi(String name, Pulumi.Config config, Provider provider) : base(name, config, provider)
    {
        ContainerArgs.Image = "zwavejs/zwave-js-ui:latest";
        ContainerArgs.Restart = "always";
        ContainerArgs.Tty = true;
        ContainerArgs.StopSignal = "SIGINT";
        ContainerArgs.Envs = zwaveEnvs(config);
        ContainerArgs.Ports = new InputList<ContainerPortArgs> {
            new ContainerPortArgs {
                Internal = 8091,
                External = 8091
            },
            new ContainerPortArgs {
                Internal = 3000,
                External = 3000
            }
        };
        ContainerArgs.Volumes = new InputList<ContainerVolumeArgs> {
            new ContainerVolumeArgs {
                ContainerPath = "/usr/src/app/store",
                HostPath = "/home/pi/zwave-js-ui/store"
            }
        };
        ContainerArgs.Devices = new InputList<ContainerDeviceArgs> {
            new ContainerDeviceArgs {
                HostPath = "/dev/serial/by-id/usb-Silicon_Labs_HubZ_Smart_Home_Controller_61201667-if00-port0",
                ContainerPath = "/dev/zwave"
            }
        };
        ContainerArgs.NetworkMode = "bridge";
    }

    private Output<string[]> zwaveEnvs(Pulumi.Config config)
    {
        return Output.All(
            config.RequireSecret("zwaveSessionSecret")
        ).Apply(values => new[]
        {
            $"SESSION_SECRET={values[0]}"
        });
    }
}