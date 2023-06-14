using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceManager : MonoBehaviour
{
    [SerializeField] private List<Action> sequenceActions = new List<Action>();
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private GameObject[] gameObjects;
    [SerializeField] private Camera[] cameras;
    [SerializeField] private float duration;

    void Start()
    {
        StartSequence();
    }

    public void StartSequence()
    {
        StartCoroutine(ExecuteSequence());
    }

    IEnumerator ExecuteSequence()
    {
        foreach (Action action in sequenceActions)
        {
            switch (action.actionType)
            {

                case ActionType.PlayAnimation:
                    if (animator != null)
                    {
                        string animationName = action.parameter1 ?? "";
                        if (!string.IsNullOrEmpty(animationName))
                            animator.Play(animationName);

                        if (action.waitForCompletion)
                            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
                    }

                    break;

                case ActionType.EnableGameObject:
                    if (!string.IsNullOrEmpty(action.parameter1))
                    {
                        foreach (GameObject go in gameObjects)
                        {
                            if (go.name == action.parameter1)
                            {
                                go.SetActive(true);
                                break; // Stop searching as we have found our target object
                            }
                        }
                        if (action.waitForCompletion)
                            yield return new WaitForSeconds(1);
                    }
                    break;

                case ActionType.DisableGameObject:
                    if (!string.IsNullOrEmpty(action.parameter1))
                    {
                        foreach (GameObject go in gameObjects)
                        {
                            if (go.name == action.parameter1)
                            {
                                go.SetActive(false);
                                break; // Stop searching as we have found our target object
                            }
                        }
                        if (action.waitForCompletion)
                            yield return new WaitForSeconds(1);
                    }
                    break;

                case ActionType.MoveCameraToPosition:
                    if (cameras != null && cameras.Length > 0)
                    {
                        Camera mainCam = cameras[0];
                        Vector3 targetPosition = ParseVector3(action.parameter1);
                        Debug.Log(targetPosition);

                        float elapsedTime = 0f;
                        while (elapsedTime < duration)
                        {
                            elapsedTime += Time.deltaTime;
                            float t = Mathf.Clamp01(elapsedTime / duration);
                            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, targetPosition, t);
                            yield return null;
                        }

                        mainCam.transform.position = targetPosition;

                        if (Vector3.Distance(mainCam.transform.position, targetPosition) <= 0.01f)
                                yield return null;
                        if (action.waitForCompletion)
                            yield return new WaitForSeconds(1);
                    }
                    break;

                case ActionType.PlayAudioClip:
                    if (audioSource != null)
                    {
                        audioSource.clip = audioClip;
                        audioSource.Play();

                        if (action.waitForCompletion)
                            yield return new WaitForSeconds(audioSource.clip.length);
                    }

                    break;

                default:
                    Debug.LogWarning($"Unknown Action Type: {action.actionType}");
                    break;
            }

            yield return null; // yield to next frame
        }
    }

    // Helper function to parse a string in format "x,y,z" into a Vector3 object
    private Vector3 ParseVector3(string inputString)
    {
        string[] parts = inputString.Split(',');
        float x = float.Parse(parts[0]);
        float y = float.Parse(parts[1]);
        float z = float.Parse(parts[2]);

        return new Vector3(x, y, z);
    }
}