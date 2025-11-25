using System;
using Pulumi;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public class ZwaveJsUi : ContainerConfigBase
{
    public ZwaveJsUi(String name, Pulumi.Config config, Provider provider) : base(name, config, provider)
    {
        var storePath = config.Require("iotGatewayBasePath") + "/zwave-js-ui/store";

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
                HostPath = storePath,
                ContainerPath = "/usr/src/app/store"
            }
        };
        ContainerArgs.Devices = new InputList<ContainerDeviceArgs> {
            new ContainerDeviceArgs {
                HostPath = "/dev/serial/by-id/usb-Silicon_Labs_HubZ_Smart_Home_Controller_61201667-if00-port0",
                ContainerPath = "/dev/zwave",
                Permissions = "rwm"
            }
        };
        ContainerArgs.NetworkMode = "bridge";
        EnsureHostPath(storePath);
    }

    private Output<string[]> zwaveEnvs(Pulumi.Config config)
    {
        return Output.All(
            config.RequireSecret("zwaveSessionSecret")
        ).Apply(values => new[] {
            $"SESSION_SECRET={values[0]}"
        });
    }
}