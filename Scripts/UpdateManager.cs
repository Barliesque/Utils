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
		private event UpdateHandler _halfSecUpdates;
		private event UpdateHandler _quarterSecUpdates;
		private event UpdateHandler _eighthSecUpdates;
		private event UpdateHandler _scaledUpdates;

		private const int _30fpsMS = 1000 / 30; // Milliseconds per frame at 30fps
		private const int _1secMS = 1000;
		private const int _halfSecMS = 500;
		private const int _quarterSecMS = 250;
		private const int _eighthSecMS = 125;

		private Stopwatch timer30FPS;
		private Stopwatch timer1Sec;
		private Stopwatch timerHalfSec;
		private Stopwatch timerQuarterSec;
		private Stopwatch timerEighthSec;
		private Stopwatch timerFixed;

		public delegate void UpdateHandler(float deltaTime);

		public enum Timing
		{
			Frame,
			Late,
			Fixed,
			Frame30FPS,
			OncePerSecond,
			EveryHalfSecond,
			EveryQuarterSecond,
			EveryEighthSecond,
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
			timerFixed = new Stopwatch();
			timerFixed.Start();
			timer1Sec = new Stopwatch();
			timer1Sec.Start();
			timerHalfSec = new Stopwatch();
			timerHalfSec.Start();
			timerQuarterSec = new Stopwatch();
			timerQuarterSec.Start();
			timerEighthSec = new Stopwatch();
			timerEighthSec.Start();
		}

		/// <summary>
		/// Add an update callback at the specified timing interval.
		/// NOTE: Be careful to remove update callbacks in, for example, OnDestroy().
		/// </summary>
		/// <param name="update">Update callback will receive a (float) deltaTime parameter, with the precise duration since it was last called — <i>not the duration between frames.</i></param>
		/// <param name="timing">The UpdateManager.Timing enum provides a variety of different intervals, mostly called from a central Update() routine.</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
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
				case Timing.EveryHalfSecond:
					_halfSecUpdates += update;
					break;
				case Timing.EveryQuarterSecond:
					_quarterSecUpdates += update;
					break;
				case Timing.EveryEighthSecond:
					_eighthSecUpdates += update;
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
				case Timing.EveryHalfSecond:
					_halfSecUpdates -= update;
					break;
				case Timing.EveryQuarterSecond:
					_quarterSecUpdates -= update;
					break;
				case Timing.EveryEighthSecond:
					_eighthSecUpdates -= update;
					break;
				case Timing.Scaled:
					_scaledUpdates -= update;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(timing), timing, "Undefined update handler!");
			}
		}

		static public void RemoveAll()
		{
			_inst._fixedUpdates = null;
			_inst._frameUpdates = null;
			_inst._lateUpdates = null;
			_inst._30FPSUpdates = null;
			_inst._oneSecUpdates = null;
			_inst._halfSecUpdates = null;
			_inst._quarterSecUpdates = null;
			_inst._eighthSecUpdates = null;
			_inst._scaledUpdates = null;
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

			if (timerHalfSec.ElapsedMilliseconds >= _halfSecMS)
			{
				_halfSecUpdates?.Invoke(timerHalfSec.ElapsedMilliseconds / 1000f);
				timerHalfSec.Reset();
				timerHalfSec.Start();
			}

			if (timerQuarterSec.ElapsedMilliseconds >= _quarterSecMS)
			{
				_quarterSecUpdates?.Invoke(timerQuarterSec.ElapsedMilliseconds / 1000f);
				timerQuarterSec.Reset();
				timerQuarterSec.Start();
			}

			if (timerEighthSec.ElapsedMilliseconds >= _eighthSecMS)
			{
				_eighthSecUpdates?.Invoke(timerEighthSec.ElapsedMilliseconds / 1000f);
				timerEighthSec.Reset();
				timerEighthSec.Start();
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

		/// <inheritdoc cref="AddUpdate" />
		static public void Add(UpdateHandler update, Timing timing = Timing.Frame)
		{
			if (!_inst)
			{
				var go = new GameObject("UpdateManager");
				_inst = go.AddComponent<UpdateManager>();
			}

			_inst.AddUpdate(update, timing);
		}

		static public void Remove(UpdateHandler update, Timing timing = Timing.Frame)
		{
			if (_inst) _inst.RemoveUpdate(update, timing);
		}
	}
}