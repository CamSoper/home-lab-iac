using Pulumi;
using Pulumi.Docker;

namespace home_lab_iac.ContainerConfigs;

public class Onstar2Mqtt : ContainerConfigBase
{
    public Onstar2Mqtt(string name, Pulumi.Config config, Provider provider, string basePath) : base(name, config, provider)
    {
        ContainerArgs.Image = "ghcr.io/bigthundersr/onstar2mqtt:" + (config.Get("onstar2MqttImageTag") ?? "latest");
        ContainerArgs.Restart = "unless-stopped";
        ContainerArgs.Envs = onstar2MqttEnvs(config);
    }

    private Output<string[]> onstar2MqttEnvs(Pulumi.Config config)
    {
        return Output.All(
            Output.Create(config.Require("onstarDeviceId")),
            Output.Create(config.Require("onstarVin")),
            config.RequireSecret("onstarUser"),
            config.RequireSecret("onstarPassword"),
            config.RequireSecret("onstarTotp"),
            config.RequireSecret("onstarPin"),
            Output.Create(config.Require("onstarMqttHost")),
            config.RequireSecret("onstarMqttUser"),
            config.RequireSecret("onstarMqttPassword")
        ).Apply(values => new[]
        {
            $"ONSTAR_DEVICEID={values[0]}",
            $"ONSTAR_VIN={values[1]}",
            $"ONSTAR_USERNAME={values[2]}",
            $"ONSTAR_PASSWORD={values[3]}",
            $"ONSTAR_TOTP={values[4]}",
            $"ONSTAR_PIN={values[5]}",
            $"MQTT_HOST={values[6]}",
            $"MQTT_USERNAME={values[7]}",
            $"MQTT_PASSWORD={values[8]}"
        });
    }
}