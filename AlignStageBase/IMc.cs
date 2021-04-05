using System;
using System.Collections.Generic;

namespace Free302.TnM.Device
{
	public interface IMc : IDevice<McId>
    {
		/// <summary>
		/// list of McAxis currently connected 
		/// </summary>
		List<McAxis> EffectiveAxis { get; }

		bool isMoving(McAxis axis = McAxis.All);

		Dictionary<McAxis, double> getPosition(McAxis axis = McAxis.All);


        void SaveDynamicData();
        void SaveDynamicData(McAxis mcAxis);

        /// <summary>
        /// wait(async) the axis stop and reset the logical position
        /// </summary>
        /// <param name="newPosition">새 위치 - 외부 단위 um, degree</param>
        /// <param name="axis"></param>
        void resetPosition(McAxis axis = McAxis.All);

		void readMotionStatus(McMotionParam param);	//read drive status of param.axis

		void reportStatusAsync(McMotionParam param, IProgress<McMotionParam> reporter);
		void reportStatusAsync(McMotionParam[] paramList, IProgress<McMotionParam> reporter);

		void move(McMotionParam param);
		void moveToLogicalOrigin(McAxis axis = McAxis.All);		//move to logical 0 position
		void moveToOrigin(McAxis axis = McAxis.All);	//move to HW origin detected by ORG
		void moveToStrokeCenter(McAxis axis = McAxis.All);	//move to the center of stroke
		void stop(McAxis axis = McAxis.All);

		int getSpeedValue(McAxis axis, McSpeed speed);

		StageBase getStage(McAxis mcAxis);


		//
		//---- 테스트 지원 methods ----
		//
		void setSpeedValue(McAxis axis, int speedValue);//fast speed value
		void setReportDelay(McAxis mcAxis, int delayTime);


		//McPool 지원
		void OpenBase();
		void CloseBase();

	}
}
