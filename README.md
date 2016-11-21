# Managed.AnyCPU
Simple project demonstrating how to reference managed C++ functionality from a C# AnyCPU dll

I believe the cleanest way to reference managed C++ functionality from a C# dll, compiled with the AnyCPU setting, is to create a C# front end to the C++ functionality. 

In order for the C++ functionality to be consumed by a C# dll, the C++ project must produce both x86 and x64 versions of the dll. It is impossible to reference just a x86 or  a x64 dll from a C# dll compiled with the AnyCPU setting.

## Building the projects
1. Build->Batch Build
2. Check every checkbox under the Build title
3. Press the Build button

## Test projects
1. Test->Windows->Test Explorer
2. Test->Test Settings->Default Processor Architecture->x86
3. "Run All" in test explorer
4. Test->Test Settings->Default Processor Architecture->x64
5. "Run All" in test explorer

# Project Descriptions
## AnyCPU.Interop
This is a managed C++ project producing both x86 and x64 dlls. It contains a single class with a method returning a discriptor identifying the CPU architecture the process is running on.

## AnyCPU
The purpose of this dll is to create a clean bridge between the managed C++ dll and client C# dlls compiled with the AnyCPU setting. This dll is primarilly responsible for creating a unified view of the C++ functionality from a C# client perspective. The managed C++ dll's are compiled as embedded resources in this dll and the assembly is dynamically loaded at runtime. 

**The nuget package InjectModuleInitializer is essential to create a solution where C++ can easily be access in C# from a single dll.**
The InjectModuleIntitializer enables the Dll to hook up events allowing the dll itself to resolve loading assemblies it is dependent upon.

*Note: the project copies dlls from the AnyCPU.Interop project using pre build events.*

## AnyCPU.Develop.Test
This project references the AnyCPU project. It is intended to demonstrate the AnyCPU dll can reference either the x86 or x64 dll, but it cannot reference them both at the same time. Additionally, this project is intended to demonstrate how one might debug the managed C++ dll from C#.

*Note: The AnyCPU project references the x64 dll when it is built.*

When the unit test is run in x64 mode it passes and when run in x86 mode it fails.

## AnyCPU.Distribute.Test
This project references the AnyCPU project by copying the AnyCPU.dll alongside the AnyCPU.Distribute.Test.dll. The project references the dll directly so the AnyCPU.Interop.dll is not visible to the AppDomain assembly loader. When the unit tests are run in either x86 mode or x64 mode they pass. Demonstrating that the AnyCPU.dll is able to intercept the failed assembly load request and load the correct AnyCPP.Interop.dll at runtime.

*Note: the project copies the AnyCPU dll using pre build events.*

**In Visual Studio the menu path Test->Test Settings->Default Processor Architecture allows for setting the processor architecture the unit tests are run on.**

## Issues
There is still one more issue to solve. I do not have a solution for deleting the managed C++ dll copied to disk from the AnyCPU's embedded resources. I have tried a couple of different approaches, but so far I have not found a good solution.

If using nuget, a solution which solves the above issue is to configure the nuget package to create x86 and x64 directories when consumed by client code. Doing this absolves the need to emmbed the AnyCPU.Interop.dll into th AnyCPU.dll.