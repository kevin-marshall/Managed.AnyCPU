// AnyCPU.Interop.h

#pragma once

using namespace System;

namespace AnyCPUInterop {

	public ref class Object
	{
	public:
		property System::String^ Platform
		{
			System::String^ get()
			{
				return gcnew System::String((System::Environment::Is64BitProcess) ? "x64" : "x86");
			}
		}
	};
}
