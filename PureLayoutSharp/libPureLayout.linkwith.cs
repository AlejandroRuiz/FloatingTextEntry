using ObjCRuntime;

[assembly: LinkWith("libPureLayout.a", LinkTarget.ArmV7 | LinkTarget.Simulator | LinkTarget.Simulator64 | LinkTarget.Arm64,
	SmartLink = true, 
	ForceLoad = true, 
	LinkerFlags = "-ObjC")]
