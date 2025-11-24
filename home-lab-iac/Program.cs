// Program.cs
using System.Collections.Generic;
using System.Linq;
using Pulumi;
using Pulumi.Command.Remote;
using Pulumi.Docker;
using home_lab_iac.ContainerConfigs;
using Pulumi.Command.Remote.Inputs;
using System;

return await Deployment.RunAsync(() =>
{
    var config = new Pulumi.Config();

    var serverHost = config.Require("serverHost");
    var iotGatewayHost = config.Require("iotGatewayHost");
    var sshUser = config.Require("sshUser");
    var sshPrivateKey = config.RequireSecret("sshPrivateKey");

    var server = new Provider("server", new ProviderArgs
    {
        Host = "ssh://" + serverHost
    });
    var iotGateway = new Provider("iotGateway", new ProviderArgs
    {
        Host = "ssh://" + iotGatewayHost
    });

    var serverConnection = new ConnectionArgs
    {
        Host = serverHost,
        User = sshUser,
        PrivateKey = sshPrivateKey
    };

    var iotGatewayConnection = new ConnectionArgs
    {
        Host = iotGatewayHost,
        User = sshUser,
        PrivateKey = sshPrivateKey
    };

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
        new Fing("fingagent", config, server),
        new PiHole("pihole", config, server),

        // IoT Gateway
        new ZwaveJsUi("zwave-js-ui", config, iotGateway),
        new Zigbee2Mqtt("zigbee2mqtt", config, iotGateway),
    };

    var serverDirectories = containers
        .Where(c => c.Provider == server)
        .SelectMany(c => c.HostDirectories)
        .Where(path => !string.IsNullOrWhiteSpace(path))
        .OrderBy(path => path)
        .Distinct()
        .ToList();

    var iotGatewayDirectories = containers
        .Where(c => c.Provider == iotGateway)
        .SelectMany(c => c.HostDirectories)
        .Where(path => !string.IsNullOrWhiteSpace(path))
        .OrderBy(path => path)
        .Distinct()
        .ToList();

    var serverDirectoryResources = serverDirectories
        .Select((path, index) => CreateDirectoryCommand($"server-dir-{index}", path, serverConnection))
        .Cast<Resource>()
        .ToList();

    var iotGatewayDirectoryResources = iotGatewayDirectories
        .Select((path, index) => CreateDirectoryCommand($"iot-dir-{index}", path, iotGatewayConnection))
        .Cast<Resource>()
        .ToList();

    foreach (var cfg in containers)
    {
        _ = new RemoteImage(cfg.Name, new()
        {
            Name = cfg.ContainerArgs.Image,
        });

        var dependsOn = cfg.Provider == server ? serverDirectoryResources : iotGatewayDirectoryResources;

        _ = new Container(cfg.Name, cfg.ContainerArgs,
            new CustomResourceOptions
            {
                Provider = cfg.Provider,
                DependsOn = dependsOn
            });
    }
});

static Command CreateDirectoryCommand(string name, string path, ConnectionArgs connection)
{
    return new Command(name, new CommandArgs
    {
        Connection = connection,
        Create = Output.Format($"mkdir -p \"{path}\""),
        Update = Output.Format($"mkdir -p \"{path}\""),
    });
}