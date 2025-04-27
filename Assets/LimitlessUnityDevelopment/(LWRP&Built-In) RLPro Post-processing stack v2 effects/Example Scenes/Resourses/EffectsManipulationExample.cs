using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class EffectsManipulationExample : MonoBehaviour
{
	// Volume where effect is;
	public PostProcessVolume volume;
	RLProGlitch1 m_Glitch1 = null;

	private void Start()
	{
		if (volume == null)
			return;
		volume.profile.TryGetSettings(out m_Glitch1); // Get efffect from pp volume

		if (m_Glitch1)
			m_Glitch1.enabled.value = true;
		else
			Debug.Log("No glitch1 effect found!");
	}
	void Update()
	{
		//Manipulate effect here;
		if (m_Glitch1)
			m_Glitch1.amount.value = UnityEngine.Random.Range(0f, 2f);
	}
}