using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;

namespace MySilkProgram;

public class Program
{
    private static IWindow _window;
    private static GL _gl;

    private static uint program;

    private static readonly string VertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec3 vPos;
        layout (location = 1) in vec4 vCol;

        out vec4 outCol;

        void main()
        {
            outCol = vCol;
            gl_Position = vec4(vPos.x,vPos.y,vPos.z,1.0);
        }
    ";

    private static readonly string FragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;

        in vec4 outCol;

        void main()
        {
            FragColor = outCol;
        }
    ";
    

    public static void Main(string[] args)
    {
        WindowOptions options = WindowOptions.Default;
        options.Size = new Vector2D<int>(800, 600);
        options.Title = "My first Silk.NET program!";

        _window = Window.Create(options);

        _window.Load += OnLoad;
        _window.Update += OnUpdate;
        _window.Render += OnRender;

        _window.Run();
    }

    private static void OnLoad(){

        _gl = _window.CreateOpenGL();
         
    	_gl.ClearColor( 0.0f, 0.0f, 0.0f, 0.0f );

        uint vshader = _gl.CreateShader(ShaderType.VertexShader);
        uint fshader = _gl.CreateShader(ShaderType.FragmentShader);

        _gl.ShaderSource(vshader, VertexShaderSource);
        _gl.ShaderSource(fshader, FragmentShaderSource);

        _gl.CompileShader(vshader);
        _gl.CompileShader(fshader);

        program = _gl.CreateProgram();
        _gl.AttachShader(program, vshader);
        _gl.AttachShader(program, fshader);
        _gl.LinkProgram(program);        
        _gl.DetachShader(program, vshader);
        _gl.DetachShader(program, fshader);
        _gl.DeleteShader(vshader);
        _gl.DeleteShader(fshader);
    
        _gl.GetProgram( program, GLEnum.LinkStatus, out var status );
			if ( status == 0 ) {
				Console.WriteLine( $"Error linking shader {_gl.GetProgramInfoLog( program )}" );
			}
    }

    private static void OnUpdate(double dt){
        // no OpenGL
    }

    private static unsafe void OnRender(double dt){

        _gl.Clear(ClearBufferMask.ColorBufferBit);
        
        uint vao = _gl.GenVertexArray();

        _gl.BindVertexArray(vao);

        uint vertices = _gl.GenBuffer();
        uint colors = _gl.GenBuffer();
        uint indices = _gl.GenBuffer();
    

	    float[] vertexArray = new float[] {
		    -0.5f, -0.5f, 0.0f,
			+0.5f, -0.5f, 0.0f,
			0.0f, +0.5f, 0.0f
		};

		float[] colorArray = new float[] {
			1.0f, 0.0f, 0.0f, 1.0f,
			0.0f, 0.0f, 1.0f, 1.0f,
			0.0f, 1.0f, 0.0f, 1.0f
		};

		uint[] indexArray = new uint[] {0, 1, 2};
  
        _gl.BindBuffer(GLEnum.ArrayBuffer, vertices);
        _gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)vertexArray.AsSpan(), GLEnum.StaticDraw);
        _gl.VertexAttribPointer(0,3,GLEnum.Float, false,0,null); 
        _gl.EnableVertexAttribArray(0);
    
        
        _gl.BindBuffer(GLEnum.ArrayBuffer, colors);
        _gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)colorArray.AsSpan(), GLEnum.StaticDraw);
        _gl.VertexAttribPointer(1,4,GLEnum.Float, false,0,null); 
        _gl.EnableVertexAttribArray(1);


        _gl.BindBuffer(GLEnum.ElementArrayBuffer, indices);
        _gl.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>)indexArray.AsSpan(), GLEnum.StaticDraw);

        _gl.BindBuffer(GLEnum.ArrayBuffer,0);
        
        _gl.UseProgram(program);

        _gl.DrawElements(GLEnum.Triangles,3,GLEnum.UnsignedInt,null); 
        
        _gl.BindVertexArray(vao);

        _gl.DeleteBuffer(vertices);
        _gl.DeleteBuffer(colors);
        _gl.DeleteBuffer(indices);
        _gl.DeleteVertexArray(vao);

    }
}
