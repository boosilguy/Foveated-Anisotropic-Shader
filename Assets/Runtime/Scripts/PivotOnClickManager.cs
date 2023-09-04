using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace foveated.sample
{
    public class PivotOnClickManager : MonoBehaviour, IPointerClickHandler
    {
        FoveatedRenderingManager FoveatedRenderingManager => FoveatedRenderingManager.Instance;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (FoveatedRenderingManager.State == FRMState.Remove)
            {
                FoveatedRenderingManager.RemovePivotInPool(this);
                Destroy(this.gameObject);
            }
        }
    }
}