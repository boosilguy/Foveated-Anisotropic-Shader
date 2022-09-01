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
            UIDebugInfo.Log(foveatedRenderingManager.State.ToString(), "�Ŵ��� ����");
            UIDebugInfo.Log(foveatedRenderingManager.Mat.shader.name, "���� ���̴�");
            UIDebugInfo.Log(cameraShader.AnisotropyPivotList.Count.ToString(), "�ǹ� ����");
            UIDebugInfo.Log(cameraShader.GetAnisotropyPivotListInfo(true), "�ǹ� ����");
        }
    }
}
