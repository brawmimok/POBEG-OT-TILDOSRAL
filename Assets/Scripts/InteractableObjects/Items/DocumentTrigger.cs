using UnityEngine;
using UnityEngine.Events;
// щрнр яйпхор ондундхр рнкэйн й хмрпн!
public class DocumentTrigger : Displayable
{
    private bool sasung = true;
    [SerializeField] private DelayedUnityEvent pickUpEvent;
    public override void GetItemComponent(Player activator)
    {
        base.GetItemComponent(activator);
        if (sasung)
        {
            sasung = false;
            pickUpEvent.Invoke();
        }
    }
}