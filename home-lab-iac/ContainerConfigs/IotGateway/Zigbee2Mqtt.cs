using System.Collections.Generic;
using Pulumi;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public class Zigbee2Mqtt : ContainerConfigBase
{
    public Zigbee2Mqtt(string name, Pulumi.Config config, Provider provider) : base(name, config, provider)
    {
        ContainerArgs.Image = "koenkk/zigbee2mqtt:" + (config.Get("zigbee2mqttImageTag") ?? "latest");
        var dataPath = config.Require("iotGatewayBasePath") + "/zigbee2mqtt/data";
        ContainerArgs.Mounts = new List<ContainerMountArgs> {
            new ContainerMountArgs {
                Source = dataPath,
                Target = "/app/data",
                Type = "bind"
            },
            new ContainerMountArgs{
                Source = "/run/udev",
                Target = "/run/udev",
                Type = "bind",
                ReadOnly = true
            }
        };
        HostDirectories.Add(dataPath);
        ContainerArgs.Devices = new InputList<ContainerDeviceArgs> {
            new ContainerDeviceArgs {
                HostPath = "/dev/serial/by-id/usb-ITead_Sonoff_Zigbee_3.0_USB_Dongle_Plus_d4bf08e152c9eb1186ae914f1d69213e-if00-port0",
                ContainerPath = "/dev/ttyUSB0",
                Permissions = "rwm"
            }
        };
        ContainerArgs.Restart = "always";
        ContainerArgs.NetworkMode = "bridge";
        ContainerArgs.Privileged = true;
        ContainerArgs.Envs = Output.Create(new[] { "TZ=America/Chicago", "DEBUG=zigbee-herdsman*" });
        ContainerArgs.Ports = new List<ContainerPortArgs> {
            new ContainerPortArgs {
                Internal = 8080,
                External = 8080
            }
        };
    }
}