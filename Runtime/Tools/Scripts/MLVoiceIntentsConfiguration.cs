using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MLVoiceIntentsConfiguration", menuName = "Magic Leap/Voice Intents Configuration", order = 1)]
/// <summary>
/// A Class that will maintain the proper JSON structure needed by the MLVoice API.
/// </summary>
public class MLVoiceIntentsConfiguration : ScriptableObject
{
    [System.Serializable]
    /// <summary>
    /// The current structure of the JSON data that will be sent to the MLVoice API. Subject to change.
    /// </summary>
    /// 
    public struct JSONData
    {
        [SerializeField]
        public string name;
        [SerializeField]
        public string id;
        [SerializeField]
        public string value;
    }

    [System.Serializable]
    /// <summary>
    /// The current structure of the System Intents JSON data that will be sent to the MLVoice API. Subject to change.
    /// </summary>
    public struct SystemJSONData
    {
        [SerializeField]
        public List<string> name;
    }

    [System.Serializable]
    /// <summary>
    /// The Simplified Voice Command data needed to add commands from the inspector.
    /// The unique name field required in the final JSON will be auto generated based on the unique Id.
    /// </summary>
    public struct CustomVoiceIntents
    {
        [SerializeField]
        [Tooltip("Must Be Unique.")]
        public uint Id;
        [SerializeField]
        [Tooltip("The command to be spoken. Can have 2 phrases seperated by an | to indicate multiple commands triggering the same event.")]
        public string Value;
    }

    [System.Serializable]
    /// <summary>
    /// The container for the data being changed into a JSON string to send to the MLVoice API.
    /// </summary>
    public class JSONContainer
    {
        [SerializeField]
        public List<JSONData> app_intents;
        [SerializeField]
        public SystemJSONData sys_intent_list;
    }

    private JSONContainer container;

    /// <summary>
    /// Used to auto create a unique name field for the final JSON data with the Unique ID provided in the list VoiceCommandsToAdd.
    /// </summary>
    private string autoVoiceIntentName = "UnityApp_VoiceIntent_ID";

    [HideInInspector]
    /// <summary>
    /// The final data properly laid out to be assigned to the container that will be sent to the MLVoice API.
    /// VoiceCommandsToAdd will automatically be added to this in the proper format. Can also manually add to this list if
    /// it is not desired to do this through the editor inspector. This list follows the current structure of the JSONData
    /// and is subject to change.
    /// </summary>
    public List<JSONData> AllVoiceIntents;

    /// <summary>
    /// The simplified list of Voice Command data to be filled out in the inspector or manually added to. A unique name field will be created
    /// based on the Unique Id provided.
    /// 
    /// If GetJSONString or GetValues is called, this data will be altered to fit the current proper layout the API needs to function correctly
    /// and will be added to AllVoiceIntents, then this list will be cleared as these commands will no longer need to be added.
    /// </summary>
    public List<CustomVoiceIntents> VoiceCommandsToAdd;


    public string GetJSONString()
    {
        setupJSONContainer();
        return JsonUtility.ToJson(container);
    }

    /// <summary>
    /// Retrieve a list of the current list of voice commands as a string.
    /// </summary>
    public List<string> GetValues()
    {
        setupJSONContainer();
        List<string> values = new List<string>();

        for (int i = 0; i < container.app_intents.Count; i++)
        {
            values.Add(container.app_intents[i].value);
        }

        return values;
    }

    private void setupJSONContainer()
    {
        if (container == null)
        {
            container = new JSONContainer();
        }

        if (VoiceCommandsToAdd.Count > 0)
        {
            addCustomVoiceCommands();
        }

        ValidationCheck();

        container.app_intents = new List<JSONData>();
        container.app_intents.AddRange(AllVoiceIntents);
        container.sys_intent_list = new SystemJSONData();
        container.sys_intent_list.name = new List<string>();
    }

    private void addCustomVoiceCommands()
    {
        foreach (CustomVoiceIntents customCommand in VoiceCommandsToAdd)
        {
            JSONData newData = new JSONData();
            newData.name = autoVoiceIntentName + customCommand.Id.ToString();
            newData.id = customCommand.Id.ToString();
            newData.value = customCommand.Value;

            AllVoiceIntents.Add(newData);
        }

        VoiceCommandsToAdd.Clear();
    }

    private void ValidationCheck()
    {
        List<string> Ids = new List<string>();
        List<string> Values = new List<string>();
        foreach (JSONData data in AllVoiceIntents)
        {
            Ids.Add(data.id);
            if (data.value.Contains("|"))
            {
                string[] splitValues = data.value.Split("|", StringSplitOptions.RemoveEmptyEntries);
                foreach (string split in splitValues)
                {
                    string final = split.Trim();
                    final = final.ToLower();
                    Values.Add(final);
                }
            }
            else
            {
                Values.Add((data.value.Trim()).ToLower());
            }
        }

        var unique_ids = new HashSet<string>(Ids);
        var unique_values = new HashSet<string>(Values);

        bool idsUnique = unique_ids.Count == Ids.Count;
        bool valuesUnique = unique_values.Count == Values.Count;

        if (!idsUnique || !valuesUnique)
        {
            Debug.LogError("The Voice Intents Configuration File has detected duplicate Values or Ids used. This could cause unwanted behaviour.");
        }
    }
}
