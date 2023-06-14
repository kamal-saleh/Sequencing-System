using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    None,
    PlayAnimation,
    PlayAudioClip,
    EnableGameObject,
    DisableGameObject,
    MoveCameraToPosition
}

[System.Serializable]
public class Action
{
    public ActionType actionType;

    // Optional parameters needed for specific actions.
    // For example, if action type is play animation then animation name would need specified here
    public string parameter1;

    // If true wait before proceeding with next action 
    public bool waitForCompletion;
}
