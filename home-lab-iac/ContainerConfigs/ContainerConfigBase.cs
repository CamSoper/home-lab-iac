using System.Collections.Generic;
using Pulumi.Docker;

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
    public IList<string> PathsToEnsure { get; } = new List<string>();

    protected void EnsureHostPath(string path)
    {
        PathsToEnsure.Add(path);
    }
}