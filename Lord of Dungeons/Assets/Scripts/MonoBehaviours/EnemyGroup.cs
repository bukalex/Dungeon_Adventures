using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyGroup : MonoBehaviour, ITrainable
{
    private bool isTraining = true;

    public IEnumerator StartTraining()
    {
        TrainingManager.Instance.uiBlocked = false;
        TrainingManager.Instance.movementBlocked = false;
        TrainingManager.Instance.attacksBlocked = false;

        //Dialog
        TrainingManager.Instance.dialogPanel.SetActive(true);
        TrainingManager.Instance.nameText.text = "";
        TrainingManager.Instance.textFieldObject.text = "";
        StartCoroutine(TrainingManager.Instance.StartTyping("Player:", TrainingManager.Instance.nameText));
        yield return new WaitWhile(() => TrainingManager.Instance.isTyping);
        StartCoroutine(TrainingManager.Instance.StartTyping("Who are you?", TrainingManager.Instance.textFieldObject));
        yield return new WaitWhile(() => TrainingManager.Instance.isTyping);
        yield return new WaitForSeconds(2.0f);
        TrainingManager.Instance.dialogPanel.SetActive(false);

        TrainingManager.Instance.AddTask("Kill the Guards");
        while (TrainingManager.Instance.HasUndoneTasks())
        {
            TrainingManager.Instance.taskList.GetChild(0).GetComponent<Toggle>().isOn = transform.childCount == 0;
            yield return null;
        }

        StartCoroutine(TrainingManager.Instance.RemoveTasks());
        yield return new WaitWhile(() => TrainingManager.Instance.isRemovingTasks);

        isTraining = false;
    }

    public bool IsTraining()
    {
        return isTraining;
    }
}
