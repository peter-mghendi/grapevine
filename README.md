# grapevine
Asynchronously broadcast gRPC streams over SignalR.

# Introduction
This is a proof-of-concept on broadcasting [gRPC](https://grpc.io) streams asynchronously to other clients via either 
[WebSockets](https://developer.mozilla.org/en-US/docs/Web/API/WebSockets_API), [gRPC-web](https://grpc.io/docs/platforms/web/) 
or [SignalR](https://dotnet.microsoft.com/apps/aspnet/signalr).

# Getting started

## Prerequisites

- [Git](https://git-scm.com) - Version Control System.
- [.NET 5](https://dot.net) - Free, cross-platform, open source developer platform for building all your apps.

## Installation

Please check the [official .NET documentation](https://dotnet.microsoft.com/download) for installation instructions before you start.

### 0. TL;DR Command List

Here's the summary:

    git clone https://github.com/sixpeteunder/grapevine.git
    cd grapevine
    dotnet restore
    dotnet dev-certs https --trust
    cd Grapevine.Edge
    dotnet run
    
**The `dotnet run command will block usage of the terminal for the lifetime of the program. Therefore, the following commmands need to be run in a separate terminal window**

    cd Grapevine.CommandLine
    dotnet run

### 1. Clone the repository

Clone the repsitry using any of these methods.

HTTPS:

    git clone https://github.com/sixpeteunder/grapevine.git
    
SSH:

    git clone git@github.com:sixpeteunder/grapevine.git
    
GitHub CLI:

    gh repo clone sixpeteunder/grapevine

### 2. Switch to the repo folder

    cd grapevine

### 3. Install dependencies

> Warning: These steps may be bandwith-intensive.

Restore .NET dependencies using Nuget:

> This step is automatically performed whenever the code is built or run.

    dotnet restore
    
### 4. Set up project

> macOS doesn't support ASP.NET Core gRPC with TLS. Additional configuration is required to successfully run gRPC services on macOS. 
For more information, see [this document](https://docs.microsoft.com/en-gb/aspnet/core/grpc/troubleshoot?view=aspnetcore-5.0#unable-to-start-aspnet-core-grpc-app-on-macos).

To enable HTTPS during development, you need to [trust the HTTPS development certificate]:

> The following command doesn't work on Linux. See your Linux distribution's documentation for trusting a certificate.

    dotnet dev-certs https --trust

Start the gRPC server:

    cd Grapevine.Edge
    dotnet run
    
In a separate console window, start a client application.

    cd Grapevine.CommandLine
    dotnet run
    
### 5. Develop!

You can now access the gRPC server at [http://localhost:5000](http://localhost:5000) or [https://localhost:5001](https://localhost:5001).
