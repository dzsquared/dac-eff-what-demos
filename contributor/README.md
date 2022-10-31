# Sample Deployment Contributor
Removes steps from the deployment plan to avoid impacting objects based on their name.


## Setup
The example uses the .NET 6 SDK. You can download it from [here](https://dotnet.microsoft.com/download/dotnet/6.0).

```
dotnet restore
dotnet build
```

After build, place the `Azure.Samples.PlanFilterer.dll` from bin/Debug/net6.0 in the SqlPackage folder or use the `/p:AdditionalDeploymentContributorPaths` property to specify the location of the DLL when running SqlPackage with the contributor.

## Usage Examples

### Example: Does not publish objects with the name "audits"

```bash
./sqlpackage /a:Publish /sf:"../UserAccounts.dacpac" /tcs:"" /p:AdditionalDeploymentContributors=Azure.Samples.PlanFilterer /p:AdditionalDeploymentContributorArguments="ObjectName=audits"
```

### Example: Does not publish objects with the names "audits" or "users"

```bash
./sqlpackage /a:Publish /sf:"../UserAccounts.dacpac" /tcs:"" /p:AdditionalDeploymentContributors=Azure.Samples.PlanFilterer /p:AdditionalDeploymentContributorArguments="ObjectName1=audits;ObjectName2=users"
```