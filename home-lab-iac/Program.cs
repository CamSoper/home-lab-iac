// Program.cs
using System.Collections.Generic;
using Pulumi;
using Pulumi.Docker;
using home_lab_iac.ContainerConfigs;

return await Deployment.RunAsync(() =>
{
    var config = new Pulumi.Config();

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
        new HomeAssistant("homeassistant", config, server),
        new Govee2Mqtt("govee2mqtt", config, server),
        new MiniDnla("minidlna", config, server),
        new Mqtt("mqtt", config, server),
        new OctoPrint("octoprint", config, server),
        new Omada("omada", config, server),
        new Onstar2Mqtt("onstar2mqtt", config, server),
        new OpenVpn("openvpn", config, server),

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

        var container = new Container(cfg.Name, cfg.ContainerArgs,
            new CustomResourceOptions {
                Provider = cfg.Provider
        });
    }
});