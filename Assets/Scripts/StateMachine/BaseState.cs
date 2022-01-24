using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    public abstract void OnStateEnter(ApplicationController appController);

    public abstract void Update(ApplicationController appController);
}
