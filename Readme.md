![](docs/mdus-logo.png)

# Markdown User Stories

A simple, local-only web app to create and display user stories, and store them as Markdown files. This is a useful project for solo development, but also an experimental project for me to learn and apply languages and frameworks.

## Simplest Version
*   ASP.NET Core 3
*   One added JQuery library (TBD)
*   Otherwise, default MVC behaviors

This is the simplest implementation that works and has reasonable usability. It's intentionally using the MVC template and built-in functionality, while still tending toward a Clean Architecture.

The project became "self-generating" after just a few commits. That is, I published the app locally, and use it to create the user stories that drive the features.

## Roadmap
Here are the variations I'm planning, once I consider the Simplest Version to be feature complete.

*   Service-oriented (with explicit assemblies)
*   Angular UI
*   React UI
*   Kanban style
*   SignalR
*   OpenID 
*   Desktop app
