using UnityEngine;
using System;

[Serializable]
public class Timer{

	float startTime;
	float currentTime;

    /// <summary>
    /// Constructs a timer object that can be handily made to tick towards 0.
    /// </summary>
    /// <param name="time">Time upon reset</param>
    /// <param name="initialTime">Time upon construction</param>
	public Timer(float time, float initialTime) {
		this.startTime = TimeClamp(time);
		this.currentTime = TimeClamp(initialTime);
	}

    /// <summary>
    /// Constructs a timer object that can be handily made to tick towards 0.
    /// </summary>
    /// <param name="time">Time upon reset and construction.</param>
	public Timer(float time) {
		this.startTime = TimeClamp(time);
		this.currentTime = TimeClamp(time);
	}

	public void Reset() {
		this.currentTime = this.startTime;
	}

    /// <summary>
    /// Set current time without changing the starting (maximum) time
    /// </summary>
	public void Set(float time) {
		this.currentTime = TimeClamp(time);
	}

    /// <summary>
    /// Change both current and starting time of this timer.
    /// </summary>
    public void HardSet(float time) {
        this.currentTime = TimeClamp(time);
        this.startTime = TimeClamp(time);
    }

    /// <summary>
    /// Change starting time of this timer without affecting current time.
    /// Effects will only take root on the next reset/loop
    /// </summary>
    public void SetStart(float time) {
        this.startTime = TimeClamp(time);
    }

    public float MaxTime() {
		return this.startTime;
	}

	/// <summary>
	/// Number of seconds the timer is away from completion
	/// </summary>
	public float GetTime() {
		return currentTime;
	}

	/// <summary>
	/// Fractional value (between 0 and 1) of this timer's completion.
	/// Is 0 when timer has completed, 1 when it is just starting.
	/// </summary>
	public float GetTimeFrac() {
		return currentTime/startTime;
	}

	/// <summary>
	/// Advances time of this timer by given deltaTime. 
	/// Returns true if time is up.
    /// Time unit need not necessarily be seconds.
	/// </summary>
	public bool Tick(float deltaTime) {
		currentTime -= deltaTime;
		if(currentTime <= 0f) {
			currentTime = 0f;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Advances time of this timer; should be called once in Update();
    /// Returns true if time is up. Use this method only from the Unity 
    /// main thread, time units are assumed to be seconds.
	/// </summary>
	public bool Tick() {
		return this.Tick(Time.deltaTime);
	}

	/// <summary>
	/// Advances time of this timer by given deltaTime. 
	/// Returns true if timer runs out in this tick.
	/// </summary>
	public bool TickTrigger(float deltaTime) {
		if(currentTime <= 0f)
			return false;

		currentTime -= deltaTime;
		if(currentTime <= 0f) {
			currentTime = 0f;
			return true;
		}
		return false;
	}

    /// <summary>
    /// advances time of the timer, looping around the end back to the start.
    /// Returns true if the timer loops around in this tick.
    /// </summary>
    public bool LoopTick(float deltaTime) {
		currentTime -= deltaTime;
        bool toReturn = currentTime < 0f;
        if(startTime <= 0f)
            currentTime = 0f;
        else
		    currentTime = Mathf.Repeat(currentTime, startTime);
        return toReturn;
	}

	/// <summary>
	/// advances time of the timer, looping around the end back to the start.
    /// Returns true if the timer loops around in this tick.
	/// </summary>
	public bool LoopTick() {
		return this.LoopTick(Time.deltaTime);
	}

	/// <summary>
	/// Advances time of this timer; should only be called in Update(). 
	/// Returns true if timer runs out in this tick.
	/// </summary>
	public bool TickTrigger() {
		return this.TickTrigger(Time.deltaTime);
	}

	/// <summary>
	/// Advances time of this timer, returning actual delta time
	/// </summary>
	public float PreciseTick() {
		float toReturn = currentTime;
		currentTime -= Time.deltaTime;
		if(currentTime <= 0f) {
			currentTime = 0f;
			return toReturn;
		}
		return Time.deltaTime;
	}

	public bool done {
		get {
			return currentTime <= 0f;
		}
	}

	public void SetDone() {
		this.currentTime = 0f;
	}

	/// <summary>
	/// Return smallest time this timer is away from start or finish, 
	/// useful for managing symetrical transition in and out.
	/// </summary>
	public float GetBorderTime() {
		return Mathf.Min(currentTime, startTime - currentTime);
	}

	/// <summary>
	/// returns true if remaining time is lower than given fraction of the initial time.
	/// </summary>
	public bool DoneFrac(float frac) {
		return currentTime <= frac * startTime;
	}

	/// <summary>
	/// returns true if remaining time is lower than given time
	/// </summary>
	public bool Done(float time) {
		return this.currentTime <= time;
	}

	/// <summary>
	/// Creates fresh copy of this timer. 
	/// Remember to orient your objects kids!
	/// </summary>
	public Timer Clone() {
		return new Timer(this.startTime, this.currentTime);
	}

	/// <summary>
	/// Creates linear interpolotation of this timer and the given timer.
	/// Start time of the timers is assumed equal.
	/// </summary>
	public Timer Lerp(Timer right, float factor) {
		float time = Mathf.Lerp(this.currentTime, right.currentTime, factor);
		return new Timer(this.startTime, time);
	}

    /// <summary>
    /// Clamps given time value between valid floating point values.
    /// </summary>
    private static float TimeClamp(float input) {
        if(float.IsNaN(input))
            return 0f;
        return Mathf.Clamp(input, 0f, float.MaxValue);

    }

    /// <summary>
    /// Ticks time forward if tick is true, resets otherwise.
    /// Returns true if timer has run out.
    /// </summary>
    public bool TimeoutTick(bool tick) {
        if(tick)
            return Tick();
        else
            Reset();
        return false;
    }
}
