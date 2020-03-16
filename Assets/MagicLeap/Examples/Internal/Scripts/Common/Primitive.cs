// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019-present, Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Developer Agreement, located
// here: https://auth.magicleap.com/terms/developer
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine;

namespace MagicLeap
{
    /// <summary>
    /// Simple interface to Magic Leap's primitive to ensure all visual augmentations are consistant to the visual assets
    /// </summary>
    public class Primitive : MonoBehaviour
    {
        private const float _expansionAmount = 1.3f;
        private Material _innerGlass;
        private Transform _expansionPiece;
        private Color _initialColor;
        private Color _initialEmission;

        private const string _emissionProperty = "_EmissionColor";

        void Awake()
        {
            //get pieces and hook them to references:
            foreach (Transform item in transform)
            {
                //find inner glass:
                if (item.name.Contains("InnerGlass"))
                {
                    _innerGlass = item.GetComponent<Renderer>().material;
                    _initialColor = _innerGlass.color;
                    _initialEmission = _innerGlass.GetColor(_emissionProperty);
                }

                //find expansion piece:
                if (item.name.Contains("Plastic"))
                {
                    _expansionPiece = item;
                }
            }
        }

        public void Expand()
        {
            _expansionPiece.localScale = Vector3.one * _expansionAmount;
        }

        public void Contract()
        {
            _expansionPiece.localScale = Vector3.one;
        }

        public void ChangeColor(Color color)
        {
            _innerGlass.color = color;
        }

        public void ChangeEmission(Color color)
        {
            _innerGlass.SetColor(_emissionProperty, color);
            _innerGlass.EnableKeyword("_EMISSION");
        }

        public void ResetColor()
        {
            _innerGlass.color = _initialColor;
        }

        public void ResetEmission()
        {
            _innerGlass.SetColor(_emissionProperty, _initialEmission);
        }
    }
}
