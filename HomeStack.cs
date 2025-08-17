// Program.cs
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Pulumi;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;
using home_lab_iac.ContainerConfigs;

class HomeStack : Stack
{
    public HomeStack()
    {
        var config = new Pulumi.Config();
        var basePath = config.Require("baseVolumePath");

        var server = new Provider("server", new ProviderArgs
        {
            Host = "ssh://" + config.Require("serverHost")
        });
        var iotGateway = new Provider("iotGateway", new ProviderArgs
        {
            Host = "ssh://" + config.Require("iotGatewayHost")
        });

        var containers = new List<ContainerConfigBase>
        {
            new NodeRed(config, server),
            new EspHome(config, server),
            new HomeAssistant(config, server, basePath),
            new Govee2Mqtt(config, server),
            new MiniDnla(config, server),
            new Mqtt(config, server, basePath),
            new OctoPrint(config, server, basePath),
            new ZwaveJsUi(config, iotGateway)
        };

        foreach (var cfg in containers)
        {
            var image = new RemoteImage(cfg.Name, new()
            {
                Name = cfg.Image,
            });

            var container = new Container(cfg.Name, new ContainerArgs
            {
                Name = cfg.Name,
                Image = cfg.Image,
                Restart = cfg.Restart,
                NetworkMode = cfg.NetworkMode ?? string.Empty,
                Envs = cfg.Envs ?? Output.Create(new string[0]),
                Mounts = cfg.Mounts ?? new List<ContainerMountArgs>(),
                Ports = cfg.Ports ?? new InputList<ContainerPortArgs>(),
                Init = cfg.Init,
                Privileged = cfg.Privileged,
                Devices = cfg.Devices ?? new InputList<ContainerDeviceArgs>(),
                NetworksAdvanced = cfg.Networks ?? new InputList<ContainerNetworksAdvancedArgs>(),
                Tty = cfg.Tty,
                StopSignal = cfg.StopSignal,
                Volumes = cfg.Volumes ?? new InputList<ContainerVolumeArgs>(),
                User = cfg.User
            },
            new CustomResourceOptions
            {
                Provider = cfg.Provider
            });
        }
    }
}

