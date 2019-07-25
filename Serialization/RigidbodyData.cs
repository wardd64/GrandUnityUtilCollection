using System;
using UnityEngine;

[Serializable]
public class RigidbodyData : ComponentData {

	bool sleeping;
	Vector3Data position, velocity, angularVelocity;
	QuaternionData rotation;

	public RigidbodyData(Rigidbody body) {

		this.position = new Vector3Data(body.transform.position);
		this.rotation = new QuaternionData(body.transform.rotation);

		this.sleeping = body.IsSleeping();
		if(!sleeping) {
			this.velocity = new Vector3Data(body.velocity);
			this.angularVelocity = new Vector3Data(body.angularVelocity);
		}
	}

	public override void Apply(Component targetComponent) {
		Rigidbody body = (Rigidbody)targetComponent;

		body.position = this.position.Regenerate();
		body.rotation = this.rotation.Regenerate();

		if(sleeping)
			body.Sleep();
		else {
			body.WakeUp();
			body.velocity = this.velocity.Regenerate();
			body.angularVelocity = this.angularVelocity.Regenerate();
		}
	}

	public override void 
		ApplyLerp(Component targetComponent, ComponentData rightData, float factor) {
		Rigidbody body = (Rigidbody)targetComponent;
		RigidbodyData right = (RigidbodyData)rightData;

		body.transform.position = this.position.Lerp(right.position, factor);
		body.transform.rotation = this.rotation.Lerp(right.rotation, factor);
	}
}
