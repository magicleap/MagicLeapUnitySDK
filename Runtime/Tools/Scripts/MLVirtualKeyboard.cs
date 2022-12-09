// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2021-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// This class provides callbacks and manages the state of the Virtual Keyboard.
    /// </summary>
    public class MLVirtualKeyboard : MonoBehaviour
    {
        [System.Serializable]
        public class KeyboardCancelEvent : UnityEvent { }

        [System.Serializable]
        public class KeyboardSubmitEvent : UnityEvent<string> { }

#pragma warning disable 414
        //[SerializeField, Tooltip("A reference to the controller connection handler in the scene.")]
        // private MLControllerConnectionHandlerBehavior _controllerConnectionHandler = null;
#pragma warning restore 414

        [SerializeField, Tooltip("The preview field for the typed text.")]
        private Text _inputField = null;

        [Header("Keyboard Layouts")]

        [SerializeField, Tooltip("The GameObject for the lowercase version of the keyboard.")]
        private GameObject _lowercaseKeyboard = null;

        [SerializeField, Tooltip("The GameObject for the uppercase version of the keyboard.")]
        private GameObject _uppercaseKeyboard = null;

        [SerializeField, Tooltip("The GameObject for the numeric version of the keyboard.")]
        private GameObject _numericKeyboard = null;

        [SerializeField, Tooltip("The GameObject for the numeric (shift) version of the keyboard.")]
        private GameObject _numericShiftKeyboard = null;

        [Space, Tooltip("The event that occurs when the keyboard is canceled.")]
        public KeyboardCancelEvent OnKeyboardCancel = new KeyboardCancelEvent();

        [Space, Tooltip("The event that occurs when the keyboard is submitted.")]
        public KeyboardSubmitEvent OnKeyboardSubmit = new KeyboardSubmitEvent();

        [Space, Tooltip("The event that occurs when the input value changes. The parameter includes the full text of the input field.")]
        public UnityEvent<string> OnInputValueChange = new UnityEvent<string>();

        [Space, Tooltip("The event that occurs when a character is added.")]
        public UnityEvent<char> OnCharacterAdded = new UnityEvent<char>();

        [Space, Tooltip("The event that occurs when a character is deleted.")]
        public UnityEvent OnCharacterDeleted = new UnityEvent();

        private bool _shift = false;
        private bool _alternate = false;
        private InputField targetInput = null;

        /// <summary>
        /// Appends a string to the end of the input field text.
        /// </summary>
        /// <param name="character"></param>
        public void InsertCharacter(string character)
        {
            if (character.Length > 0)
            {
                _inputField.text += character;
                OnInputValueChange?.Invoke(_inputField.text);
                for (int i = 0; i < character.Length; ++i)
                {
                    OnCharacterAdded?.Invoke(character[i]);
                }
            }
        }

        /// <summary>
        /// Toggles the active keyboard between (a-Z) and (Alphanumeric)
        /// </summary>
        public void ToggleKeyboard()
        {
            _alternate = !_alternate;

            UpdateKeyboard();
        }

        /// <summary>
        /// Toggles the shift state of the currently active keyboard.
        /// </summary>
        public void ToggleShift()
        {
            _shift = !_shift;

            UpdateKeyboard();
        }

        /// <summary>
        /// If a ControllerConnectionHandler is assigned, the Bump feedback pattern will be sent to the active controller.
        /// </summary>
        public void Hover()
        {
            //if (_controllerConnectionHandler != null && _controllerConnectionHandler.ConnectedController != null)
            //{
            //    _controllerConnectionHandler.ConnectedController.StartFeedbackPatternVibe(MLInput.Controller.FeedbackPatternVibe.Bump, MLInput.Controller.FeedbackIntensity.Low);
            //}
        }

        /// <summary>
        /// Deletes the last element from the input field text.
        /// </summary>
        public void Delete()
        {
            OnCharacterDeleted?.Invoke();

            if (_inputField.text.Length <= 0)
            {
                return;
            }

            _inputField.text = _inputField.text.Remove(_inputField.text.Length - 1);
            OnInputValueChange?.Invoke(_inputField.text);
        }

        /// <summary>
        /// Appends a space to the end of the input field text.
        /// </summary>
        public void Space()
        {
            _inputField.text += " ";
            OnInputValueChange?.Invoke(_inputField.text);
            OnCharacterAdded?.Invoke(' ');
        }

        /// <summary>
        /// Adds a NewLine to the input field text.
        /// </summary>
        public void Return()
        {
            _inputField.text += System.Environment.NewLine;
            OnInputValueChange?.Invoke(_inputField.text);
            for (int i = 0; i < System.Environment.NewLine.Length; ++i)
            {
                OnCharacterAdded?.Invoke(System.Environment.NewLine[i]);
            }
        }

        public void Open(InputField targetField = null)
        {
            // Clear any existing strings.
            _inputField.text = string.Empty;

            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }

            targetInput = targetField;
        }

        /// <summary>
        /// Invokes the Cancel event for the Virtual Keyboard.
        /// </summary>
        public void Cancel()
        {
            targetInput = null;
            OnKeyboardCancel?.Invoke();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Invokes the Submit event for the Virtual Keyboard.
        /// </summary>
        public void Submit()
        {
            if (targetInput != null)
            {
                if (targetInput.onEndEdit.GetPersistentEventCount() == 0)
                {
                    // just set the text directly and let it be handled later
                    targetInput.text = _inputField.text;
                }
                else
                {
                    // ensure that the end edit event is invoked
                    // this sets the input field's text member as well 
                    targetInput.onEndEdit.Invoke(_inputField.text);
                }
            }
            OnKeyboardSubmit?.Invoke(_inputField.text);
            targetInput = null;
            gameObject.SetActive(false);
        }

        private void UpdateKeyboard()
        {
            // a-Z Keyboards
            _lowercaseKeyboard.SetActive(!_alternate && !_shift);
            _uppercaseKeyboard.SetActive(!_alternate && _shift);

            // Alphanumeric Keyboards
            _numericKeyboard.SetActive(_alternate && !_shift);
            _numericShiftKeyboard.SetActive(_alternate && _shift);
        }
    }
}
