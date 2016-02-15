using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class UIMapper : MonoBehaviour {

	[SerializeField] ProceduralVolumetricFire fire;
	[SerializeField] Slider lacunaritySlider;
	[SerializeField] Slider gainSlider;
	[SerializeField] Slider magnitudeSlider;
	[SerializeField] Slider attenuationSlider;

	void Update () {
		fire.lacunarity = lacunaritySlider.value;
		fire.gain = gainSlider.value;
		fire.magnitude = magnitudeSlider.value;
		fire.attenuation = attenuationSlider.value;
	}

}
