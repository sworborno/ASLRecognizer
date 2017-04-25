//Author: Jiban Adhikary



using Leap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net.Sockets; 

using UnityEngine.Networking;

public class TestSample : MonoBehaviour {


	Controller controller;
	//Vector initCoordiante = new Vector(0, 0, 0);
	//int count = 0; 
	//int countAvg = 0;
	private Vector3 fingerTipPos;
	public List<Vector3> linePoints;
	public List<Vector3> avgLinePoints; 

	int frameCount = 0; 

	const int timeDelay = 1000; //milliseconds

	private const int avgWindow = 5;

	String filePath = "Letters\\X5.csv";
	const char symbol = 'X' ; // 0 means character A, 1 mean B and so on
	const bool recordData = false; // if it is turned on it enters into record mode, else it is in recognize mode
    // manually handle this variable to generate train set and evaluate
    // Also remember to uncomment corresponding 'String data' strings in line 188 and 191

    //Set the below variable to run the program in Manual mode where no ML algorithm is applied
    const bool runManual = false;

    bool serverandClientReady = false;

    public InteractionBox iBox;


	float threeDdistance(Vector A, Vector B)
	{
		return (float) Math.Sqrt ((A.x-B.x)*(A.x-B.x)+(A.y-B.y)*(A.y-B.y)+(A.z-B.z)*(A.z-B.z));
	}



	void Start () {
		controller = new Controller ();

		//String header = "A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, Class"+ Environment.NewLine;  
		//String header = "f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14, f15, f16, f17, f18, f19, f20, f21, f22, f23, f24, f25, f26, f27, f28, f29, f30, f31, f32, f33, f34, f35, class"+ Environment.NewLine;

		//if(recordData)
			//System.IO.File.AppendAllText (filePath, header);

		//Debug.Log ("Let's call the client setup");
		//serverandClientReady = setUpClient ();  // Run the client function to connect with server

	}


