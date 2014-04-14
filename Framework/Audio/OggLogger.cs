
namespace MG.Framework.Audio
{
	class OggLogger : LoggerBase
	{
		public override void Log(LogEventBoolean eventType, bool context)
		{
			switch (eventType)
			{
				case LogEventBoolean.IsOpenAlSoft:
					Utility.Log.Info("OpenAL Soft: " + context);
					break;
				case LogEventBoolean.XRamSupport:
					Utility.Log.Info("X-RAM: " + context);
					break;
				case LogEventBoolean.EfxSupport:
					Utility.Log.Info("Efx: " + context);
					break;
			}
		}

		public override void Log(LogEventSingle eventType, float context)
		{

		}

		public override void Log(LogEvent eventType, OggStream stream)
		{
			switch (eventType)
			{
				//case LogEvent.BeginPrepare:
				//    Write("(*", 7, p.Y);
				//    SetHOffset(stream, 9);
				//    break;
				//case LogEvent.EndPrepare:
				//    Write(")", p.X, p.Y);
				//    SetHOffset(stream, p.X + 1);
				//    break;
				case LogEvent.Play:
					Utility.Log.Info("Playing music stream.");
					break;
				case LogEvent.Stop:
					Utility.Log.Info("Stopping music stream.");
					break;
				case LogEvent.Pause:
					Utility.Log.Info("Pausing music stream.");
					break;
				case LogEvent.Resume:
					Utility.Log.Info("Resuming music stream.");
					break;
				//case LogEvent.Empty:
				//    Write(new string(Enumerable.Repeat(' ', Console.BufferWidth - 6).ToArray()), 6, p.Y);
				//    SetHOffset(stream, 7);
				//    break;
				//case LogEvent.NewPacket:
				//    Write(".", p.X, p.Y);
				//    SetHOffset(stream, p.X + 1);
				//    break;
				//case LogEvent.LastPacket:
				//    Write("|", p.X, p.Y);
				//    SetHOffset(stream, p.X + 1);
				//    break;

				case LogEvent.BufferUnderrun:
					Utility.Log.Warning("Music stream buffer underrun detected.");
					break;
			}
		}
	}
}
