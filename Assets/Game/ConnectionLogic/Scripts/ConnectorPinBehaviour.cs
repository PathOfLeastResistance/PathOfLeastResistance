using System;
using System.Collections;
using UnityEngine;
using UnityTools;
using Zenject;

/// <summary>
/// The pin behaviour, user can drag and drop this pin to connect to other pins
/// </summary>
public class ConnectorPinBehaviour : MonoBehaviour
{
    [Inject] private UniqueIdProvider m_idProvider;
    [Inject] private ConnectionManager m_connectionManager;
    [SerializeField] private Transform m_pinConnectionTransform;
    
    private uint m_id;
    private Vector3 m_lastPos;
    private InteractionObject m_interactionObject;
    private Func<int> mPostGetter;

    private event Action<ConnectorPinBehaviour, Vector3> PinDragStartEvent;
    private event Action<ConnectorPinBehaviour, Vector3> PinDragEvent;
    private event Action<ConnectorPinBehaviour, Vector3> PinDragEndEvent;

    private event Action<Vector3> PinPositionChangeEvent;

    private bool m_isInited = false;
    
    public uint Id => m_id;

    public bool HasId => m_id != 0;

    public int Post => mPostGetter();
    
    public Vector3 ConnectionPoint => m_pinConnectionTransform.position;

    public IDisposable SubscribePinPosition(Action<Vector3> pinPositionHandler)
    {
        pinPositionHandler?.Invoke(ConnectionPoint);
        PinPositionChangeEvent += pinPositionHandler;
        return new DisposableAction(() => PinPositionChangeEvent -= pinPositionHandler);
    }
    
    public IDisposable SubscribePinDrag(Action<ConnectorPinBehaviour, Vector3> dragStart, Action<ConnectorPinBehaviour, Vector3> drag, Action<ConnectorPinBehaviour, Vector3> dragEnd)
    {
        PinDragStartEvent += dragStart;
        PinDragEvent += drag;
        PinDragEndEvent += dragEnd;
        return new DisposableAction(() =>
        {
            PinDragStartEvent -= dragStart;
            PinDragEvent -= drag;
            PinDragEndEvent -= dragEnd;
        });
    }

    public void Init(Func<int> postGetter, ulong id = 0)
    {
        if (m_isInited)
            return;

        if (id == 0)
            m_id = m_idProvider.GetId();

        mPostGetter = postGetter ?? throw new ArgumentNullException();
        m_interactionObject = GetComponent<InteractionObject>();
        m_interactionObject.SubscribePointerDragEvent(OnDragStart, OnDrag, OnDragEnd);  
        m_connectionManager.RegisterPin(this);
        m_isInited = true;
    }

    private void OnDragStart(object sender, PointerDragInteractionEventArgs args) => PinDragStartEvent?.Invoke(this, args.PointerPrevPosition);

    private void OnDrag(object sender, PointerDragInteractionEventArgs args) => PinDragEvent?.Invoke(this, args.PointerPosition);

    private void OnDragEnd(object sender, PointerDragInteractionEventArgs args) => PinDragEndEvent?.Invoke(this, args.PointerPrevPosition);

    
    private void Update()
    {
        if (!m_lastPos.Equals(ConnectionPoint))
        {
            m_lastPos = ConnectionPoint;
            PinPositionChangeEvent?.Invoke(ConnectionPoint);
        }
    }
}