using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Reflection;

namespace Shiftless.Clockwork.Retro.Rendering
{
    public class Shader
    {
        // Values
        public readonly int Handle;

        private readonly Dictionary<string, int> _uniformLocations = [];


        // Constructor
        public Shader(string vertexSource, string fragmentSource)
        {
            // Compile the vertex shader
            int vertHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertHandle, vertexSource);
            CompileShader(vertHandle);

            // Compile the fragment shader
            int fragHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragHandle, fragmentSource);
            CompileShader(fragHandle);

            // Create the main program
            Handle = GL.CreateProgram();

            // Attach the stuff to the shader
            GL.AttachShader(Handle, vertHandle);
            GL.AttachShader(Handle, fragHandle);

            // Links it all together :)
            LinkProgram(Handle);

            // Now we can delate the stuffs
            GL.DetachShader(Handle, vertHandle);
            GL.DetachShader(Handle, fragHandle);
            GL.DeleteShader(vertHandle);
            GL.DeleteShader(fragHandle);

            // And cache all uniforms :)
            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
            for (var i = 0; i < numberOfUniforms; i++)
            {
                string key = GL.GetActiveUniform(Handle, i, out _, out _);
                int location = GL.GetUniformLocation(Handle, key);

                _uniformLocations.Add(key, location);
            }
        }


        // Func
        public bool Contains(string name) => _uniformLocations.ContainsKey(name);

        public void Set(string name, int value)
        {
            Use();
            GL.Uniform1(_uniformLocations[name], value);
        }
        public void Set(string name, float value)
        {
            Use();
            GL.Uniform1(_uniformLocations[name], value);
        }
        public void Set(string name, bool value)
        {
            Use();
            GL.Uniform1(_uniformLocations[name], value ? 1 : 0);
        }
        public void Set(string name, Vector2 value)
        {
            Use();
            GL.Uniform2(_uniformLocations[name], value);
        }
        public void Set(string name, Vector2i value)
        {
            Use();
            GL.Uniform2(_uniformLocations[name], value);
        }
        public void Set(string name, Vector3 value)
        {
            Use();
            GL.Uniform3(_uniformLocations[name], value);
        }
        public void Set(string name, Vector3i value)
        {
            Use();
            GL.Uniform3(_uniformLocations[name], value);
        }
        public void Set(string name, Vector4 value)
        {
            Use();
            GL.Uniform4(_uniformLocations[name], value);
        }
        public void Set(string name, Vector4i value)
        {
            Use();
            GL.Uniform4(_uniformLocations[name], value);
        }
        public void Set(string name, Matrix4 value)
        {
            Use();
            GL.UniformMatrix4(_uniformLocations[name], true, ref value);
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }


        // Static stuff
        public static Shader LoadEmbeddedShader(Assembly assembly, string vertexName, string fragmentName)
        {
            vertexName = $"{assembly.GetName().Name ?? throw new Exception("Assembly name was null!?")}.{vertexName}";
            fragmentName = $"{assembly.GetName().Name ?? throw new Exception("Assembly name was null!?")}.{fragmentName}";

            string vertexSource;
            using (Stream stream = assembly.GetManifestResourceStream(vertexName) ?? throw new ArgumentException($"Embedded resource of name {vertexName}"))
            using (StreamReader reader = new StreamReader(stream))
            {
                vertexSource = reader.ReadToEnd();
            }

            string fragmentSource;
            using (Stream stream = assembly.GetManifestResourceStream(fragmentName) ?? throw new ArgumentException($"Embedded resource of name {fragmentName}"))
            using (StreamReader reader = new StreamReader(stream))
            {
                fragmentSource = reader.ReadToEnd();
            }

            return new(vertexSource, fragmentSource);
        }

        private static void CompileShader(int shaderHandle)
        {
            GL.CompileShader(shaderHandle);

            GL.GetShader(shaderHandle, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                var infoLog = GL.GetShaderInfoLog(shaderHandle);
                throw new Exception($"Error occurred whilst compiling Shader({shaderHandle}).\n\n{infoLog}");
            }
        }

        private static void LinkProgram(int programHandle)
        {
            // We link the program
            GL.LinkProgram(programHandle);

            // Check for linking errors
            GL.GetProgram(programHandle, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                throw new Exception($"Error occurred whilst linking Program({programHandle})");
            }
        }
    }
}
