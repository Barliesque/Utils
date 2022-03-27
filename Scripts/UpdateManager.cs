using UnityEngine;
using System.Diagnostics;
using System;

namespace Barliesque.Utils
{
	
	public sealed class UpdateManager : MonoBehaviour
	{
		static private UpdateManager _inst;

		private event UpdateHandler _frameUpdates;
		private event UpdateHandler _lateUpdates;
		private event UpdateHandler _fixedUpdates;
		private event UpdateHandler _30FPSUpdates;
		private event UpdateHandler _oneSecUpdates;
		private event UpdateHandler _scaledUpdates;

		private const int _30fpsMS = 1000 / 30; // Milliseconds per frame at 30fps
		private const int _1secMS = 1000;

		private Stopwatch timer30FPS;
		private Stopwatch timer1Sec;
		private Stopwatch timerFixed;

		public delegate void UpdateHandler(float deltaTime);

		public enum Timing
		{
			Frame,
			Late,
			Fixed,
			Frame30FPS,
			OncePerSecond,
			Scaled
		}

		public UpdateManager()
		{
			if (_inst != null) throw new Exception("UpdateManager is a singleton class and may not be instantiated more than once.");
			_inst = this;
		}

		private void Awake()
		{
			timer30FPS = new Stopwatch();
			timer30FPS.Start();
			timer1Sec = new Stopwatch();
			timer1Sec.Start();
			timerFixed = new Stopwatch();
			timerFixed.Start();
		}

		public void AddUpdate(UpdateHandler update, Timing timing = Timing.Frame)
		{
			RemoveUpdate(update, timing);
			switch (timing)
			{
				case Timing.Fixed:
					_fixedUpdates += update;
					break;
				case Timing.Frame:
					_frameUpdates += update;
					break;
				case Timing.Late:
					_lateUpdates += update;
					break;
				case Timing.Frame30FPS:
					_30FPSUpdates += update;
					break;
				case Timing.OncePerSecond:
					_oneSecUpdates += update;
					break;
				case Timing.Scaled:
					_scaledUpdates += update;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(timing), timing, "Undefined update handler!");
			}
		}

		public void RemoveUpdate(UpdateHandler update, Timing timing = Timing.Frame)
		{
			switch (timing)
			{
				case Timing.Fixed:
					_fixedUpdates -= update;
					break;
				case Timing.Frame:
					_frameUpdates -= update;
					break;
				case Timing.Late:
					_lateUpdates -= update;
					break;
				case Timing.Frame30FPS:
					_30FPSUpdates -= update;
					break;
				case Timing.OncePerSecond:
					_oneSecUpdates -= update;
					break;
				case Timing.Scaled:
					_scaledUpdates -= update;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(timing), timing, "Undefined update handler!");
			}
		}

		public void RemoveAll()
		{
			_fixedUpdates = null;
			_frameUpdates = null;
			_lateUpdates = null;
			_30FPSUpdates = null;
			_oneSecUpdates = null;
			_scaledUpdates = null;
		}

		//....................................

		private void Update()
		{
			_frameUpdates?.Invoke(Time.unscaledDeltaTime);
			_scaledUpdates?.Invoke(Time.deltaTime);

			// Err on the side of more frequent updates to account for framerate fluctuations
			if (timer30FPS.ElapsedMilliseconds + Time.unscaledDeltaTime >= _30fpsMS)
			{
				_30FPSUpdates?.Invoke(timer30FPS.ElapsedMilliseconds / 1000f);
				timer30FPS.Reset();
				timer30FPS.Start();
			}

			if (timer1Sec.ElapsedMilliseconds >= _1secMS)
			{
				_oneSecUpdates?.Invoke(timer1Sec.ElapsedMilliseconds / 1000f);
				timer1Sec.Reset();
				timer1Sec.Start();
			}
		}

		private void FixedUpdate()
		{
			_fixedUpdates?.Invoke(timerFixed.ElapsedMilliseconds / 1000f);
			timerFixed.Reset();
			timerFixed.Start();
		}

		private void LateUpdate()
		{
			_lateUpdates?.Invoke(Time.unscaledDeltaTime);
		}

		//.....................................................

		static public void Add(UpdateHandler update, Timing timing = Timing.Frame)
		{
			if (_inst == null)
			{
				var go = new GameObject("UpdateManager");
				_inst = go.AddComponent<UpdateManager>();
			}

			_inst.AddUpdate(update, timing);
		}

		static public void Remove(UpdateHandler update, Timing timing = Timing.Frame)
		{
			if (_inst != null)
				_inst.RemoveUpdate(update, timing);
		}
	}
}