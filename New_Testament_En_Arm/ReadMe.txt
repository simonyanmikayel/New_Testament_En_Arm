1.	API __CxxFrameHandler4 in vcruntime140_1_app.dll is not supported for this application type. Microsoft.Graphics.Canvas.dll calls this API
	VS2019->Properties->C/C++->Command Line add '-d2FH4-'
	VS2019->Properties->Linker->Command Line add '-d2:-FH4-'