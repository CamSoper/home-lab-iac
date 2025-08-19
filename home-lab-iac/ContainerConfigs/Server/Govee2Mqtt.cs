using Pulumi;
using Pulumi.Docker;

namespace home_lab_iac.ContainerConfigs;

public class Govee2Mqtt : ContainerConfigBase
{
    public Govee2Mqtt(string name, Pulumi.Config config, Provider provider) : base(name, config, provider)
    {
        ContainerArgs.Image = "ghcr.io/wez/govee2mqtt:" + (config.Get("govee2mqttImageTag") ?? "latest");
        ContainerArgs.Envs = goveeEnvs(config);
        ContainerArgs.NetworkMode = "host";
        ContainerArgs.Restart = "always";
        ContainerArgs.Init = true;
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
        ).Apply(values => new[]{
            $"GOVEE_EMAIL={values[0]}",
            $"GOVEE_PASSWORD={values[1]}",
            $"GOVEE_API_KEY={values[2]}",
            $"GOVEE_MQTT_HOST={values[3]}",
            $"GOVEE_MQTT_PORT={values[4]}",
            $"GOVEE_MQTT_USER={values[5]}",
            $"GOVEE_MQTT_PASSWORD={values[6]}",
            $"GOVEE_TEMPERATURE_SCALE={values[7]}",
            "RUST_LOG_STYLE=always",
            "TZ=America/Chicago"
        });
    }
}
