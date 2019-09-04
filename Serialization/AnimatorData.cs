using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public class AnimatorData : ComponentData {

	bool sleeping, enabled;

	AnimState[] animStates;
	AnimParam[] animParams;

	public AnimatorData(Animator anim) {
		this.enabled = anim.enabled;
		this.sleeping = !anim.isActiveAndEnabled && anim.runtimeAnimatorController != null;
		if(this.sleeping)
			return;

		//animation parameters
		int paramCount = anim.parameterCount;
		this.animParams = new AnimParam[paramCount];
		for(int i = 0; i < paramCount; i++) 
			this.animParams[i] = new AnimParam(anim.parameters[i], anim);

		//current state
		int layerCount = anim.layerCount;
		this.animStates = new AnimState[layerCount];
		for(int layer = 0; layer < layerCount; layer++) {
			this.animStates[layer] = new AnimState(layer, anim);
		}
	}

	public override void Apply(Component targetComponent) {
		Animator anim = (Animator)targetComponent;
		
		anim.enabled = this.enabled;
		if(sleeping || !anim.gameObject.activeInHierarchy)
			return;

		//animation parameters
		int paramCount = anim.parameterCount;
		for(int i = 0; i < paramCount; i++)
			this.animParams[i].Apply(anim.parameters[i], anim);

		//current state
		int layerCount = anim.layerCount;
		for(int layer = 0; layer < layerCount; layer++) {
			this.animStates[layer].Apply(layer, anim);
		}
	}

	public override void 
		ApplyLerp(Component targetComponent, ComponentData rightData, float factor) {

		Animator anim = (Animator)targetComponent;
		AnimatorData right = (AnimatorData)rightData;

		//fast asleep
		if((this.sleeping && right.sleeping) || !anim.gameObject.activeInHierarchy) {
			return;
		}

		//waking up
		else if(this.sleeping) {
			right.Apply(targetComponent);
		}

		//going to sleep
		else if(right.sleeping) {
			this.Apply(targetComponent);
		}

		//totally awake
		else {
			//animation parameters
			int paramCount = anim.parameterCount;
			for(int i = 0; i < paramCount; i++)
				this.animParams[i].ApplyLerp(right.animParams[i], factor, anim.parameters[i], anim);

			//current state
			int layerCount = anim.layerCount;
			for(int layer = 0; layer < layerCount; layer++) {
				this.animStates[layer].ApplyLerp(right.animStates[layer], factor, layer, anim);
			}
		}
	}

	[Serializable]
	class AnimParam {
		int value;

		public AnimParam(AnimatorControllerParameter param, Animator anim) {
			switch(param.type) {
			case AnimatorControllerParameterType.Trigger:
			value = 0;
			break;
			case AnimatorControllerParameterType.Bool:
			value = anim.GetBool(param.name) ? 1 : 0;
			break;
			case AnimatorControllerParameterType.Int:
			value = anim.GetInteger(param.name);
			break;
			case AnimatorControllerParameterType.Float:
			value = BitConverter.ToInt32(BitConverter.GetBytes(anim.GetFloat(param.name)), 0);
			break;
			}
		}

		public void Apply(AnimatorControllerParameter param, Animator anim) {
			switch(param.type) {
			case AnimatorControllerParameterType.Bool:
			anim.SetBool(param.name, value == 1);
			break;
			case AnimatorControllerParameterType.Int:
			anim.SetInteger(param.name, value);
			break;
			case AnimatorControllerParameterType.Float:
			anim.SetFloat(param.name, BitConverter.ToSingle(BitConverter.GetBytes(value), 0));
			break;
			}
		}

		public void ApplyLerp(AnimParam right, float factor,
			AnimatorControllerParameter param, Animator anim) {

			if(param.type == AnimatorControllerParameterType.Float) {
				float leftFloat = BitConverter.ToSingle(BitConverter.GetBytes(this.value), 0);
				float rightFloat = BitConverter.ToSingle(BitConverter.GetBytes(right.value), 0);
				anim.SetFloat(param.name, Mathf.Lerp(leftFloat, rightFloat, factor));
			}
			else
				this.Apply(param, anim);
		}
	}

	[Serializable]
	class AnimState {
		/* WARNING:
		 * In some cases it is very difficult, or flat-out impossible to determine the length
		 * of a given transition. In order to contain this problem, the user can provide 
		 * default and maximum transition lengths, that are assigned when needed.
		 * Note that having non-zero transition offsets will exacerbate this problem, 
		 * so it might be best to avoid using those, unless the transition length is 
		 * equal to the provided maximum value.
		 */

		private const float timeShift = 0.001f;

		int currentHash;
		float currentTime;
		float currentLength;
		float weight;

		bool inTransition;
		int nextHash;
		float transitionTime;
		float transitionLength;
		float nextLength;

		/// <summary>
		/// Create AnimState object using state of given anim: SAVING
		/// </summary>
		public AnimState(int layer, Animator anim) {
			this.weight = anim.GetLayerWeight(layer);

			//retrieve current animation info
			AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(layer);
			this.currentHash = info.fullPathHash;
			this.currentLength = info.length;
			this.currentTime = info.normalizedTime * currentLength;

			//if there is a transition, move on and gather transition info
			inTransition = anim.IsInTransition(layer);
			if(inTransition) {
				AnimatorTransitionInfo transInfo = anim.GetAnimatorTransitionInfo(layer);
				AnimatorStateInfo nextInfo = anim.GetNextAnimatorStateInfo(layer);

				//Really Unity? You're making me do this? Get your shit together.
				if(nextInfo.fullPathHash == currentHash) {
					inTransition = false;
					return;
				}

				transitionTime = 0f;
				if(transInfo.normalizedTime == 0f) {
					//move animation forward so we have proper info to work with.
					anim.Update(timeShift);
					transInfo = anim.GetAnimatorTransitionInfo(layer);
					nextInfo = anim.GetNextAnimatorStateInfo(layer);
					//anim.Update(-timeShift);
				}

				//retrieve all necessary info
				nextHash = nextInfo.fullPathHash;
				nextLength = nextInfo.length;
				transitionTime = nextInfo.normalizedTime * nextLength;
				transitionLength = transitionTime / transInfo.normalizedTime;
			}
		}

		/// <summary>
		/// Apply this AnimState object to the given animator: LOADING
		/// </summary>
		public void Apply(int layer, Animator anim) {
			anim.SetLayerWeight(layer, weight);

			//apply current animation info
			anim.Play(currentHash, layer, currentTime/currentLength);
			anim.Update(0f);

			//if necessary, apply transition info
			if(inTransition) {
				//rewind current animation, so it can be wound forward along with the fading
				float playTime = (currentTime/* - transitionTime*/) / currentLength;
				anim.Play(currentHash, layer, playTime);
				anim.Update(0f);

				//set crossfade time and update to the required point
				float crossPlayTime = transitionTime / nextLength;
				anim.CrossFade(nextHash, transitionLength / currentLength, layer, crossPlayTime);
				anim.Update(transitionTime);
			}
		}

		public void ApplyLerp(AnimState right, float factor, int layer, Animator anim) {
			anim.SetLayerWeight(layer, Mathf.Lerp(this.weight, right.weight, factor));

			//if playing the same animation, Lerp animation time
			if(this.currentHash == right.currentHash) {
				float lerpTime = Mathf.Lerp(this.currentTime, right.currentTime, factor);
				anim.Play(currentHash, layer, lerpTime / currentLength);
			}
			//if playing different animations, check which side we're on right now
			else {
				float rightTime = right.currentTime - FrameData.timeToRight;
				float leftTime = this.currentTime + FrameData.timeToLeft;
				if(rightTime > 0f)
					anim.Play(right.currentHash, layer, rightTime / right.currentLength);
				else
					anim.Play(this.currentHash, layer, leftTime / this.currentLength);
			}
			anim.Update(0f);

			

			//set up lerped transition variables,
			//search left and right Data to fill them up.
			bool foundTransition = false;

			float lerpCurrentTime = 0f;
			float lerpCurrentLength = 0f;
			int lerpCurrentHash = 0;

			float lerpTransitionTime = 0f;
			float lerpTransitionLength = 0f;
			float lerpNextLength = 0f;
			int lerpNextHash = 0;

			//prioritize newer transition
			if(right.inTransition) {
				float rightTransTime = right.transitionTime - FrameData.timeToRight;
				foundTransition = rightTransTime > 0f;

				//assign right transition values
				if(foundTransition) {
					lerpCurrentTime = right.currentTime - FrameData.timeToRight;
					lerpCurrentLength = right.currentLength;
					lerpCurrentHash = right.currentHash;

					lerpTransitionTime = rightTransTime;
					lerpTransitionLength = right.transitionLength;
					lerpNextLength = right.nextLength;
					lerpNextHash = right.nextHash;
				}
			}

			//if transition was found check if the same transition is happening on the left
			if(foundTransition && this.inTransition && 
				this.nextHash == right.nextHash && this.currentHash == right.currentHash) {
				lerpTransitionTime =
					Mathf.Lerp(this.transitionTime, right.transitionTime, factor);
				lerpCurrentTime =
					Mathf.Lerp(this.currentTime, right.currentTime, factor);
			}

			//if no transition was found, try older transition too
			if(!foundTransition && this.inTransition) {
				float leftTransTime = this.transitionTime + FrameData.timeToLeft;
				foundTransition = leftTransTime < this.transitionLength;

				//assign left transition values
				if(foundTransition) {
					lerpCurrentTime = this.currentTime + FrameData.timeToLeft;
					lerpCurrentLength = this.currentLength;
					lerpCurrentHash = this.currentHash;

					lerpTransitionTime = leftTransTime;
					lerpTransitionLength = this.transitionLength;
					lerpNextLength = this.nextLength;
					lerpNextHash = this.nextHash;
				}
			}

			//now finally, apply the transition info, if any was found
			if(foundTransition) {
				//rewind current animation, so it can be wound forward along with the fading
				float playTime = (lerpCurrentTime/* - lerpTransitionTime*/) / lerpCurrentLength;
				anim.Play(lerpCurrentHash, layer, playTime);
				anim.Update(0f);

				//set crossfade time and update to the required point
				float crossPlayTime = lerpTransitionTime / lerpNextLength;
				anim.CrossFade(lerpNextHash, lerpTransitionLength / lerpCurrentLength, layer, crossPlayTime);
				anim.Update(lerpTransitionTime);
			}
		}
    }

    public override string ToString() {
        StringBuilder toReturn = new StringBuilder("AnimMom: ");
        toReturn.Append(sleeping ? "sleeping " : "");
        toReturn.Append(enabled ? "active" : "inactive");
        

        if(animStates != null && animStates.Length > 0) {
            toReturn.Append(" - State[0]: ");
            toReturn.Append(animStates[0]);
        }
        else {
            toReturn.Append(" - Empty");
        }

        return toReturn.ToString();
    }
}
