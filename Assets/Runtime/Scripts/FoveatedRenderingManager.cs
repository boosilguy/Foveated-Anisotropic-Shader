using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace foveated.sample
{
    public enum FRMState
    {
        Ready,
        Add,
        Remove,
        Generate,
        End
    }

    public class FoveatedRenderingManager : MonoBehaviour
    {
        [Header ("Foveated Shader Material")]
        [SerializeField] Material mat;

        [Header("Sample Interface")]
        [SerializeField] Canvas canvas;
        [SerializeField] Button btn_addPivot;
        [SerializeField] Button btn_removePivot;
        [SerializeField] Button btn_generateShader;
        [SerializeField] Button btn_reset;

        [Header("Pivot Prefab")]
        [SerializeField] PivotOnClickManager pivot;

        [HideInInspector] public Material Mat => mat;
        [HideInInspector] public CameraShader CameraShader => Camera.main.GetComponent<CameraShader>();

        public event EventHandler<Vector2> OnClickScreenViewport;

        private FRMState state = FRMState.Ready;
        private PointerEventData pointerEventData = new PointerEventData(null);
        private List<RaycastResult> raycastResults;
        private List<GameObject> pivotPool = new List<GameObject>();

        private GraphicRaycaster GraphicRaycaster => canvas.GetComponent<GraphicRaycaster>();

        public FRMState State
        {
            get => state;
            set
            {
                if (value != state)
                {
                    OnStateChanged(value);
                    state = value;
                    switch (value)
                    {
                        case FRMState.Ready:
                            OnReady(); 
                            break;
                        case FRMState.Add:
                            OnAddMode(); 
                            break;
                        case FRMState.Remove:
                            OnRemoveMode(); 
                            break;
                        case FRMState.Generate:
                            OnGenerateMode(); 
                            break;
                        case FRMState.End:
                            OnEnd(); 
                            break;
                    }
                }
            }
        }

        static FoveatedRenderingManager instance;
        public static FoveatedRenderingManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<FoveatedRenderingManager>();
                }
                return instance;
            }
        }

        private void Awake()
        {
            btn_addPivot.onClick.AddListener(() => State = FRMState.Add);
            btn_removePivot.onClick.AddListener(() => State = FRMState.Remove);
            btn_generateShader.onClick.AddListener(() => State = FRMState.Generate);
            btn_reset.onClick.AddListener(() => State = FRMState.Ready);

        }

        private void Update()
        {
            if (state == FRMState.Add)
            {
                pointerEventData.position = Input.mousePosition;
                raycastResults = new List<RaycastResult>();
                GraphicRaycaster.Raycast(pointerEventData, raycastResults);

                if (raycastResults.Count < 1)
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        OnClickScreenViewport?.Invoke(this, OnPointerClick());
                    }
                }
            }
        }

        private void OnReady()
        {
            // Ready State에서 Material 초기화
            CameraShader.ResetAnisotropyRender();
            
            // Ready State에서 PivotList 초기화
            CameraShader.ClearAnisotropyPivotList();

            // Ready State에서 Pivot GameObject 초기화
            if (pivotPool.Count != 0)
            {
                foreach (var item in pivotPool)
                    Destroy(item);
                pivotPool.Clear();
            }

            // Ready State에서 모든 이벤트 구독취소
            OnClickScreenViewport -= CameraShader.AddAnisotropyPivot;
        }

        private void OnAddMode()
        {
            // Add State에서 Pivot 추가 이벤트 구독 및 삭제 이벤트 구독 취소
            OnClickScreenViewport += CameraShader.AddAnisotropyPivot;
        }

        private void OnRemoveMode()
        {
            // Add State에서 Pivot 추가 이벤트 구독취소 및 삭제 이벤트 구독
            OnClickScreenViewport -= CameraShader.AddAnisotropyPivot;
        }

        private void OnGenerateMode()
        {
            // Generate State에서 모든 이벤트 구독취소
            OnClickScreenViewport -= CameraShader.AddAnisotropyPivot;
            
            var itemCount = CameraShader.AnisotropyPivotList.Count;
            if (itemCount < 3)
            {
                Debug.LogWarning($"<color=#00FF22>[FoveatedRenderingManager]</color> Pivot List의 아이템 개수가 최소 3개 이상 (현재 : {itemCount})이어야 합니다. Pivot을 더 추가하세요.");
                State = FRMState.Add;
            }
            else
            {
                CameraShader.GenerateAnisotropyRender();
            }
            
        }

        private void OnEnd()
        {

        }

        private void OnStateChanged(FRMState state)
        {
            Debug.Log($"<color=#00FF22>[FoveatedRenderingManager]</color> StateChange {this.state} => {state}");
        }

        public Vector2 OnPointerClick()
        {
            var input = Input.mousePosition;
            Vector2 point = Camera.main.ScreenToViewportPoint(input);
            GameObject generatedPivotObject = Instantiate(pivot.gameObject, input, Quaternion.identity, canvas.transform);
            generatedPivotObject.GetComponent<PivotOnClickManager>().PivotPosition = point;
            pivotPool.Add(generatedPivotObject);
            Debug.Log($"<color=#00FF22>[FoveatedRenderingManager]</color> Pivot을 추가하였습니다.");
            return point;
        }

        public bool RemovePivotInPool(PivotOnClickManager target)
        {
            bool result = pivotPool.Remove(target.gameObject);
            if(result)
                Debug.Log($"<color=#00FF22>[FoveatedRenderingManager]</color> Pivot을 제거하였습니다.");
            return result;
        }
    }
}
