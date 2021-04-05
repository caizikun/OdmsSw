using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Free302.MyLibrary;

namespace Free302.TnM.Device
{

	#region === Data Type Definition ===

	public enum AlignStageId { None = 0, AS1, AS2, AS3, AS4, AS5, AS6 }

	/// <summary>
	/// 6축 정의
	/// 시야 = 정면
	/// +방향은 CoordinatesChirality에 의해 결정됨
	/// </summary>
	[Flags]
	public enum AlignAxis
	{
		None = 0,
		X = 0x01,	//앞뒤 방향
		Y = 0x02,	//위아래
		Z = 0x04,	//좌우
		Tx = 0x08, //X축에 대한 회전
		Ty = 0x10, //Y축에 대한 회전
		Tz = 0x20,	//Z축에 대한 회전
		All = X | Y | Z | Tx | Ty | Tz
	}

	/// <summary>
	/// 좌표계의 방향성 - 각 축의 + 방향을 결정
	/// </summary>
	public enum CoordinatesChirality
	{
		Right,	//오른손 좌표계
		Left	//왼존 좌표계 : Z, Tx, Ty, Tz 의 +방향이 오른손 좌표계와 반대
	}

	#endregion 

	




}
