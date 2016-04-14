using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using AssemblyCSharp;
using SimpleJSON;

public class socketScript : MonoBehaviour {

	private TCPConnection myTCP;
	private string serverMsg;
	public string msgToServer;
	public GameObject thingObject;
	public Thing thing;
	public AccellGyroModel accellGyroModel;
	
	void Awake() {
		//add a copy of TCPConnection to this game object
		myTCP = gameObject.AddComponent<TCPConnection>();
	}
	
	void Start () {
		thing = thingObject.GetComponent<Thing> ();

		string theJson = "{\"Name\" : \"Matthew\"}";
		var json = JSON.Parse (theJson);
		Debug.Log(json["Name"]);
	}

	void Update () {
		//keep checking the server for messages, if a message is received from server, it gets logged in the Debug console (see function below)
		SocketResponse ();	
	}
	
	
	
	void OnGUI() {
		//if connection has not been made, display button to connect
		
		if (myTCP.socketReady == false) {
			if (GUILayout.Button ("Connect")) {
				//try to connect
				Debug.Log("Attempting to connect..");
				
				myTCP.setupSocket();
			}
			
		}

		//once connection has been made, display editable text field with a button to send that string to the server (see function below)
		if (myTCP.socketReady == true) {
			
			msgToServer = GUILayout.TextField(msgToServer);
			if (GUILayout.Button ("Write to server", GUILayout.Height(30))) {
				SendToServer(msgToServer);
			}
			
		}	
		
	}
	
	//socket reading script
	void SocketResponse() {
		
		string serverSays = myTCP.readSocket();
		
		if (serverSays != "") {
//			Debug.Log("[SERVER]" + serverSays);

			string[] strings = serverSays.Split(',');

			accellGyroModel = new AccellGyroModel(
				float.Parse(strings[0]),
				float.Parse(strings[1]),
				float.Parse(strings[2]),
				float.Parse(strings[3]),
				float.Parse(strings[4]),
				float.Parse(strings[5]),
				float.Parse(strings[6]),
				float.Parse(strings[7])
				);

//			Debug.Log("[SERVER]" + accellGyroModel.RotX);

			thing.ReceiveData(accellGyroModel);


			//Make this work, JSON is being a dick for some reason I cannot fathom at this time.
//			var json = JSON.Parse(serverSays);
//			var json = JSON.Parse("{"RotY": -4.37688574424315,"RotX": 65.72613775760719,"AccelZ": 0.374755859375,"AccelX": 0.07080078125,"AccelY": 0.845703125,"GyroZ": -1,"GyroX": -5,"GyroY": -1}");
//
//
//			accellGyroModel = new AccellGyroModel(
//				json["AccelX"].AsFloat,
//				json["AccelY"].AsFloat,
//				json["AccelZ"].AsFloat,
//				json["GyroX"].AsFloat,
//				json["GyroY"].AsFloat,
//				json["GyroZ"].AsFloat,
//				json["RotX"].AsFloat,
//				json["RotY"].AsFloat);
//
//			Debug.Log("[SERVER]" + accellGyroModel.ScaledAccellX);
		}
		
	}

	//send message to the server
	public void SendToServer(string str) {
		myTCP.writeSocket(str);
		Debug.Log ("[CLIENT] -> " + str);
	}
	
}
