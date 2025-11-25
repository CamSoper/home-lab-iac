// Program.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Pulumi;
using Pulumi.Command.Remote;
using Pulumi.Docker;
using home_lab_iac.ContainerConfigs;

return await Deployment.RunAsync(() =>
{
    var config = new Pulumi.Config();
    var serverHostValue = config.Require("serverHost");
    var iotGatewayHostValue = config.Require("iotGatewayHost");

    var serverConnection = CreateConnection(serverHostValue);
    var iotGatewayConnection = CreateConnection(iotGatewayHostValue);

    var server = new Provider("server", new ProviderArgs
    {
        Host = "ssh://" + serverHostValue
    });
    var iotGateway = new Provider("iotGateway", new ProviderArgs
    {
        Host = "ssh://" + iotGatewayHostValue
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
        new Fing("fingagent", config, server),
        new PiHole("pihole", config, server),

        // IoT Gateway
        new ZwaveJsUi("zwave-js-ui", config, iotGateway),
        new Zigbee2Mqtt("zigbee2mqtt", config, iotGateway),
    };

    foreach (var cfg in containers)
    {
        var connection = cfg.Provider == server ? serverConnection : iotGatewayConnection;
        var ensureDirectories = cfg.PathsToEnsure
            .Distinct()
            .Select((path, index) => CreateDirectoryCommand($"{cfg.Name}-dir-{index}", path, connection))
            .Cast<Resource>()
            .ToList();

        var image = new RemoteImage(cfg.Name, new()
        {
            Name = cfg.ContainerArgs.Image,
        });

        var container = new Container(cfg.Name, cfg.ContainerArgs,
            new CustomResourceOptions
            {
                Provider = cfg.Provider,
                DependsOn = ensureDirectories
            });
    }
});

static ConnectionArgs CreateConnection(string connectionString)
{
    var parts = connectionString.Split('@', StringSplitOptions.RemoveEmptyEntries);
    return parts.Length == 2
        ? new ConnectionArgs { User = parts[0], Host = parts[1] }
        : new ConnectionArgs { Host = connectionString };
}

static Command CreateDirectoryCommand(string name, string path, ConnectionArgs connection)
{
    return new Command(name, new CommandArgs
    {
        Create = $"mkdir -p \"{path}\"",
        Update = $"mkdir -p \"{path}\"",
        Delete = $"rm -rf \"{path}\"",
        Connection = connection
    });
}
