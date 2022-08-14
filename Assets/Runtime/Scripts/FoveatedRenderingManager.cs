using System.Collections;
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
        [SerializeField] Button btn_addPivot;
        [SerializeField] Button btn_removePivot;
        [SerializeField] Button btn_generateShader;
        [SerializeField] Button btn_reset;

        [HideInInspector] public Material Mat => mat;
        [HideInInspector] public CameraShader CameraShader => Camera.main.GetComponent<CameraShader>();

        public event EventHandler<Vector3> OnClickScreenViewport;

        private FRMState state = FRMState.Ready;

        public FRMState State
        {
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
                if (Input.GetMouseButtonUp(0))
                {
                    OnClickScreenViewport?.Invoke(this, OnPointerClick());
                }
            }
        }

        private void OnReady()
        {
            // Ready State에서 PivotList 초기화
            CameraShader.ClearAnisotropyPivotList();
        }

        private void OnAddMode()
        {
            // Add State에서 Pivot 추가 이벤트 구독 및 삭제 이벤트 구독 취소
            OnClickScreenViewport += CameraShader.AddAnisotropyPivot;
            OnClickScreenViewport -= CameraShader.RemoveAnisotropyPivot;
        }

        private void OnRemoveMode()
        {
            // Add State에서 Pivot 추가 이벤트 구독취소 및 삭제 이벤트 구독
            OnClickScreenViewport -= CameraShader.AddAnisotropyPivot;
            OnClickScreenViewport += CameraShader.RemoveAnisotropyPivot;
        }

        private void OnGenerateMode()
        {
            CameraShader.GenerateAnisotropyRender();
        }

        private void OnEnd()
        {

        }

        private void OnStateChanged(FRMState state)
        {
            Debug.Log($"<color=#00FF22>[FoveatedRenderingManager]</color> StateChange {this.state} => {state}");
        }

        public Vector3 OnPointerClick()
        {
            Vector3 point = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            Debug.Log($"<color=#00FF22>[FoveatedRenderingManager]</color> 마우스 버튼이 ({point.x}, {point.y})에서 눌렸습니다.");
            return point;
        }
    }

}
