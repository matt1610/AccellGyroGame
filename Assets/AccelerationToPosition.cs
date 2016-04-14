// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
namespace AssemblyCSharp
{
	public class AccelerationToPosition
	{

		public int Sample_X;
		public int Sample_Y;
		byte Sample_Z;
		public long[] Sensor_Data = new long[8];
		byte countx;
		byte county;
		int[] accelerationx = new int[2];
		int[] accelerationy = new int[2];
		int[] velocityx = new int[2];
		int[] velocityy = new int[2];
		int[] positionX = new int[2];
		int[] positionY = new int[2];
		int[] positionZ = new int[2];
		int direction;
		long sstatex;
		long sstatey;

		public AccelerationToPosition ()
		{
		}

		void init()
		{

		} // End init


		void SetAccellData()
		{
			Sample_X = 0;
			Sample_Y = 0;
		}


		/*******************************************************************************
		 * The purpose of the calibration routine is to obtain the value of the reference 
		 * threshold. 
		 * It consists on a 1024 samples average in no-movement condition.
		********************************************************************************/

		void Calibrate()
		{
			uint count1;
			count1 = 0;

			do{
//				ADC_GetAllAxis();
//				SetAccellData();
				sstatex = sstatex + Sample_X;
				sstatey = sstatey + Sample_Y;
				count1++;
			}while(count1!=0x0400);

			sstatex = sstatex>>10;
			sstatey = sstatey>>10;

		} // End Calibrate

		public void DoOneCalibration(long sampleX, long sampleY) 
		{
			sstatex = sstatex + sampleX;
			sstatey = sstatey + sampleY;
		}

		public void EndCalibration() 
		{
			sstatex = sstatex>>10;
			sstatey = sstatey>>10;
		}




		/*******************************************************************************************
		 * This function obtains magnitude and direction
		 * In this particular protocol direction and magnitude are sent in separate variables. 
		 * Management can be done in many other different ways. 
		 *****************************************************************************************/

		void data_transfer()
		{
			long positionXbkp;
			long positionYbkp;
			uint delay;
			long[] posx_seg = new long[4];
			long[] posy_seg = new long[4];
			if (positionX[1]>=0) {              //This line compares the sign of the X direction data
				direction= (direction | 0x10);      //if its positive the most significant byte
				posx_seg[0]= positionX[1] & 0x000000FF;     // is set to 1 else it is set to 8
				posx_seg[1]= (positionX[1]>>8) & 0x000000FF; // the data is also managed in the
				// subsequent lines in order to
				posx_seg[2]= (positionX[1]>>16) & 0x000000FF;  // be sent. The 32 bit variable must be
				posx_seg[3]= (positionX[1]>>24) & 0x000000FF;  // split into 4 different 8 bit
				// variables in order to be sent via
				// the 8 bit SCI frame
			}
			else {
				direction=(direction | 0x80);
				positionXbkp = positionX[1]-1;
				positionXbkp = positionXbkp^0xFFFFFFFF;
				posx_seg[0] = positionXbkp & 0x000000FF;
				posx_seg[1] = (positionXbkp>>8) & 0x000000FF;
				posx_seg[2] = (positionXbkp>>16) & 0x000000FF;
				posx_seg[3] = (positionXbkp>>24) & 0x000000FF;
			}
			if (positionY[1]>=0) {                      //  Same management than in the previous case
				direction = (direction | 0x08);        // but with the Y data.
				posy_seg[0] = positionY[1] & 0x000000FF;
				posy_seg[1] = (positionY[1]>>8) & 0x000000FF;
				posy_seg[2] = (positionY[1]>>16) & 0x000000FF;
				posy_seg[3] = (positionY[1]>>24) & 0x000000FF;
			}
			else {
				direction= (direction | 0x01);
				positionYbkp = positionY[1]-1;
				positionYbkp = positionYbkp^0xFFFFFFFF;
				posy_seg[0] = positionYbkp & 0x000000FF;
				posy_seg[1] = (positionYbkp>>8) & 0x000000FF;
				posy_seg[2] = (positionYbkp>>16) & 0x000000FF;
				posy_seg[3] = (positionYbkp>>24) & 0x000000FF;
			}
			delay = 0x0100;
			Sensor_Data[0] = 0x03;
			Sensor_Data[1] = direction;
			Sensor_Data[2] = posx_seg[3];
			Sensor_Data[3] = posy_seg[3];
			Sensor_Data[4] = 0x01;
			Sensor_Data[5] = 0x01;
//			Sensor_Data[6] = END_OF_FRAME;
			Sensor_Data[6] = 1;

//			while (--delay);

//			SCITxMsg(Sensor_Data);
//
//			while (SCIC2 & 0x08);

		} // End data_transfer



