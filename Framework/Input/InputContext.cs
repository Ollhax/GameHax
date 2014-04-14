using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using MG.Framework.Numerics;
using MG.Framework.Utility;
using MG.Framework.Converters;

namespace MG.Framework.Input
{
	/// <summary>
	/// Handles the transformation from real key inputs to input system ranges and actions.
	/// </summary>
	public class InputContext : IDisposable
	{
		private class RangeTransform
		{
			public double GetNormalizedValue(double realValue)
			{
				if (MathTools.Equals(InputMax, InputMin)) return 0;
				var inputClamped = MathTools.Clamp(realValue, InputMin, InputMax);
				var inputNormalized = (inputClamped - InputMin) / (InputMax - InputMin);
				var output = (inputNormalized * (OutputMax - OutputMin)) + OutputMin;
				return output;
			}

			public string Range;
			public double InputMin;
			public double InputMax;
			public double OutputMin;
			public double OutputMax;

			public string InputMinString;
			public string InputMaxString;
			public string OutputMinString;
			public string OutputMaxString;
		}

		private Dictionary<InputBinding, HashSet<string>> buttonTransforms = new Dictionary<InputBinding, HashSet<string>>();
		private Dictionary<string, RangeTransform> rangeTransforms = new Dictionary<string, RangeTransform>();
		
		public string Name { get; private set; }
		public int Priority { get; private set; }
		
		public InputContext(string name, int priority)
		{
			Name = name;
			Priority = priority;
			InputHandler.SystemValuesChanged += ReparseRangeStrings;
		}

		public void Dispose()
		{
			InputHandler.SystemValuesChanged -= ReparseRangeStrings;
		}

		public void Load(string file)
		{
			Log.Debug("Loading inputs from " + file);

			try
			{
				using (FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					Load(fs);
				}
			}
			catch (Exception e)
			{
				Log.Error("- Error: " + e.Message);
			}
		}

		public void Load(Stream stream)
		{
			using (var xmlReader = XmlReader.Create(stream, XmlHelper.DefaultReaderSettings))
			{
				var document = new XmlDocument();
				document.Load(xmlReader);
				Load(document.DocumentElement);
			}
		}

		public void Load(XmlNode node)
		{
			foreach (XmlNode child in node.ChildNodes)
			{
				if (child.Name == "Bind")
				{
					if (XmlHelper.HasAttribute(child, "Action"))
					{
						var action = XmlHelper.ReadAttributeString(child, "Action");
						var input = XmlHelper.ReadAttributeString(child, "Input");
						Bind(action, input);
					}
					else if (XmlHelper.HasAttribute(child, "Range"))
					{
						var action = XmlHelper.ReadAttributeString(child, "Range");
						var input = XmlHelper.ReadAttributeString(child, "Input");
						var inputMin = XmlHelper.ReadAttributeString(child, "InputMin");
						var inputMax = XmlHelper.ReadAttributeString(child, "InputMax");
						var outputMin = XmlHelper.ReadAttributeString(child, "OutputMin");
						var outputMax = XmlHelper.ReadAttributeString(child, "OutputMax");
						Bind(action, input, inputMin, inputMax, outputMin, outputMax);
					}
				}
			}
		}
		
		public void Bind(string action, string input)
		{
			InputBinding binding;
			try
			{
				binding = new InputBinding(input);
			}
			catch (Exception e)
			{
				Log.Error(e.Message);
				return;
			}

			HashSet<string> actions;
			if (!buttonTransforms.TryGetValue(binding, out actions))
			{
				actions = new HashSet<string>();
				buttonTransforms.Add(binding, actions);
			}

			if (!actions.Contains(action))
			{
				Log.Frequent("Bound " + binding + " to action " + action);
				actions.Add(action);
			}
			else
			{
				Log.Error("Tried to re-bind " + binding + " to " + action);
			}
		}

		public void Bind(string range, string axis, string inputMin, string inputMax, string outputMin, string outputMax)
		{
			if (rangeTransforms.ContainsKey(axis))
			{
				Log.Error("Tried to re-bind range " + axis);
				return;
			}

			var newRange = new RangeTransform
				{
					Range = range,
					InputMinString = inputMin,
					InputMaxString = inputMax,
					OutputMinString = outputMin,
					OutputMaxString = outputMax
				};

			ParseRangeStrings(newRange);
			rangeTransforms.Add(axis, newRange);

			Log.Frequent("Bound " + axis + " to range " + range);
		}
		
		private void ParseRangeStrings(RangeTransform range)
		{
			range.InputMin = ParseRangeParam(range.InputMinString);
			range.InputMax = ParseRangeParam(range.InputMaxString);
			range.OutputMin = ParseRangeParam(range.OutputMinString);
			range.OutputMax = ParseRangeParam(range.OutputMaxString);
		}

		private double ParseRangeParam(string param)
		{
			// Try system values
			double r;
			if (InputHandler.SystemValues.TryGetValue(param, out r))
			{
				return r;
			}
			
			// Try parse it
			if (TypeConvert.FromString(param, out r))
			{
				return r;
			}
			
			Log.Error("Invalid range param: " + param);
			return 0;
		}

		internal void ReparseRangeStrings()
		{
			foreach (var range in rangeTransforms)
			{
				ParseRangeStrings(range.Value);
			}
		}

		internal bool TranslateButton(Button button, KeyModifier modifier, HashSet<string> actions)
		{
			HashSet<string> a;
			bool success = false;

			foreach (var transformPair in buttonTransforms)
			{
				var binding = transformPair.Key;

				if (binding.Button == button && (binding.KeyModifier == modifier || binding.KeyModifier == KeyModifier.Any || modifier == KeyModifier.Any))
				{
					actions.UnionWith(transformPair.Value);
					success = true;
				}
			}
			
			return success;
		}

		internal bool TranslateAxis(string axis, double value, Dictionary<string, Range> ranges)
		{
			RangeTransform transform;
			if (rangeTransforms.TryGetValue(axis, out transform))
			{
				ranges[transform.Range] = new Range { Name = transform.Range, Value = transform.GetNormalizedValue(value) };
				return true;
			}

			return false;
		}
	}
}

