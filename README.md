# Bing Summariser Plugin

- [Bing Summariser Plugin](#bing-summariser-plugin)
  - [Overview](#overview)
  - [Getting Started](#getting-started)
    - [Prerequisites](#prerequisites)
    - [Configuration](#configuration)
    - [Using local.settings.json](#using-localsettingsjson)
  - [Running the plugin](#running-the-plugin)
  - [Testing the plugin](#testing-the-plugin)
  - [Adding the plugin to the Chat Copilot application](#adding-the-plugin-to-the-chat-copilot-application)
  - [Verify the plugin is working end to end](#verify-the-plugin-is-working-end-to-end)

## Overview

**Plugin sample built using Semantic Kernel that summarises a given text from Bing Web Search. Tested with Semantic Kernel beta8!**

This sample plugin uses the [Bing Web Search API](https://docs.microsoft.com/en-us/azure/cognitive-services/bing-web-search/) to search for a given text and summarise the results. It is adapted from the [Bing Web Search sample plugin]() included in the Chat Copilot sample repository. It accomplishes this by using native and semantic functions to perform the following tasks:

1. Search for a given text using the Bing Web Search API.
2. Extract the top 'X' results from the search.
3. Summarise the results and present back to the end user.

![GIF](./azure-function/Assets/pluginprocess.gif)

## Getting Started

### Prerequisites

- [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- [.NET 6](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Azure Functions Core Tools](https://www.npmjs.com/package/azure-functions-core-tools)
- Install the recommended extensions
  - [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
  - [Semantic Kernel Tools](https://marketplace.visualstudio.com/items?itemName=ms-semantic-kernel.semantic-kernel)

### Configuration

1. Open `appsettings.json` and configure the `kernel` and `aiPlugin` objects.

Configure an OpenAI endpoint
- Copy [settings.json.openai-example](./config/appsettings.json.openai-example) to `./appsettings.json`
- Edit the `kernel` object to add your OpenAI endpoint configuration
- Edit the `aiPlugin` object to define the properties that get exposed in the ai-plugin.json file

'OR'

Configure an Azure OpenAI endpoint
- Copy [settings.json.azure-example](./config/appsettings.json.azure-example) to `./appsettings.json`
- Edit the `kernel` object to add your Azure OpenAI endpoint configuration
- Edit the `aiPlugin` object to define the properties that get exposed in the ai-plugin.json file

### Using local.settings.json

1. Copy [local.settings.json.example](./azure-function/local.settings.json.example) to `./azure-function/local.settings.json`
1. Edit the `Values` object to add your OpenAI endpoint configuration in the `apiKey` and `BingApiKey` properties

## Running the plugin

To run the Azure Functions application just hit `F5` from Visual Studio or Visual Studio Code.

To build and run the Azure Functions application from a terminal use the following commands:

```bash
cd azure-function
dotnet build
cd bin/Debug/net6.0
func host start  
```

## Testing the plugin
Swagger is automatically included in the Azure Functions application. To access the Swagger UI, navigate to `http://localhost:7071/swagger` in your browser.  
- Select the `WebSearch` function and click `Try it out` 
- Enter a search term and click `Execute` The results will be displayed in the `Response body` section
- Copy and paste the results and run it against the `BingSummariser` function and click `Try it out`
- The results will be displayed in the `Response body` section

## Adding the plugin to the Chat Copilot application
Run the [Chat Copilot](https://github.com/microsoft/chat-copilot) application. Click the `Plugins` link (top right corner) and enter the following details:
- Select `Custom Plugin`
- Select `Add`
- Enter your website domain: `http://localhost:7071/BingSummariser` and select `Find manifest file`
- In the Verify Plugin prompt, select `Add Plugin`
- The plugin will now be available in the `Plugins` list
- Enable the plugin by selecting the `Enable` button

## Verify the plugin is working end to end
Once the plugin is added and enabled: 
- Ask the following prompt to the chat window `What is the latest sports news?`
- The planner should show two steps, one for the `Search` step to search Bing and two `TextSummarize` step to summarize the results
- Select `Yes` to run the planner
- The results should be displayed in the chat window