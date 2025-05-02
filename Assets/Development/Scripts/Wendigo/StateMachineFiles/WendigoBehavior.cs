using System.Threading;
using UnityEngine;


public class WendigoBehaviour : MonoBehaviour
{
    protected bool isActive = false;
    protected bool isEnding = false;
    public bool isComplete
    {
        get
        {
            return !isEnding && !isActive;
        }
    }

    public virtual void EnterState()
    {
        isActive = true;
        isEnding = false;
    }

    public virtual void ExitState()
    {
        isActive = false;
        isEnding = true;
    }

    public virtual void Run()
    {
    }
}
