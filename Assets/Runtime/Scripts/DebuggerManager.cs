using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace foveated.sample
{
    public class DebuggerManager : MonoBehaviour
    {
        FoveatedRenderingManager foveatedRenderingManager;
        CameraShader cameraShader;
        private void Awake()
        {
            foveatedRenderingManager = FoveatedRenderingManager.Instance;
            cameraShader = foveatedRenderingManager.CameraShader;
        }

        private void Update()
        {
            UIDebugInfo.Log(foveatedRenderingManager.State.ToString(), "매니져 상태");
            UIDebugInfo.Log(foveatedRenderingManager.Mat.shader.name, "적용 쉐이더");
            UIDebugInfo.Log(cameraShader.AnisotropyPivotList.Count.ToString(), "피벗 개수");
            UIDebugInfo.Log(cameraShader.GetAnisotropyPivotListInfo(true), "피벗 정보");
        }
    }
}
