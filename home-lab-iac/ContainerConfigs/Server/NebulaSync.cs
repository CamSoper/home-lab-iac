using System;
using System.Collections.Generic;
using System.Xml;
using Pulumi;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public class NebulaSync : ContainerConfigBase
{
    public NebulaSync(String name, Pulumi.Config config, Provider provider) : base(name, config, provider)
    {
        ContainerArgs.Image = "ghcr.io/lovelaze/nebula-sync:" + (config.Get("nebulaSyncImageTag") ?? "latest");
        ContainerArgs.NetworkMode = "bridge";
        ContainerArgs.Restart = "always";
        ContainerArgs.Envs = new InputList<string>
        {
            //Output.Format($"FTLCONF_webserver_api_password={config.RequireSecret("piholeWebPassword").Apply(p => p.ToString())}"),
            Output.Format($"PRIMARY={config.Require("nebulaSyncPrimary")}|{config.RequireSecret("piholeWebPassword").Apply(p => p.ToString())}"),
            Output.Format($"REPLICAS={config.Require("nebulaSyncReplica")}|{config.RequireSecret("piholeWebPassword").Apply(p => p.ToString())}"),
            "FULL_SYNC=true",
            "RUN_GRAVITY=false",
            $"CRON={config.Require("nebulaSyncCron")}"
        };
    }
}
