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
            // Server
            new NodeRed("nodered", config, server),
            new EspHome("esphome", config, server),
            new HomeAssistant("homeassistant", config, server, basePath),
            new Govee2Mqtt("govee2mqtt", config, server),
            new MiniDnla("minidlna", config, server),
            new Mqtt("mqtt", config, server, basePath),
            new OctoPrint("octoprint", config, server, basePath),
            new Omada("omada", config, server, basePath),
            new Onstar2Mqtt("onstar2mqtt", config, server, basePath),
            new OpenVpn("openvpn", config, server, basePath),

            // IoT Gateway
            new ZwaveJsUi("zwave-js-ui", config, iotGateway),
            new Zigbee2Mqtt("zigbee2mqtt", config, iotGateway),
        };

        foreach (var cfg in containers)
        {
            var image = new RemoteImage(cfg.Name, new()
            {
                Name = cfg.ContainerArgs.Image,
            });

            var container = new Container(cfg.Name, cfg.ContainerArgs, new CustomResourceOptions
            {
                Provider = cfg.Provider
            });
        }
    }
}

