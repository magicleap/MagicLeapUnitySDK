using System;

namespace MagicLeap.OpenXR.Features.FacialExpressions
{
    /// <summary>
    /// The type of facial expression blend shape.
    /// </summary>
    public enum FacialBlendShape
    {
        BrowLowererL = 0,
        BrowLowererR,
        CheekRaiserL,
        CheekRaiserR,
        ChinRaiser,
        DimplerL,
        DimplerR,
        EyesClosedL,
        EyesClosedR,
        InnerBrowRaiserL,
        InnerBrowRaiserR,
        JawDrop,
        LidTightenerL,
        LidTightenerR,
        LipCornerDepressorL,
        LipCornerDepressorR,
        LipCornerPullerL,
        LipCornerPullerR,
        LipFunnelerLB,
        LipFunnelerLT,
        LipFunnelerRB,
        LipFunnelerRT,
        LipPressorL,
        LipPressorR,
        LipPuckerL,
        LipPuckerR,
        LipStretcherL,
        LipStretcherR,
        LipSuckLB,
        LipSuckLT,
        LipSuckRB,
        LipSuckRT,
        LipTightenerL,
        LipTightenerR,
        LipsToward,
        LowerLipDepressorL,
        LowerLipDepressorR,
        NoseWrinklerL,
        NoseWrinklerR,
        OuterBrowRaiserL,
        OuterBrowRaiserR,
        UpperLidRaiserL,
        UpperLidRaiserR,
        UpperLipRaiserL,
        UpperLipRaiserR,
        TongueOut
    }

    /// <summary>
    /// Flags that determine if a blend shape is considered valid and/or tracked.
    /// </summary>
    [Flags]
    public enum BlendShapePropertiesFlags
    {
        None = 0,
        ValidBit = 1 << 0,
        TrackedBit = 1 << 1
    }

    /// <summary>
    /// The data properties associated with a given blend shape obtained by the Facial Expressions API.
    /// </summary>
    public struct BlendShapeProperties
    {
        /// <summary>
        /// The type of facial expression blend shape.
        /// </summary>
        public FacialBlendShape FacialBlendShape;

        /// <summary>
        /// A value between 0 and 1 that states the current weight of the blend shape property.
        /// </summary>
        public float Weight;

        /// <summary>
        /// Flags which indicate if the blend shape property is valid and/or tracked.
        /// </summary>
        public BlendShapePropertiesFlags Flags;
    }
}
