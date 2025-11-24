using Pulumi.Docker;
using System.Collections.Generic;

namespace home_lab_iac.ContainerConfigs;

public abstract class ContainerConfigBase
{
    public ContainerConfigBase(string name, Pulumi.Config config, Provider provider)
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
    public List<string> HostDirectories { get; } = new();
}