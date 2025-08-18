using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Pulumi;
using Pulumi.Docker;
using Pulumi.Docker.Inputs;

namespace home_lab_iac.ContainerConfigs;

public abstract class ContainerConfigBase
{
    public ContainerConfigBase(String name, Pulumi.Config config, Provider provider)
    {
        Name = name;
        Config = config;
        Provider = provider;
        ContainerArgs.Name = name;
    }

    public string Name { get; }
    public Pulumi.Config Config { get; }
    public Provider Provider { get; }
    public ContainerArgs ContainerArgs { get; set; } = new ContainerArgs();
}