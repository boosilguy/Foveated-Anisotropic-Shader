using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace foveated.sample
{
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode] // Editor 모드에서도 확인하기 위해 달아놓은 Attribute
    public class CameraShader : MonoBehaviour
    {
        private bool Init { get; set; } = false;
        private List<Vector4> anisotropyPivotList = new List<Vector4>();

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (FoveatedRenderingManager.Instance.Mat != null)
            {
                if (Init == false)
                {
                    Debug.Log($"<color=#00FF22>[CameraShader]</color> {FoveatedRenderingManager.Instance.Mat.name} 카메라 쉐이더가 적용되었습니다.");
                    Init = true;
                }

                Graphics.Blit(source, destination, FoveatedRenderingManager.Instance.Mat);
            }
            else
            {
                Debug.LogWarning($"<color=#00FF22>[CameraShader]</color> 카메라 쉐이더가 null입니다.");
            }
        }

        public void AddAnisotropyPivot(object sender, Vector3 target)
        {
            anisotropyPivotList.Add(new Vector4(target.x, target.y));
        }

        public void RemoveAnisotropyPivot(object sender, Vector3 target)
        {
            if (anisotropyPivotList.Count != 0)
            {
                Vector2 target_v2 = new Vector2(target.x, target.y);
                Vector2 nearest = anisotropyPivotList.OrderBy(p => Vector2.Distance(p, target_v2)).FirstOrDefault();
                anisotropyPivotList.Remove(nearest);
            }
            else
                Debug.LogError($"<color=#00FF22>[CameraShader]</color> Shader PivotList가 이미 비어있습니다.");
        }

        public void RemoveAnisotropyPivot()
        {
            if (anisotropyPivotList.Count != 0)
                anisotropyPivotList.RemoveAt(anisotropyPivotList.Count - 1);
            else
                Debug.LogError($"<color=#00FF22>[CameraShader]</color> Shader PivotList가 이미 비어있습니다.");
        }

        public void ClearAnisotropyPivotList()
        {
            anisotropyPivotList.Clear();
            Debug.Log($"<color=#00FF22>[CameraShader]</color> Shader PivotList 초기화");
        }

        public void GenerateAnisotropyRender()
        {
            FoveatedRenderingManager.Instance.Mat.SetVectorArray("_Pivots", anisotropyPivotList);
            var output = FoveatedRenderingManager.Instance.Mat.GetVectorArray("_Pivots");

            foreach (var item in output)
                Debug.Log(item);
        }
    }
}