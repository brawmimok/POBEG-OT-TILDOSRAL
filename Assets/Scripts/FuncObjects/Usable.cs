using UnityEngine;

public abstract class Usable : MonoBehaviour
{
    virtual public void Use(Player activator)
    {

    }
}
public class LockableUsable : Usable
{
    protected bool locked { get; set; }
    virtual public void LockedUse(Player activator)
    {
    }
    public override void Use(Player activator)
    {
        if (!locked)
        {
            base.Use(activator);
            return;
        }
        LockedUse(activator);
    }
    public void Lock()
    {
        locked = true;
    }
    public void UnLock()
    {
        locked = false;
    }
}