# WebFormsTest
A unit testing harness and sample project that assist in unit-testing ASP.NET web forms.

[![Build status](https://ci.appveyor.com/api/projects/status/snidwpdkgswcib21/branch/master?svg=true)](https://ci.appveyor.com/project/csharpfritz/webformstest)
[![NuGet Downloads](https://img.shields.io/nuget/dt/WebFormsTest.svg)](https://www.nuget.org/packages/WebFormsTest)
![NuGet Version](https://img.shields.io/nuget/v/WebFormsTest.svg)

## How to get it

Well, you are here looking at the source.  You COULD download the source code and compile the WebFormsTest project and start using the DLL that it outputs.

OR...  you could install the WebFormsTest harness from NuGet with the following command in your package manager console:

	Install-Package WebFormsTest
	
## Why you need it

The biggest barrier to unit-testing ASP.NET webforms applications is the inability to mock the basic interactions with a Page object.  The ASP.NET pipeline is configured and loaded through the web startup, HttpModules and handlers for the ASPX page object.  Additionally, since we cannot mock the base HttpContext object you have no way to emulate interactions that a user would take with your pages.

WebFormsTest attempts to mimic a 'real webserver' by providing a harness that acts like a webserver and provides all of the features to the ASP.NET framework that are expected.  In this way, you are able to test your code with the real ASP.NET framework objects: Context, Session, Application, Request, Response.  There is no faking, no stubbing, no inheriting and working around sealed classes.  You can work directly with your code-behind classes and even the contents of your ASPX / ASCX pages.  Controls and their behavior are completed supported, and you can even inspect the rendered output of pages in the application. 

All code written in the project uses reflection and considers the publicly accessible source code to System.Web on referencesource.microsoft.com

## How to use it

The start building tests, you need to consider that you are 'configuring a web server' and need to tell it some things about your application.  With this harness, we use a WebApplicationProxy object that will proxy "requests for features" to the application and deliver those results to you.  Configure one WebApplicationProxy per suite of tests, and being requesting pages through it and enjoy testing your code without needing a server or a running service.

Take a look at our GitHub Wiki to learn more abourt building your first test and some of the more complex interactions that you can take.  We also list the operations that are currently supported.  Your contributions and feedback are very welcome as we build more features and support more test capabilities with this harness.
