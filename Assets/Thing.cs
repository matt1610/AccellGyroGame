using UnityEngine;
using System.Collections;
using SimpleJSON;
using AssemblyCSharp;
using System;

public class Thing : MonoBehaviour {

	public AccellGyroModel accgyro;
	public float smooth = 2.0F;
	public AccelerationToPosition accelerationToPosition;
	private bool calibrated = false;
	int calibrationCount = 0;

	void Start () {
		accgyro = new AccellGyroModel (0, 0, 0, 0, 0, 0, 0, 0);
		accelerationToPosition = new AccelerationToPosition ();
	}

	void Update () {
		Quaternion target = Quaternion.Euler(accgyro.RotX, 0, accgyro.RotY);
		transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);

//		transform.Translate(new Vector3(-accgyro.ScaledAccellX * 10,0,0) * Time.deltaTime);
	}
	
	public void ReceiveData(AccellGyroModel accellGyroModel)
	{
		accgyro = accellGyroModel;

		if (!calibrated) {

			if (calibrationCount < 1025) {

				Debug.Log("Calibrating " + calibrationCount);

				accelerationToPosition.DoOneCalibration(
					Convert.ToInt64( accgyro.ScaledAccellX ), 
					Convert.ToInt64( accgyro.ScaledAccellY )
					);

				calibrationCount++;

			} else {
				accelerationToPosition.EndCalibration();
				calibrated = true;
			}

		} else {
			accelerationToPosition.Sample_X = Convert.ToInt32( accgyro.ScaledAccellX );
			accelerationToPosition.Sample_Y = Convert.ToInt32( accgyro.ScaledAccellY );
			accelerationToPosition.position();
		}

		if (calibrated) {
			Debug.Log (accelerationToPosition.Sensor_Data[2]);
			Debug.Log (accelerationToPosition.Sensor_Data[3]);
		}


	}
}
