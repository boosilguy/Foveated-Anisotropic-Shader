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
        private List<Vector2> anisotropyPivotList = new List<Vector2>();

        public List<Vector2> AnisotropyPivotList => anisotropyPivotList;

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
            // HLSL에서 Dynamic Array를 지원하지 않음.
            // 따라서, Texture의 Pixel 데이터들을 이용해서 동적 데이터를 Shader로 넘길거임.
            // 인자값으로 Texture 데이터와 Pivot Length를 쉐이더로 넘긴 후, Decode하여 사용할 예정.
            int count = anisotropyPivotList.Count;
            Texture2D input = new Texture2D(count, 1, TextureFormat.RGBA32, false);
            input.filterMode = FilterMode.Point;
            input.wrapMode = TextureWrapMode.Clamp;

            for (int i = 0; i < count; i++)
            {
                input.SetPixel(i, 0, new Color(anisotropyPivotList[i].x, anisotropyPivotList[i].y, 0.0f, 1.0f));
            }
            input.Apply();
            FoveatedRenderingManager.Instance.Mat.SetTexture("_Container", input);
            FoveatedRenderingManager.Instance.Mat.SetInt("_ContainerLength", count);
        }
    }
}