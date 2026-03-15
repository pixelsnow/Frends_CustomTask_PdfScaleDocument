# Frends Task template

[![BuildMaster](https://github.com/FrendsPlatform/FrendsTaskTemplate/actions/workflows/BuildMaster.yml/badge.svg)](https://github.com/FrendsPlatform/FrendsTaskTemplate/actions/workflows/BuildMaster.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

This template can be used to create new .NET (previously .NET Core) Tasks for [Frends](https://frends.com) integration
platform. This enables you to start writing code without any hassle with project formats etc.

You can learn more about custom Tasks [here](https://docs.frends.com/en/articles/2206746-custom-tasks).

## Prerequisite

You will need the [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet), at
minimum [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) is required.

Frends Tasks are usually written in C#, for the best experience you will want a compatible integrated development
environment (IDE); some common examples are Visual Studio, Visual Studio Code and JetBrains Rider. You can also use any
text editor and the [dotnet](https://learn.microsoft.com/en-us/dotnet/core/tools/) command-line interface.

Some IDEs allow you to install the template to the project wizard but can always
use [dotnet new](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-new) command when that is not possible.

## Install the template

You can install the template using the following command.

`dotnet new install frendstasktemplate --nuget-source https://pkgs.dev.azure.com/frends-platform/frends-tasks/_packaging/main/nuget/v3/index.json`

## Clone repository

You need to clone the repository for the new Task. After cloning, move to that folder.

## Create a new Task

You can create a new Task by running the following command in the Task's repository folder.

`dotnet new frends-task -F Frends.ClassName.MethodName -D "Description of the Task"`

## Get help using the template

`dotnet new frends-task -h`

### Example output:

```
Frends Task (C#)
Author: Frends

Usage:
  dotnet new frends-task [options] [template options]

Options:
  -n, --name <name>       The name for the output being created. If no name is specified, the name of the output directory is used.
  -o, --output <output>   Location to place the generated output.
  --dry-run               Displays a summary of what would happen if the given command line were run if it would result in a template creation.
  --force                 Forces content to be generated even if it would change existing files.
  --no-update-check       Disables checking for the template package updates when instantiating a template.
  --project <project>     The project that should be used for context evaluation.
  -lang, --language <C#>  Specifies the template language to instantiate.
  --type <solution>       Specifies the template type to instantiate.

Template options:
  -D, --Description <Description>    Description of what the Task will do.
                                     Type: string
                                     Default: Description of the Task
  -F, --FullTaskName <FullTaskName>  Full name in format: Company.System.Action e.g. Frends.Xml.Write
                                     Required: *true*
                                     Type: string
                                     Default: Frends.PDF.Scale
```

## Update the template

`dotnet new update`

## Uninstall the template

`dotnet new uninstall frendstasktemplate`

## Developing the template

To develop this template, you can pull the repository.

`git pull https://github.com/FrendsPlatform/FrendsTaskTemplate.git`

To test the changes, you can pack a new nuget from the root folder:

`dotnet pack`

This command will build the project and create a NuGet package in .\bin\Debug

To install this template from a locally created NuGet package use

`dotnet new install <ABSOLUTE_PATH_TO_NUPKG_FILE>`
