using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

namespace foveated.sample
{
    public class PivotOnClickManager : MonoBehaviour, IPointerClickHandler
    {
        FoveatedRenderingManager FoveatedRenderingManager => FoveatedRenderingManager.Instance;
        Vector2 pivotPosition = Vector2.zero;
        public Vector2 PivotPosition { set => pivotPosition = value; }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (FoveatedRenderingManager.State == FRMState.Remove)
            {
                FoveatedRenderingManager.CameraShader.RemoveAnisotropyPivot(this, pivotPosition);
                FoveatedRenderingManager.RemovePivotInPool(this);
                Destroy(this.gameObject);
            }
        }
    }
}