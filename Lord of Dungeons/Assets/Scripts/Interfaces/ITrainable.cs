using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrainable
{
    public IEnumerator StartTraining();
    public bool IsTraining();
}
