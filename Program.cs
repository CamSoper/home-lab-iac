using System.Collections.Generic;
using Pulumi;
using Pulumi.Docker;

return await Deployment.RunAsync(() =>
{
    // Add your resources here

    var nodeRed = new RemoteImage("node-red", new()
    {
        Name = ""
    });

    // Export outputs here
    return new Dictionary<string, object?>
    {
        ["outputKey"] = "outputValue"
    };
});