		/****************************************************************************************** 
		 * This function returns data format to its original state. When obtaining the magnitude 
		 * and direction of the position, an inverse two's complement is made. This function makes 
		 * the two's complement in order to return the data to it original state. 
		 * It is important to notice that the sensibility adjustment is greatly impacted here, the amount of "ones"
		 * inserted in the mask must be equivalent to the "ones" lost in the shifting made in the 
		 * previous function upon the sensibility modification.
		******************************************************************************************/


//		void data_reintegration()
//		{
//			if (direction >=10)
//			{
//				positionX[1]= positionX[1]|0xFFFFC000;
//			}
//
//			//amount of shifts
//			direction = direction & 0x01;
//
//			if (direction ==1)
//			{
//				positionY[1]= positionY[1]|0xFFFFC000;
//			}
//		}

		void concatenate_data()
		{
			
		} //End concatenate_data



		/****************************************************************************************** 
		 * This function allows movement end detection. If a certain number of acceleration samples 
		 * are equal to zero we can assume movement has stopped. Accumulated Error generated in the 
		 * velocity calculations is eliminated by resetting the velocity variables. This stops 
		 * position increment and greatly eliminates position error. 
		 ******************************************************************************************/

		void movement_end_check()
		{
			if (accelerationx[1]==0)
			{ 
				countx++;
			} else 
			{ 
				countx =0;
			}
			if (countx>=25)
			{
				velocityx[1]=0;
				velocityx[0]=0;
			}
			if (accelerationy[1]==0)
			{ 
				county++;
			}
			else 
			{ 
				county =0;
			}
			if (county>=25)
			{
				velocityy[1]=0;
				velocityy[0]=0;
			}
		} // End movement_end_check





		/****************************************************************************************** 
		 * This function transforms acceleration to a proportional position by integrating the 
		 * acceleration data twice. It also adjusts sensibility by multiplying the "positionX" 
		 * and "positionY" variables.
		 * This integration algorithm carries error, which is compensated in the 
		 * "movenemt_end_check" subroutine. Faster sampling frequency implies less error but 
		 * requires more memory. Keep in mind that the same process is applied to the X and Y axis.
		 *****************************************************************************************/


		void position()
		{
			byte count2;
			count2 = 0;
			do{
//				ADC_GetAllAxis();
//				SetAccellData();
				accelerationx[1]=accelerationx[1] + Sample_X; //filtering routine for noise attenuation
				accelerationy[1]=accelerationy[1] + Sample_Y; //64 samples are averaged. The resulting
				//average represents the acceleration of
				//an instant
				count2++;
			} while (count2!=0x40);

			accelerationx[1]= accelerationx[1]>>6;
			accelerationy[1]= accelerationy[1]>>6;
			// 64 sums of the acceleration sample
			// division by 64
			//accelerationx[1] = accelerationx[1] - (int); //eliminating zero reference
			accelerationx[1] = accelerationx[1];
			//offset of the acceleration data
			accelerationy[1] = accelerationy[1] - (int)sstatey; // to obtain positive and negative
			//acceleration
			if ((accelerationx[1] <=3)&&(accelerationx[1] >= -3)) //Discrimination window applied
			{
				accelerationx[1] = 0;
			}                        // to the X axis acceleration
			//variable
			if ((accelerationy[1] <=3)&&(accelerationy[1] >= -3))
			{
				accelerationy[1] = 0;
			}
			//first X integration:
			velocityx[1]= velocityx[0]+ accelerationx[0]+ ((accelerationx[1] -accelerationx[0])>>1);
			//second X integration:
			positionX[1]= positionX[0] + velocityx[0] + ((velocityx[1] - velocityx[0])>>1);
			//first Y integration:
			velocityy[1] = velocityy[0] + accelerationy[0] + ((accelerationy[1] -accelerationy[0])>>1);
			//second Y integration:
			positionY[1] = positionY[0] + velocityy[0] + ((velocityy[1] - velocityy[0])>>1);
			accelerationx[0] = accelerationx[1];  //The current acceleration value must be sent
			//to the previous acceleration
			accelerationy[0] = accelerationy[1];
			//acceleration value.
			velocityx[0] = velocityx[1];
			velocityy[0] = velocityy[1];
			positionX[1] = positionX[1]<<18;
			//is a sensibility adjustment.
			positionY[1] = positionY[1]<<18;
			//particular situation

			data_transfer();

			positionX[1] = positionX[1]>>18;
			positionY[1] = positionY[1]>>18;

			movement_end_check();

			positionX[0] = positionX[1];
			positionY[0] = positionY[1];
			//variable in order to introduce the new
			//Same done for the velocity variable
			//The idea behind this shifting (multiplication)
			//Some applications require adjustments to a
			//i.e. mouse application
			//once the variables are sent them must return to
			//their original state
			//actual position data must be sent to the
			//previous position
			direction = 0;
		} // End Position

		void main()
		{
//			init ();
//
//			do {
//				position ();
//			} while (1);

		} // End Main






	}
}


























