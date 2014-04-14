using System;
using System.Collections.Generic;
using System.IO;

using MG.Framework.Numerics;
using MG.Framework.Utility;

using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace MG.Framework.Audio
{
	/// <summary>
	/// Main audio device. Handles loading and playing audio files.
	/// </summary>
	public class AudioDevice : IDisposable
	{
		private AudioContext context;
		private OggStreamer oggStreamer;
		private Vector3 listenerPosition;
		
		private const int maxSources = 32;
		private int[] sources = new int[maxSources];

		private Dictionary<string, int> soundBuffers = new Dictionary<string,int>();
		private List<MusicStream> musicStreams = new List<MusicStream>();

		public Vector3 ListenerPosition 
		{ 
			get { return listenerPosition; }

			set
			{
				if (!MathTools.Equals(listenerPosition, value))
				{
					listenerPosition = value;
					AL.Listener(ALListener3f.Position, value.X, value.Y, value.Z);
				}
			}
		}

		public AudioDevice()
		{
			context = new AudioContext();
			oggStreamer = new OggStreamer { Logger = new OggLogger() };

			AL.GenSources(sources);

			Log.Info("OpenAL Extensions: " + AL.Get(ALGetString.Extensions));
			Log.Info("OpenAL Renderer: " + AL.Get(ALGetString.Renderer));
			Log.Info("OpenAL Vendor: " + AL.Get(ALGetString.Vendor));
			Log.Info("OpenAL Version: " + AL.Get(ALGetString.Version));
			
			// Check for error
			var error = AL.GetError();
			if (error != ALError.NoError)
			{
				Log.Error("OpenAL Error: " + AL.GetErrorString(error));
			}
		}

		public void Dispose()
		{
			foreach (var stream in musicStreams)
			{
				stream.Stream.Dispose();
			}

			musicStreams.Clear();
			oggStreamer.Dispose();
			
			AL.SourceStop(maxSources, sources);
			AL.DeleteSources(sources);
			
			context.Dispose();
		}
		
		public void PreloadSound(string sound)
		{
			if (soundBuffers.ContainsKey(sound))
			{
				// Sound already loaded, ignore
				return;
			}

			try
			{
				int channels;
				int bitsPerSample;
				int sampleRate;
				byte[] soundData = LoadWave(File.Open(sound, FileMode.Open), out channels, out bitsPerSample, out sampleRate);

				int buffer = AL.GenBuffer();
				AL.BufferData(buffer, GetSoundFormat(channels, bitsPerSample), soundData, soundData.Length, sampleRate);
				soundBuffers.Add(sound, buffer);
			}
			catch (Exception e)
			{
				Log.Error("Could not load sound \"" + sound + "\", error: " + e);
				return;
			}
			
			Log.Info("Loaded sound: " + sound);
		}
		
		public int PlaySound(string sound, bool loop = false, float volume = 1.0f, float pitch = 1.0f, Vector3? position = null)
		{
			// Find the buffer
			int buffer;
			if (!soundBuffers.TryGetValue(sound, out buffer))
			{
				PreloadSound(sound);

				if (!soundBuffers.TryGetValue(sound, out buffer))
				{
					Log.Error("Cannot play sound \"" + sound + "\", could not load it.");
					return 0;
				}
			}

			// Play sound, if possible. Return sound id
			int source;
			if (!FindFreeSource(out source))
			{
				Log.Error("Could not play sound \"" + sound + "\", out of free sources.");
				return 0;
			}
			
			// Play the buffer
			SetSoundPosition(source, position);
			AL.Source(source, ALSourcei.Buffer, buffer);
			AL.Source(source, ALSourcef.Gain, volume);
			AL.Source(source, ALSourcef.Pitch, pitch);
			AL.Source(source, ALSourceb.Looping, loop);
			
			////// The distance that the source will be the loudest (if the listener is
			////// closer, it won't be any louder than if they were at this distance)
			//AL.Source(source, ALSourcef.ReferenceDistance, 1.0f);

			////// The distance that the source will be the quietest (if the listener is
			////// farther, it won't be any quieter than if they were at this distance)
			//AL.Source(source, ALSourcef.MaxDistance, 1.0f);
			
			AL.SourcePlay(source);
			
			return source; 
		}
		
		public void StopSound(int source)
		{
			AL.SourceStop(source);
			CheckError();
		}

		public void SetSoundVolume(int source, float volume)
		{
			AL.Source(source, ALSourcef.Gain, volume);
			CheckError();
		}

		public void SetSoundPosition(int source, Vector3? position = null)
		{
			var soundPosition = position ?? listenerPosition;
			AL.Source(source, ALSource3f.Position, soundPosition.X, soundPosition.Y, 0);
			CheckError();
		}

		public bool IsSource(int source)
		{
			return AL.IsSource(source);
		}

		public bool IsSoundPlaying(int source)
		{
			bool b = AL.GetSourceState(source) == ALSourceState.Playing;
			CheckError();
			return b;
		}

		//public void PauseSound(int source)
		//{
		//    AL.SourcePause(source);
		//}

		//public void ResumeSound(int source)
		//{
		//    AL.SourcePlay(source);
		//}

		public MusicStream LoadMusic(string music)
		{
			MusicStream musicStream;
			try
			{
				musicStream = new MusicStream(new OggStream(music) { IsLooped = true });
			}
			catch (Exception e)
			{
				Log.Info("Could not load music \"" + music + "\", error: " + e);
				throw;
			}
			
			Log.Info("Loaded music: " + music);
			musicStreams.Add(musicStream);
			return musicStream;
		}
		
		private bool FindFreeSource(out int freeSource)
		{
			for (int i = 0; i < sources.Length; i++)
			{
				var s = sources[i];

				if (AL.IsSource(s))
				{
					int state;
					AL.GetSource(s, ALGetSourcei.SourceState, out state);
					if ((ALSourceState)state == ALSourceState.Playing) continue; // Still using this source
				}

				freeSource = s;
				return true;
			}

			freeSource = 0;
			return false;
		}

		private void CheckError()
		{
			var error = AL.GetError();

			if (error != ALError.NoError)
			{
				Log.Warning("Got AL Error: " + AL.GetErrorString(error));
			}
		}
		
		private byte[] LoadWave(Stream stream, out int channels, out int bits, out int rate)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");

			using (BinaryReader reader = new BinaryReader(stream))
			{
				// RIFF header
				string signature = new string(reader.ReadChars(4));
				if (signature != "RIFF")
					throw new NotSupportedException("Specified stream is not a wave file.");

				int riff_chunck_size = reader.ReadInt32();

				string format = new string(reader.ReadChars(4));
				if (format != "WAVE")
					throw new NotSupportedException("Specified stream is not a wave file.");

				// WAVE header
				string format_signature = new string(reader.ReadChars(4));
				if (format_signature != "fmt ")
					throw new NotSupportedException("Specified wave file is not supported.");

				int format_chunk_size = reader.ReadInt32();
				int audio_format = reader.ReadInt16();
				int num_channels = reader.ReadInt16();
				int sample_rate = reader.ReadInt32();
				int byte_rate = reader.ReadInt32();
				int block_align = reader.ReadInt16();
				int bits_per_sample = reader.ReadInt16();

				// Hack: The ExtraParamSize value is sometimes written even though AudioFormat=1.
				if (reader.PeekChar() == '\0')
				{
					reader.ReadChars(2); // ExtraParamSize is two bytes.
				}

				string data_signature = new string(reader.ReadChars(4));
				if (data_signature != "data")
					throw new NotSupportedException("Specified wave file is not supported.");

				int data_chunk_size = reader.ReadInt32();

				channels = num_channels;
				bits = bits_per_sample;
				rate = sample_rate;

				return reader.ReadBytes((int)reader.BaseStream.Length);
			}
		}

		private ALFormat GetSoundFormat(int channels, int bits)
		{
			switch (channels)
			{
				case 1:
					return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
				case 2:
					return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
				default:
					throw new NotSupportedException("The specified sound format is not supported.");
			}
		}
	}
}
