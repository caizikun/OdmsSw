using System;
namespace Free302.TnM.Device
{
	/// <summary>
	/// the input & output prarameters of move()
	/// </summary>
	interface IMcMotionParam
	{
		//axis to move/read
		McAxis axis { get; set; }
        
		//coordinates
		double x { get; set; }				//current position
		double x0 { get; set; }			//initial position
		void initCoord(double x0, double x);	//set coordinates with status change


		//displacements
		double dx { get; }					//current displacement
		double dx0 { get; set; }			//target displacement
		McMoveType moveType { get; set; }
		int speed { get; set; }
		
		//sensor config
		bool senseNORG { get; set; }		//sense NORG signal
		bool senseORG { get; set; }			//sense ORG signal
		McSensorConfig sensor { get; set; }	//sensor configuration


		//status
		McMoveStatus status { get; }
		bool isComplete { get; }		//dx == dx0
		bool isError { get; set; }		//HW Limit, Emergent stop
		bool isNorg { get; set; }		//NORG detected
		bool isOrg { get; set; }		//ORG detected
		bool isStop { get; set; }		//NORG, ORG

		void resetAll();					//clear all status & coordinates
		void resetStatus();					//clear all status
		string ToString();
	}
}
