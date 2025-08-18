# Home Lab IaC (Infrastructure as Code)

This repository contains a Pulumi program written in C# to manage a home lab infrastructure using Docker containers. It provides a structured, code-based approach to deploying and managing various services for a smart home environment.

## Overview

This project uses Pulumi to define and deploy Docker containers for a range of services used in my home lab environment, including:

- Home automation systems (Home Assistant)
- IoT device integrations (Zigbee, Z-Wave, Govee)
- Networking tools (OpenVPN, MQTT)
- Media services (MiniDLNA)
- 3D printing (OctoPrint)
- And more...

The infrastructure is split between two deployment targets:
1. **Server** - Main server hosting most services
2. **IoT Gateway** - Dedicated device for IoT protocol bridges (Zigbee, Z-Wave)

## Project Structure

```
home-lab-iac/
├── Program.cs                    # Main entry point for Pulumi deployment
├── ContainerConfigs/             # Container configuration classes
│   ├── ContainerConfigBase.cs    # Base class for all container configurations
│   ├── IotGateway/               # IoT gateway container configurations
│   │   ├── Zigbee2Mqtt.cs        # Zigbee to MQTT bridge
│   │   └── ZwaveJsUi.cs          # Z-Wave JS UI interface
│   └── Server/                   # Server container configurations
│       ├── EspHome.cs            # ESPHome for custom firmware
│       ├── Govee2Mqtt.cs         # Govee devices integration
│       ├── HomeAssistant.cs      # Home Assistant automation platform
│       ├── MiniDnla.cs           # DLNA media server
│       ├── Mqtt.cs               # MQTT broker
│       ├── NodeRed.cs            # Node-RED automation
│       ├── OctoPrint.cs          # 3D printer management
│       ├── Omada.cs              # TP-Link Omada controller
│       ├── Onstar2Mqtt.cs        # OnStar to MQTT bridge
│       └── OpenVpn.cs            # OpenVPN server
```

## Architecture

The project follows a modular architecture:

1. **Base Configuration Class**: `ContainerConfigBase` provides common properties and functionality for all containers.
2. **Specialized Container Classes**: Each service has its own class that inherits from the base class and specifies service-specific configurations.
3. **Deployment Logic**: The `Program.cs` file contains the main deployment logic that creates and deploys all containers.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Pulumi CLI](https://www.pulumi.com/docs/get-started/install/)
- [Docker](https://docs.docker.com/get-docker/) installed on target machines
- SSH access to target machines

## Configuration

This project uses Pulumi's configuration system to store settings. The following configuration values are required:

```bash
# General configuration
pulumi config set baseVolumePath /path/to/volumes

# Host configuration
pulumi config set serverHost username@server
pulumi config set iotGatewayHost username@iotgateway

# Service-specific image tags
pulumi config set nodeRedImageTag 4.1.0
pulumi config set esphomeImageTag 2025.7
pulumi config set homeAssistantImageTag latest
pulumi config set govee2mqttImageTag latest
pulumi config set minidlnaImageTag latest
pulumi config set mqttImageTag latest
pulumi config set octoprintImageTag latest
pulumi config set omadaImageTag latest
pulumi config set onstar2mqttImageTag latest
pulumi config set openvpnImageTag latest
pulumi config set zigbee2mqttImageTag latest

# Govee configuration
pulumi config set goveeTemperatureScale C
pulumi config set goveeMqttHost mqtt.example.com
pulumi config set goveeMqttPort 1883

# OnStar configuration
pulumi config set onstarDeviceId your-device-id
pulumi config set onstarVin your-vehicle-vin
pulumi config set onstarMqttHost mqtt.example.com
pulumi config set onstarMqttPort 1883
pulumi config set onstarMqttTopic onstar

# Secrets (use --secret flag)
pulumi config set --secret goveeEmail your-email@example.com
pulumi config set --secret goveePassword your-password
pulumi config set --secret goveeApiKey your-api-key
pulumi config set --secret goveeMqttUser mqtt-username
pulumi config set --secret goveeMqttPassword mqtt-password
pulumi config set --secret onstarUsername your-onstar-username
pulumi config set --secret onstarPassword your-onstar-password
pulumi config set --secret onstarPin your-onstar-pin
pulumi config set --secret onstarMqttUser mqtt-username
pulumi config set --secret onstarMqttPassword mqtt-password
pulumi config set --secret zwaveSessionSecret your-session-secret
```

## Usage

### Deployment

To deploy the infrastructure:

```bash
# Initialize a new stack (if needed)
pulumi stack init dev

# Set required configuration values
# (see Configuration section)

# Preview changes
pulumi preview

# Deploy
pulumi up
```

### Adding a New Container

1. Create a new class in the appropriate directory under `ContainerConfigs/`
2. Inherit from `ContainerConfigBase`
3. Implement the constructor to set container-specific properties
4. Add the container to the list in `Program.cs`

Example:

```csharp
public class NewService : ContainerConfigBase
{
    public NewService(string name, Pulumi.Config config, Provider provider) : base(name, config, provider)
    {
        ContainerArgs.Image = "image/name:tag";
        ContainerArgs.Envs = new[] { "ENV_VAR=value" };
        ContainerArgs.NetworkMode = "host";
        ContainerArgs.Restart = "unless-stopped";
        // Add other container-specific configuration
    }
}
```

Then in `Program.cs`:

```csharp
var containers = new List<ContainerConfigBase>
{
    // ... existing containers
    new NewService("newservice", config, server),
};
```

## Security

This project uses Pulumi's secret management to handle sensitive information such as:
- API keys
- Passwords
- Access tokens

Always use `config.RequireSecret()` or `config.GetSecret()` for sensitive values, and ensure you set these values using `pulumi config set --secret`.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Disclaimer

This README was created with the assistance of AI and may contain inaccuracies.
