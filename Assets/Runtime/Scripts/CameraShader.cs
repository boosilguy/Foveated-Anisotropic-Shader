using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace foveated.sample
{
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode] // Editor ��忡���� Ȯ���ϱ� ���� �޾Ƴ��� Attribute
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
                    Debug.Log($"<color=#00FF22>[CameraShader]</color> {FoveatedRenderingManager.Instance.Mat.name} ī�޶� ���̴��� ����Ǿ����ϴ�.");
                    Init = true;
                }

                Graphics.Blit(source, destination, FoveatedRenderingManager.Instance.Mat);
            }
            else
            {
                Debug.LogWarning($"<color=#00FF22>[CameraShader]</color> ī�޶� ���̴��� null�Դϴ�.");
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
                Debug.LogError($"<color=#00FF22>[CameraShader]</color> Shader PivotList�� �̹� ����ֽ��ϴ�.");
        }

        public void RemoveAnisotropyPivot()
        {
            if (anisotropyPivotList.Count != 0)
                anisotropyPivotList.RemoveAt(anisotropyPivotList.Count - 1);
            else
                Debug.LogError($"<color=#00FF22>[CameraShader]</color> Shader PivotList�� �̹� ����ֽ��ϴ�.");
        }

        public void ClearAnisotropyPivotList()
        {
            anisotropyPivotList.Clear();
            Debug.Log($"<color=#00FF22>[CameraShader]</color> Shader PivotList �ʱ�ȭ");
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