# Basic Schema Comparison Example

Prints to the stdout (terminal) the basic column differences in tables between 2 dacpacs.

## Setup

The example uses the .NET 6 SDK. You can download it from [here](https://dotnet.microsoft.com/download/dotnet/6.0).

```
dotnet restore
dotnet build
```

## Examples


### Example: Compare 2 dacpacs from dotnet run

```bash
dotnet run "../../../AdventureWorksLT-original.dacpac" "../../../AdventureWorksLT-updated.dacpac"
```

### Example: Compare 2 dacpacs from executable

```bash
cd bin/Debug/net6.0
./schema-compare "../../../AdventureWorksLT-original.dacpac" "../../../AdventureWorksLT-updated.dacpac"
```