	// Update is called once per frame
	void Update () 
	{
		Frame frame = controller.Frame ();

		iBox = frame.InteractionBox;



		//Debug.Log ("Frame id: " + frame.Id + ", timestamp: " + frame.Timestamp + ", hands: " + frame.Hands.Count);

		if (frame.Hands.Count > 0) 
		{
			frameCount++; 
			Leap.Hand hand = frame.Hands [0];

			Vector palmPosition = hand.PalmPosition; //The center position of the palm in millimeters from the Leap Motion Controller origin
			palmPosition.z *= -1;
			palmPosition = iBox.NormalizePoint (palmPosition, false) * 100.0f;  // Multiplied by 100 for a larger scale

			Vector palmNormal = hand.PalmNormal; //The normal vector to the palm
			palmNormal.z *= -1;
			palmNormal = iBox.NormalizePoint (palmNormal, false) * 100.0f;

			//Vector palmVelocity = hand.PalmVelocity; // The rate of change of the palm position in millimeters/second
			//Vector palmDirection = hand.Direction;   // The direction from the palm position toward the fingers



			//String palmInfo = palmPosition + "," + palmNormal + "," + palmVelocity + "," + palmDirection;

			//Debug.Log (palmInfo);

			float confidence = hand.Confidence;  // How confident we are with a given hand pose
			float grabAngle = hand.GrabAngle;    // The angle between the fingers and the hand of a grab hand pose
			float grabStrength = hand.GrabStrength; //The strength of a grab hand pose

			String grabInfo = confidence + "," + grabAngle + "," + grabStrength; //3 features

			float palmWidth = hand.PalmWidth; //The estimated width of the palm when the hand is in a flat position
			float pinchDistance = hand.PinchDistance; // The distance between the thumb and index finger of a pinch hand pose
			float pinchStrength = hand.PinchStrength; // The holding strength of a pinch hand pose

			//LeapQuaternion rotation = hand.Rotation; //	The rotation of the hand as a quaternion


			String pinchInfo = palmWidth + "," + pinchDistance + "," + pinchStrength; //+ ","+ rotation; // 3 features

			//Debug.Log (rotation);


            // Get the fingers
			Finger thumb = hand.Fingers [(int)Finger.FingerType.TYPE_THUMB];  
			Finger index = hand.Fingers [(int)Finger.FingerType.TYPE_INDEX];
			Finger middle = hand.Fingers [(int)Finger.FingerType.TYPE_MIDDLE];
			Finger ring = hand.Fingers [(int)Finger.FingerType.TYPE_RING];
			Finger pinky = hand.Fingers [(int)Finger.FingerType.TYPE_PINKY];

			//Direction info of thumb 
			Vector thumbDir = thumb.Direction; 
			//Direction info of thumb 
			Vector indexDir = index.Direction;
			//Direction info of middle 
			Vector middleDir = middle.Direction; 
			//Direction info of ring 
			Vector ringDir = ring.Direction; 
			//Direction info of pinky 
			Vector pinkyDir = pinky.Direction; 

			// 15 features
			String directionInfo = thumbDir.x + "," + thumbDir.y + "," + thumbDir.z + "," + indexDir.x + "," + indexDir.y + "," + indexDir.z + "," + middleDir.x + "," + middleDir.y + "," + middleDir.z + "," + ringDir.x + "," + ringDir.y + "," + ringDir.z + "," + pinkyDir.x + "," + pinkyDir.y + "," + pinkyDir.z;

            //Check if a particular finger is extended and record thereby
			int thumbIsExtended = (thumb.IsExtended==true)? 1: 0; 
			int indexIsExtended = (index.IsExtended==true)? 1: 0;
			int middleIsExtended = (middle.IsExtended==true)? 1: 0;
			int ringIsExtended = (ring.IsExtended==true)? 1: 0;
			int pinkyIsExtended = (pinky.IsExtended==true)? 1: 0;

            //Store them in a string to write in file later
			String isFingerExtended = thumbIsExtended+","+ indexIsExtended+","+ middleIsExtended+","+ ringIsExtended+","+ pinkyIsExtended; //5

			Vector thumbTipPosition = thumb.TipPosition;
			thumbTipPosition.z *= -1;
			thumbTipPosition = iBox.NormalizePoint (thumbTipPosition, false) * 100.0f;

			Vector indexTipPosition = index.TipPosition;
			indexTipPosition.z *= -1;
			indexTipPosition = iBox.NormalizePoint (indexTipPosition, false) * 100.0f;

			Vector middleTipPosition = middle.TipPosition;
			middleTipPosition.z *= -1;
			middleTipPosition = iBox.NormalizePoint (middleTipPosition, false) * 100.0f;

			Vector ringTipPosition = ring.TipPosition;
			ringTipPosition.z *= -1;
			ringTipPosition = iBox.NormalizePoint (ringTipPosition, false) * 100.0f;

			Vector pinkyTipPosition = pinky.TipPosition;
			pinkyTipPosition.z *= -1;
			pinkyTipPosition = iBox.NormalizePoint (pinkyTipPosition, false) * 100.0f;

			//String tipPositions = thumbTipPosition + "," + indexTipPosition + "," + middleTipPosition + "," + ringTipPosition + "," + pinkyTipPosition;

            //Calculate distance from palm to each finger
			float dThumbTipPalm = threeDdistance(thumbTipPosition, palmPosition);
			float dIndexTipPalm = threeDdistance(indexTipPosition, palmPosition);
			float dMiddleTipPalm = threeDdistance(middleTipPosition, palmPosition);
			float dRingTipPalm = threeDdistance(ringTipPosition, palmPosition);
			float dPinkyTipPalm = threeDdistance(pinkyTipPosition, palmPosition); 

			String tipToPalmDistance = dThumbTipPalm+","+ dIndexTipPalm+","+ dMiddleTipPalm+","+ dRingTipPalm+","+ dPinkyTipPalm; // 5 features

            // Calculate distance between two adjacent fingers
			float dPinkyRing = threeDdistance (pinkyTipPosition, ringTipPosition);
			float dRingMiddle = threeDdistance (ringTipPosition, middleTipPosition);
			float dMiddleIndex = threeDdistance (middleTipPosition, indexTipPosition);
			float dIndexThumb = threeDdistance (indexTipPosition, thumbTipPosition);

			String adjacentTipDistance = dPinkyRing + "," + dRingMiddle + "," + dMiddleIndex + "," + dIndexThumb; // 4 features

			// Uncomment this string when recording
			String data = grabInfo + "," + pinchInfo +"," + directionInfo + "," + isFingerExtended +"," + tipToPalmDistance +"," + adjacentTipDistance + "," + symbol + Environment.NewLine;

			//Uncomment this string for evaluation
			//String data = grabInfo + "," + pinchInfo +"," + directionInfo + "," + isFingerExtended +"," + tipToPalmDistance +"," + adjacentTipDistance + "," + "?" + Environment.NewLine;





			// Append Data to file

			if (recordData == true && frameCount % 15 == 0)
            {   // If record Mode is on append string to file
				 // To avoid writing a load of data write xth frame in file
				System.IO.File.AppendAllText (filePath, data);
				Debug.Log ("Written in file");
				
			} else if(recordData == false){
                String signClass;
                //String fakeData = "1,0.7695882,0,96.70586,78.473,0.8052388,0.3999044,-0.5740219,-0.7145455,-0.09058682,-0.3717137,-0.9239172,0.07098009,-0.2949706,-0.9528663,0.2207028,-0.3609434,-0.906096,-0.2869593,-0.7349882,0.614367,0,1,1,1,0,34.59805,83.41218,91.54636,82.62241,25.24344,80.8365,19.59681,20.53426,66.92521,?";
                if (frameCount % 100 == 0)
                {
                    Debug.Log("Sending the following data to server...");
                    Debug.Log(data);
                    signClass = sendAndReceive(data);
                    Debug.Log("Detected sign - " + signClass);
                }
            }

			// if framecount reaches the max possible integer reset it to zero
			if (frameCount == 2147483647)
				frameCount = 0;




            //Debug.Log (data);

            /*float thumbLength = thumb.Length;
			float indexLength = index.Length;
			float middleLength = middle.Length;
			float ringLength = ring.Length;
			float pinkyLength = pinky.Length;

			Bone thumbMetaCarpal = thumb.bones [(int)Bone.BoneType.TYPE_METACARPAL];
			Bone indexMetaCarpal = index.bones [(int)Bone.BoneType.TYPE_METACARPAL];
			Bone middleMetaCarpal = middle.bones [(int)Bone.BoneType.TYPE_METACARPAL];
			Bone pinkyMetaCarpal = pinky.bones [(int)Bone.BoneType.TYPE_METACARPAL];
			Bone ringMetaCarpal = ring.bones [(int)Bone.BoneType.TYPE_METACARPAL];

			Vector thumbMetaCarpalCenter = thumbMetaCarpal.Center;
			Vector indexMetaCarpalCenter = indexMetaCarpal.Center;
			Vector middleMetaCarpalCenter = middleMetaCarpal.Center;
			Vector pinkyMetaCrpalCenter = pinkyMetaCarpal.Center;
			Vector ringMetaCarpalCenter = ringMetaCarpal.Center;

			String string6 = thumbMetaCarpalCenter +","+ indexMetaCarpalCenter+"," + middleMetaCarpalCenter+"," + pinkyMetaCrpalCenter+"," + ringMetaCarpalCenter;

			Vector thumbMetaCarpalDirection = thumbMetaCarpal.Direction;
			Vector indexMetaCarpalDirection = indexMetaCarpal.Direction;
			Vector middleMetaCarpalDirection = middleMetaCarpal.Direction;
			Vector pinkyMetaCrpalDirection = pinkyMetaCarpal.Direction;
			Vector ringMetaCarpalDirection = ringMetaCarpal.Direction;

			String string7 = thumbMetaCarpalDirection+"," + indexMetaCarpalDirection+"," + middleMetaCarpalDirection+"," + pinkyMetaCrpalDirection+"," + ringMetaCarpalDirection;
			*/

            /*LeapQuaternion thumbRotation = thumb.Rotation;
			LeapQuaternion indexRotation = index.Rotation;
			LeapQuaternion middleRotation = middle.Rotation;
			LeapQuaternion pinkyRotation = pinky.Rotation;
			LeapQuaternion ringRotation = ring.Rotation;*/




            //Debug.Log ("Palm position: "+ hand.PalmPosition+"thmubp: "+thumb.TipPosition+"indexp: "+index.TipPosition+"midp: "+middle.TipPosition+"ringp: "+ring.TipPosition+"pinkyp: "+pinky.TipPosition);

            /*
			// Manual detection of finger signs
			if (recordData == false && runManual == true && frameCount % 50 == 0) {

                if (thumb.IsExtended && !index.IsExtended && !middle.IsExtended && !ring.IsExtended && !pinky.IsExtended) {
					Debug.Log ("Sign of A" + Environment.NewLine);
					//System.Threading.Thread.Sleep(timeDelay);
				} 
				//Letter B: All fingers are extended
				if (thumb.IsExtended && index.IsExtended && middle.IsExtended && ring.IsExtended && pinky.IsExtended) {
					Debug.Log ("Sign of B" + Environment.NewLine);
					//System.Threading.Thread.Sleep(timeDelay);
				} 
				else if (!thumb.IsExtended && index.IsExtended && !middle.IsExtended && !ring.IsExtended && !pinky.IsExtended) {
					Debug.Log ("Sign of D" + Environment.NewLine);
					//System.Threading.Thread.Sleep(timeDelay);
				}
				else if (!thumb.IsExtended && !index.IsExtended && middle.IsExtended && ring.IsExtended && pinky.IsExtended) {
					Debug.Log ("Sign of F" + Environment.NewLine);
					//System.Threading.Thread.Sleep(timeDelay);
				}
				else if (!thumb.IsExtended && !index.IsExtended && !middle.IsExtended && !ring.IsExtended && pinky.IsExtended) {
					Debug.Log ("Sign of I" + Environment.NewLine);
					//System.Threading.Thread.Sleep(timeDelay);
				} else if (thumb.IsExtended && index.IsExtended && !middle.IsExtended && !ring.IsExtended && !pinky.IsExtended) {
					Debug.Log ("Sign of L" + Environment.NewLine);
					//System.Threading.Thread.Sleep(timeDelay);
				} else if (!thumb.IsExtended && index.IsExtended && middle.IsExtended && ring.IsExtended && !pinky.IsExtended) {
					Debug.Log ("Sign of W" + Environment.NewLine);
					//System.Threading.Thread.Sleep(timeDelay);
				} else if (!thumb.IsExtended && index.IsExtended && middle.IsExtended && !ring.IsExtended && !pinky.IsExtended) {
					Debug.Log ("Sign of V" + Environment.NewLine);
					//System.Threading.Thread.Sleep(timeDelay);
				}
				else if (thumb.IsExtended && !index.IsExtended && !middle.IsExtended && !ring.IsExtended && pinky.IsExtended) {
					Debug.Log ("Sign of Y" + Environment.NewLine);
					//System.Threading.Thread.Sleep(timeDelay);
				}
			}*/

        }
    }

	public String sendAndReceive(String data)
	{
		TcpClient myClient = new TcpClient ("127.0.0.1", 9999);
		serverandClientReady = myClient.Connected;
		try{
			Stream s = myClient.GetStream();
			StreamReader sr = new StreamReader(s);
			StreamWriter sw = new StreamWriter(s);
			sw.AutoFlush = true;

			Debug.Log("Sending data to server on port 9999");

			//Console.WriteLine(sr.ReadLine());
			//while(true){
			//Console.Write("Name: ");
			//string name = Console.ReadLine();
			sw.WriteLine(data);
			//if(name == "") break;
			//Console.WriteLine(sr.ReadLine());
			String feedback = sr.ReadLine();
			//Debug.Log(sr.ReadLine());
			//}
			s.Close();



			return feedback; 
		}
		catch(Exception e){
			// code in finally block is guranteed 
			// to execute irrespective of 
			// whether any exception occurs or does 
			// not occur in the try block
			//myClient.Close();
			return ("Exception: "+ e);
		} 

		finally
		{
			myClient.Close();
		}

	}



}

