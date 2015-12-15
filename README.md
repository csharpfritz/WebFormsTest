# WebFormsTest
A unit testing harness and sample project that assist in unit-testing ASP.NET web forms.

[![Build status](https://ci.appveyor.com/api/projects/status/snidwpdkgswcib21?svg=true)](https://ci.appveyor.com/project/csharpfritz/webformstest)

## How to get it

Well, you are here looking at the source.  You COULD download the source code and compile the WebFormsTest project and start using the DLL that it outputs.

OR...  you could install the WebFormsTest harness from NuGet with the following command in your package manager console:

	Install-Package WebFormsTest
	
## Why you need it

The biggest barrier to unit-testing ASP.NET webforms applications is the inability to mock the basic interactions with a Page object.  The ASP.NET pipeline is configured and loaded through the web startup, HttpModules and handlers for the ASPX page object.  Additionally, since we cannot mock the base HttpContext object you have no way to emulate interactions that a user would take with your pages.

## How to use it

The interactions with Page objects are tested in the following way:

1.	Modify your web project's pages that you want to test so that they descend from `WebFormsTest.TestablePage`
1.	Construct a mock `System.Web.HttpContextBase` and set the `Context` property.  The contents of the Context will flow into the `Response`, `Request`, and `Session` properties of the `TestablePage`
1.	Trigger any events that you want to inspect by calling the `TestablePage.FireEvent` method
1.	Inspect the rendered HTML from the page by calling the `RenderHTML` method and inspecting the string of HTML that it returns.

This should get you started with some simple testing of interactions in your Page events.
	 
