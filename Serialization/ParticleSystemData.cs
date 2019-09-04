using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ParticleSystemData : ComponentData{

    /// <summary>
    /// NOTES:
    /// particles cannot be loaded properly when multiple particles are spawned 
    /// inside the same frame. As a result, particles bursts should be avoided 
    /// and spawn rates should remain lower than the frame rate of the game.
    /// If required, higher rates and bursts can be achieved by duplicating particle systems.
    /// </summary>

	int size;
	float time;
    bool playing;
	bool emissionEnabled;
	ParticleData[] particles;

	const float kickDeltaTime = 0.02f;

	public ParticleSystemData(ParticleSystem source) {
		//get particles
		ParticleSystem.Particle[] systemParticles;
		systemParticles = new ParticleSystem.Particle[source.main.maxParticles];
		size = source.GetParticles(systemParticles);
		
		//record their Data's
		particles = new ParticleData[size];
		for(int i = 0; i < size; i++)
			particles[i] = new ParticleData(systemParticles[i]);

		//record time
		time = source.time;
        playing = source.isPlaying;
        emissionEnabled = source.emission.enabled;
	}

	public override void Apply(Component targetComponent) {
		ParticleSystem target = (ParticleSystem)targetComponent;

		ParticleSystem.Particle[] systemParticles = Refresh(target, size);

		//apply Data's and apply particles
		for(int i = 0; i < size; i++) {
			systemParticles[i] = particles[i].Generate(systemParticles[i]);
		}
		target.SetParticles(systemParticles, size);

        if(playing)
            Kick(target, this.time);

        if(IsLoopingType(target))
            SetEmission(target, emissionEnabled);
	}

	public override void 
		ApplyLerp(Component targetComponent, ComponentData rightData, float factor) {

		ParticleSystem target = (ParticleSystem)targetComponent;
		ParticleSystemData right = (ParticleSystemData)rightData;

		//determine time
		float loopTime = target.main.duration;
        float deltaTime = FrameData.frameInterval;
        float lerpTime;
        if(IsLoopingType(target))
            lerpTime = GUC.Math.Repeat(this.time + FrameData.timeToLeft, loopTime);
        else if(this.time > 0f)
            lerpTime = Mathf.Clamp(this.time + FrameData.timeToLeft, 0f, loopTime);
        else if(right.time > 0f)
            lerpTime = Mathf.Clamp(right.time - FrameData.timeToRight, 0f, loopTime);
        else
            lerpTime = 0f;

		//check if lerp is caught between a loop reset
		if(deltaTime < 0f) {
			lerpTime = Mathf.Lerp(this.time, right.time + loopTime, factor) % loopTime;
			deltaTime += loopTime;
		}

		//List out all particles, left and right
		List<ParticleData> leftParticles = new List<ParticleData>();
		for(int i = 0; i < this.size; i++)
			leftParticles.Add(new ParticleData(this.particles[i]));
		List<ParticleData> rightParticles = new List<ParticleData>();
		for(int i = 0; i < right.size; i++)
			rightParticles.Add(new ParticleData(right.particles[i]));

		//match them up as well as possible
		int matched = 0;
		List<ParticleData> matchedParticles = new List<ParticleData>();
		while(leftParticles.Count > 0) {
			int sourceI = leftParticles.Count - 1;
			int matchI = FindMatch(leftParticles[sourceI], rightParticles, deltaTime);

			if(matchI >= 0) {
				//if there is a match, add a lerped particle
				ParticleData lerpedParticle = 
					new ParticleData(leftParticles[sourceI], rightParticles[matchI], factor);
				matchedParticles.Add(lerpedParticle);
				leftParticles.RemoveAt(sourceI);
				rightParticles.RemoveAt(matchI);
				matched++; 
			}
			else {
				//if there is no match, add unlerped particle if its not expired by now.
				if(!leftParticles[sourceI].IsExpired(factor * deltaTime))
					matchedParticles.Add(leftParticles[sourceI]);
				leftParticles.RemoveAt(sourceI);
			}
		}
		
		//add any remaining particles at right hand side, if they had already spawned
		for(int i = 0; i < rightParticles.Count; i++) {
			if(!rightParticles[i].IsUnborn(loopTime, (1f - factor) * deltaTime))
				matchedParticles.Add(rightParticles[i]);
		}

		//apply Data's and apply particles
		int lerpSize = matchedParticles.Count;
		ParticleSystem.Particle[] systemParticles = Refresh(target, lerpSize);
        if(lerpSize > systemParticles.Length) {
            Debug.LogWarning("Too many particles recorded for system " + targetComponent.transform.name);
            lerpSize = systemParticles.Length;
        }
		for(int i = 0; i < lerpSize; i++) {
			systemParticles[i] = matchedParticles[i].Generate(systemParticles[i]);
		}
		target.SetParticles(systemParticles, size);

        if(playing && right.playing)
            Kick(target, lerpTime);
        if(IsLoopingType(target))
            SetEmission(target, false);
        else
            target.Stop();
	}

	private void SetEmission(ParticleSystem target, bool enabled) {
		var em = target.emission;
        em.enabled = enabled;
	}

    private static bool IsLoopingType(ParticleSystem system) {
        return system.main.playOnAwake && system.main.loop;
    }

	/// <summary>
	/// Returns index of particle in searchlist that matches the particle toMatch.
	/// Returns -1 if this particle cannot be found.
	/// </summary>
	private int FindMatch(ParticleData toMatch, List<ParticleData> searchList, float deltaTime) {
		for(int i = 0; i < searchList.Count; i++) {
			if(ParticleData.Match(toMatch, searchList[i], deltaTime))
				return i;
		}
		return -1;
	}

	/// <summary>
	/// Clear out particle system and emit given amount of fresh particles
	/// </summary>
	private static ParticleSystem.Particle[] Refresh(ParticleSystem target, int size) {
		ParticleSystem.Particle[] systemParticles;
		systemParticles = new ParticleSystem.Particle[target.main.maxParticles];
		if(target.particleCount == size) {
			target.GetParticles(systemParticles);
			return systemParticles;
		}

		target.SetParticles(systemParticles, 0);
		target.Emit(size);
		target.GetParticles(systemParticles);
		return systemParticles;
	}

	/// <summary>
	/// Assign time and kick particle system so it shows correct particle sizes
	/// </summary>
	private static void Kick(ParticleSystem target, float time) {
		target.time = time - kickDeltaTime;
		target.Simulate(kickDeltaTime, true, false);
		target.Play();
	}

	[Serializable]
	private class ParticleData {

		Vector3Data position, velocity;
		float rotation, angularVelocity;
		float lifeTime;

        /// <summary>
        /// True if this particle should no longer exist
        /// </summary>
		public bool IsExpired(float passedTime) {
			return passedTime >= lifeTime;
		}

        /// <summary>
        /// True if this particle should not exist yet
        /// </summary>
		public bool IsUnborn(float totalLifeTime, float undoneTime) {
			return totalLifeTime - lifeTime >= undoneTime;
		}

		public ParticleData(ParticleData source) {
			position = source.position.Clone();
			velocity = source.velocity.Clone();
			rotation = source.rotation;
			angularVelocity = source.angularVelocity;
			lifeTime = source.lifeTime;
		}

		public ParticleData(ParticleSystem.Particle source) {
			position = new Vector3Data(source.position);
			velocity = new Vector3Data(source.velocity);
			rotation = source.rotation;
			angularVelocity = source.angularVelocity;
			lifeTime = source.remainingLifetime;
		}

		public ParticleData(ParticleData left, ParticleData right, float factor) {
			position = new Vector3Data(left.position.Lerp(right.position, factor));
			velocity = new Vector3Data(left.velocity.Lerp(right.velocity, factor));
			rotation = Mathf.Lerp(left.rotation, right.rotation, factor);
			angularVelocity = Mathf.Lerp(left.angularVelocity, right.angularVelocity, factor);
			lifeTime = Mathf.Lerp(left.lifeTime, right.lifeTime, factor);
		}

		public ParticleSystem.Particle 
			Generate(ParticleSystem.Particle defaultParticle) {
			defaultParticle.position = position.Regenerate();
			defaultParticle.velocity = velocity.Regenerate();
			defaultParticle.rotation = rotation;
			defaultParticle.angularVelocity = angularVelocity;
			defaultParticle.remainingLifetime = lifeTime;
			return defaultParticle;
		}

		public static bool Match(ParticleData left, ParticleData right, float deltaTime) {
			return Mathf.Abs(left.lifeTime - right.lifeTime - deltaTime) < 1e-5f;
		}
	}
}
