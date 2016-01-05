using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class ViewInstructionListenter : InteractEventHandler
{
    private void Awake()
    {
        Init();
    }

    public override void Interact()
    {
       // ViewInstruction.Instance.Show();
    }

    public override void Interact(UnityAction callback)
    {
        //ViewInstruction.Instance.Show(callback);
    }
}