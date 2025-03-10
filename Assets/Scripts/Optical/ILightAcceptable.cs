using System.Collections.Generic;
using UnityEngine;

namespace Phos.Optical {
	public interface ILightAcceptable {
		/// <summary>
		/// if income can continue go on, return true
		/// or else, return false
		/// </summary>
		bool OnLightHitted(LightData income, RaycastHit hit, out List<LightData> outgo);
	}
}