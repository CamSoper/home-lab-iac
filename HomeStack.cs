// Program.cs
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Pulumi;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

class ContainerConfig
{
    public string Name { get; set; } = null!;
    public string Image { get; set; } = null!;
    //public string[]? Envs { get; set; }
    public Pulumi.Output<string[]> Envs { get; set; } = Output.Create(new string[0]);
    public string? NetworkMode { get; set; }
    public string Restart { get; set; } = "unless-stopped";
    public List<ContainerMountArgs>? Mounts { get; set; }
    public InputList<ContainerPortArgs>? Ports { get; set; }
    public bool Privileged { get; set; } = false;
    public bool Init { get; set; } = false;
    public Provider Provider = null!;
}

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

        var containers = new List<ContainerConfig>
        {
            new ContainerConfig
            {
                // Node Red
                Name = "nodered",
                Image = "nodered/node-red:" + (config.Get("nodeRedImageTag") ?? "latest"),
                Envs = Output.Create(new[] { "TZ=America/Chicago" }),
                NetworkMode = "host",
                Mounts = new List<ContainerMountArgs>
                {
                    new ContainerMountArgs
                    {
                    Type   = "bind",
                    Source = basePath + "/nodered/data",
                    Target = "/data"
                    }
                },
                Init = true,
                Provider = server
            },
            new ContainerConfig
            {
                // ESPHome
                Name = "esphome",
                Image = "esphome/esphome:" + (config.Get("esphomeImageTag") ?? "latest"),
                NetworkMode = "host",
                Restart = "always",
                Mounts = new List<ContainerMountArgs>
                {
                    new ContainerMountArgs
                    {
                        Type = "bind",
                        Source = basePath + "/esphome/config",
                        Target = "/config"
                    },
                    new ContainerMountArgs
                    {
                        Type = "bind",
                        Source = "/etc/localtime",
                        Target = "/etc/localtime",
                        ReadOnly = true
                    }
                },
                Init = true,
                Provider = server
            },
            new ContainerConfig
            {
                // govee2mqtt
                Name = "govee2mqtt",
                Image = "ghcr.io/wez/govee2mqtt:" + (config.Get("govee2mqttImageTag") ?? "latest"),
                Envs = goveeEnvs(config),
                NetworkMode = "host",
                Restart = "always",
                Init = true,
                Provider = server
            },
            new ContainerConfig
            {
                Name = "homeassistant",
                Image = "homeassistant/home-assistant:" + (config.Get("homeAssistantImageTag") ?? "latest"),
                Mounts = new List<ContainerMountArgs>
                {
                    new ContainerMountArgs
                    {
                        Type = "bind",
                        Source = basePath + "/hass/config",
                        Target = "/config"
                    },
                    new ContainerMountArgs
                    {
                        Type = "bind",
                        Source = "/etc/localtime",
                        Target = "/etc/localtime",
                        ReadOnly = true
                    },
                    new ContainerMountArgs
                    {
                        Type = "bind",
                        Source = "/var/run/docker.sock",
                        Target = "/var/run/docker.sock"
                    },
                    new ContainerMountArgs
                    {
                        Type = "bind",
                        Source = basePath + "/hass/media/snapshots",
                        Target = "/media/snapshots"
                    }
                },
                NetworkMode = "host",
                Restart = "unless-stopped",
                Privileged = true,
                Provider = server
            }

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
                Privileged = cfg.Privileged
            }, new CustomResourceOptions
            {
                Provider = cfg.Provider
            });
        }
    }
    private Output<string[]> goveeEnvs(Pulumi.Config config)
    {
        return Output.All(
            config.RequireSecret("goveeEmail"),
            config.RequireSecret("goveePassword"),
            config.RequireSecret("goveeApiKey"),
            Output.Create(config.Require("goveeMqttHost")),
            Output.Create(config.Require("goveeMqttPort")),
            config.RequireSecret("goveeMqttUser"),
            config.RequireSecret("goveeMqttPassword"),
            Output.Create(config.Require("goveeTemperatureScale"))
        ).Apply(values => new[]
        {
            "GOVEE_EMAIL=" + values[0],
            "GOVEE_PASSWORD=" + values[1],
            "GOVEE_API_KEY=" + values[2],
            "GOVEE_MQTT_HOST=" + values[3],
            "GOVEE_MQTT_PORT=" + values[4],
            "GOVEE_MQTT_USER=" + values[5],
            "GOVEE_MQTT_PASSWORD=" + values[6],
            "GOVEE_TEMPERATURE_SCALE=" + values[7],
            "RUST_LOG_STYLE=always",
            "TZ=America/Chicago"
        });
    }
}

