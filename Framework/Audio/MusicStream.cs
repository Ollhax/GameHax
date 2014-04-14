using System;

namespace MG.Framework.Audio
{
	/// <summary>
	/// A wrapper for music streams.
	/// </summary>
	public class MusicStream
	{
		internal OggStream Stream;

		public float Volume { get { return Stream.Volume; } set { Stream.Volume = Math.Max(value, 0); } }

		public bool Playing { get { return Stream.IsPlaying; } }

		internal MusicStream(OggStream stream)
		{
			Stream = stream;
		}

		public void Play(float volume = 1.0f)
		{
			Stream.Volume = volume;
			Stream.Play();
		}

		public void Stop()
		{
			Stream.Stop();
		}

		public void Pause()
		{
			Stream.Pause();
		}

		public void Resume()
		{
			Stream.Resume();
		}
	}
}
