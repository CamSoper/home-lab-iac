using System;
using System.Collections.Generic;
using Pulumi;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public class Omada : ContainerConfigBase
{
    public Omada(String name, Pulumi.Config config, Provider provider, string basePath) : base(name, config, provider)
    {
        ContainerArgs.Image = "mbentley/omada-controller:" + (config.Get("omadaImageTag") ?? "latest");
        ContainerArgs.Restart = "unless-stopped";
        ContainerArgs.NetworkMode = "host";
        ContainerArgs.Envs = Output.Create(new[] {
            "PUID=508",
            "PGID=508",
            "MANAGE_HTTP_PORT=8088",
            "MANAGE_HTTPS_PORT=8043",
            "PORTAL_HTTP_PORT=8088",
            "PORTAL_HTTPS_PORT=8843",
            "PORT_APP_DISCOVERY=27001",
            "PORT_ADOPT_V1=29812",
            "PORT_UPGRADE_V1=29813",
            "PORT_MANAGER_V1=29811",
            "PORT_MANAGER_V2=29814",
            "PORT_DISCOVERY=29810",
            "SHOW_SERVER_LOGS=true",
            "SHOW_MONGODB_LOGS=false",
            "SSL_CERT_NAME=tls.crt",
            "SSL_KEY_NAME=tls.key",
            "TZ=Etc/UTC"
        });
        ContainerArgs.DestroyGraceSeconds = 60;
        ContainerArgs.Mounts = new List<ContainerMountArgs>
        {
            new ContainerMountArgs
            {
                Source = basePath + "/omada/data",
                Target = "/opt/tplink/EAPController/data",
                Type = "bind"
            },
            new ContainerMountArgs
            {
                Source = basePath + "/omada/logs",
                Target = "/opt/tplink/EAPController/logs",
                Type = "bind"
            },
            new ContainerMountArgs
            {
                Source = basePath + "/omada/work",
                Target = "/opt/tplink/EAPController/work",
                Type = "bind"
            }
        };
        ContainerArgs.Init = true;
    }
}