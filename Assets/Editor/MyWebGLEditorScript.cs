using UnityEditor;

public class Foo
{
	Foo()
	{
		PlayerSettings.WebGL.emscriptenArgs = "-Wl,--trace-symbol=sendfile";
	}
}