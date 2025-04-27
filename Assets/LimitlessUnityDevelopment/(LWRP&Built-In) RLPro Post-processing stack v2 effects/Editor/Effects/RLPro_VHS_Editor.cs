using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.PostProcessing;

namespace UnityEditor.Rendering.PostProcessing
{
    [PostProcessEditor(typeof(RLProVHSEffect))]
    internal sealed class RLPro_VHS_Editor : PostProcessEffectEditor<RLProVHSEffect>
    {

        SerializedParameterOverride colorOffset;
        SerializedParameterOverride colorOffsetAngle;
        SerializedParameterOverride verticalOffsetFrequency;
        SerializedParameterOverride verticalOffset;
		SerializedParameterOverride m_Mask;
		SerializedParameterOverride m_MaskMode;

		SerializedParameterOverride offsetDistortion;
        SerializedParameterOverride noiseTexture;
        SerializedParameterOverride blendMode;
        SerializedParameterOverride tile;
        SerializedParameterOverride _textureIntensity;
		SerializedParameterOverride smoothCut;
		SerializedParameterOverride iterations;
		SerializedParameterOverride smoothSize;
		SerializedParameterOverride deviation;
		SerializedParameterOverride _textureCutOff;
		SerializedParameterOverride stripes;
		SerializedParameterOverride unscaledTime;

		//bool laal;

		public override void OnEnable()
		{
			colorOffset = FindParameterOverride(x => x.colorOffset);
			colorOffsetAngle = FindParameterOverride(x => x.colorOffsetAngle);
			verticalOffsetFrequency = FindParameterOverride(x => x.verticalOffsetFrequency);
			verticalOffset = FindParameterOverride(x => x.verticalOffset);
			offsetDistortion = FindParameterOverride(x => x.offsetDistortion);
			noiseTexture = FindParameterOverride(x => x.noiseTexture);
			blendMode = FindParameterOverride(x => x.blendMode);
			tile = FindParameterOverride(x => x.tile);
			m_Mask = FindParameterOverride(x => x.mask);
			m_MaskMode = FindParameterOverride(x => x.maskChannel);

			_textureIntensity = FindParameterOverride(x => x._textureIntensity);
			smoothCut = FindParameterOverride(x => x.smoothCut);
			iterations = FindParameterOverride(x => x.iterations);
			smoothSize = FindParameterOverride(x => x.smoothSize);
			deviation = FindParameterOverride(x => x.deviation);
			_textureCutOff = FindParameterOverride(x => x._textureCutOff);
			stripes = FindParameterOverride(x => x.stripes);
			unscaledTime = FindParameterOverride(x => x.unscaledTime);

		}

		public override void OnInspectorGUI()
        {
            PropertyField(colorOffset);
            PropertyField(colorOffsetAngle);

			PropertyField(verticalOffsetFrequency);
			PropertyField(verticalOffset);
			PropertyField(offsetDistortion);
			PropertyField(noiseTexture);
			PropertyField(blendMode);
			PropertyField(tile);
			PropertyField(_textureIntensity);
			PropertyField(smoothCut);

			if (smoothCut.value.boolValue == true)
            {
                PropertyField(iterations);
                PropertyField(smoothSize);
                PropertyField(deviation);
            }
			PropertyField(_textureCutOff);

			PropertyField(stripes);
			PropertyField(unscaledTime);
			PropertyField(m_Mask);
			PropertyField(m_MaskMode);


		}
	}
}
