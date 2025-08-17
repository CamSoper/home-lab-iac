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
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Restart { get; set; } = string.Empty;
    public Output<string[]> Envs { get; set; } = Output.Create(Array.Empty<string>());
    public string NetworkMode { get; set; } = string.Empty;
    public bool Init { get; set; } = false;
    public InputList<ContainerPortArgs> Ports { get; set; } = new InputList<ContainerPortArgs>();
    public InputList<ContainerMountArgs> Mounts { get; set; } = new InputList<ContainerMountArgs>();
    public bool Tty { get; set; } = false;
    public string StopSignal { get; set; } = string.Empty;
    public InputList<ContainerVolumeArgs> Volumes { get; set; } = new InputList<ContainerVolumeArgs>();
    public InputList<ContainerDeviceArgs> Devices { get; set; } = new InputList<ContainerDeviceArgs>();
    public InputList<ContainerNetworksAdvancedArgs> Networks { get; set; } = new InputList<ContainerNetworksAdvancedArgs>();
    public bool Privileged { get; set; } = false;
    public string User { get; set; } = string.Empty;

    public ContainerConfigBase(Pulumi.Config config, Provider provider)
    {
        Config = config;
        Provider = provider;
    }

    public Pulumi.Config Config { get; }
    public Provider Provider { get; }
}