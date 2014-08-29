using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using MG.Framework.Utility;

using OpenTK.Graphics.OpenGL;

namespace MG.Framework.Graphics
{
	public class Effect : IDisposable
	{
		protected int fragmentShaderObject;
		protected int shaderProgram;
		protected int vertexShaderObject;

		protected Dictionary<string, int> Attributes = new Dictionary<string, int>();
		protected Dictionary<string, int> Uniforms = new Dictionary<string, int>();

		public Effect(string vertexShader, string fragmentShader)
		{
			int status_code;
			string info;

			vertexShaderObject = GL.CreateShader(ShaderType.VertexShader);
			fragmentShaderObject = GL.CreateShader(ShaderType.FragmentShader);

			// Compile vertex shader
			GL.ShaderSource(vertexShaderObject, vertexShader);
			GL.CompileShader(vertexShaderObject);
			GL.GetShaderInfoLog(vertexShaderObject, out info);
			GL.GetShader(vertexShaderObject, ShaderParameter.CompileStatus, out status_code);

			if (status_code != 1)
			{
				throw new ApplicationException(info);
			}

			// Compile vertex shader
			GL.ShaderSource(fragmentShaderObject, fragmentShader);
			GL.CompileShader(fragmentShaderObject);
			GL.GetShaderInfoLog(fragmentShaderObject, out info);
			GL.GetShader(fragmentShaderObject, ShaderParameter.CompileStatus, out status_code);

			if (status_code != 1)
			{
				throw new ApplicationException(info);
			}

			shaderProgram = GL.CreateProgram();
			GL.AttachShader(shaderProgram, fragmentShaderObject);
			GL.AttachShader(shaderProgram, vertexShaderObject);

			GL.LinkProgram(shaderProgram);

			int activeAttributes;
			int activeUniforms;
			int attributeLength;
			int uniformLength;
			GL.GetProgram(shaderProgram, ProgramParameter.ActiveAttributes, out activeAttributes);
			GL.GetProgram(shaderProgram, ProgramParameter.ActiveUniforms, out activeUniforms);
			GL.GetProgram(shaderProgram, ProgramParameter.ActiveAttributeMaxLength, out attributeLength);
			GL.GetProgram(shaderProgram, ProgramParameter.ActiveUniformMaxLength, out uniformLength);
			
			if (attributeLength > 0)
			{
				var attributeName = new StringBuilder(attributeLength);
				for (int i = 0; i < activeAttributes; i++)
				{
					int length;
					int size;
					ActiveAttribType type;
					GL.GetActiveAttrib(shaderProgram, i, attributeLength, out length, out size, out type, attributeName);

					string attributeNameStr = attributeName.ToString();
					var loc = GL.GetAttribLocation(shaderProgram, attributeNameStr);
					Attributes[attributeNameStr] = loc;

					//Log.Print(LogSystem.Graphics, LogTier.Info, attributeNameStr);
				}
			}

			if (uniformLength > 0)
			{
				var uniformName = new StringBuilder(uniformLength);
				for (int i = 0; i < activeUniforms; i++)
				{
					int length;
					int size;
					ActiveUniformType type;
					GL.GetActiveUniform(shaderProgram, i, uniformLength, out length, out size, out type, uniformName);

					string uniformNameStr = uniformName.ToString();
					var loc = GL.GetUniformLocation(shaderProgram, uniformNameStr);
					Uniforms[uniformNameStr] = loc;

					//Log.Print(LogSystem.Graphics, LogTier.Info, uniformNameStr);
				}
			}
		}

		public virtual void Enable()
		{
			GL.UseProgram(shaderProgram);
		}

		public virtual void Disable()
		{
			GL.UseProgram(0);
		}

		public void Dispose()
		{
			if (shaderProgram != 0)
			{
				GL.DeleteProgram(shaderProgram);
			}

			if (fragmentShaderObject != 0)
			{
				GL.DeleteShader(fragmentShaderObject);
			}

			if (vertexShaderObject != 0)
			{
				GL.DeleteShader(vertexShaderObject);
			}
		}

		protected static StreamReader Load(FilePath file)
		{
			try
			{
				return File.OpenText(file);
			}
			catch (Exception e)
			{
				Log.Error("Could not load file: " + file + ", error: " + e.Message);
			}

			return null;
		}
	}
}