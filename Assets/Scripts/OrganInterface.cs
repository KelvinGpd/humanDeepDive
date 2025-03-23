using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OrganInterface : MonoBehaviour
{
    private string organId;
    private bool isInteractible;

    public string getId()
    {
        return organId;
    }

    public void setInteractible(bool isInteractible)
    {
        this.isInteractible = isInteractible;
    }

    public abstract void MoveBack();
    public abstract void MoveToSpotlight(Transform targetPosition);
}
