using System;
using System.Collections;
using UnityEngine;
using UnityTools;

public class UniqueIdProvider
{
    private uint mIdCounter = 0;

    public uint GetId()
    {
        return ++mIdCounter;
    }
}

/// <summary>
/// The pin behaviour, user can drag and drop this pin to connect to other pins
/// </summary>
public class ConnectorPinBehaviour : MonoBehaviour
{
    private uint m_id;

    private InteractionObject m_interactionObject;
    private UniqueIdProvider mIdProvider;
    private Func<int> mPostGetter;

    private event Action<ConnectorPinBehaviour, Vector3> PinDragStartEvent;
    private event Action<ConnectorPinBehaviour, Vector3> PinDragEvent;
    private event Action<ConnectorPinBehaviour, Vector3> PinDragEndEvent;

    private bool m_isInited = false;

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

    public uint Id => m_id;

    public bool HasId => m_id != 0;

    public int GetPost() => mPostGetter();

    public void Init(Func<int> postGetter, ulong id = 0)
    {
        if (m_isInited)
            return;

        m_interactionObject = GetComponent<InteractionObject>();
        if (id == 0)
            m_id = mIdProvider.GetId();

        mPostGetter = postGetter ?? throw new ArgumentNullException();
        m_interactionObject = GetComponent<InteractionObject>();
        m_interactionObject.SubscribePointerDragEvent(OnDragStart, OnDrag, OnDragEnd);
    }

    private void OnDragStart(object sender, PointerDragInteractionEventArgs args) => PinDragStartEvent?.Invoke(this, args.PointerPrevPosition);

    private void OnDrag(object sender, PointerDragInteractionEventArgs args) => PinDragEvent?.Invoke(this, args.PointerPosition);

    private void OnDragEnd(object sender, PointerDragInteractionEventArgs args) => PinDragEndEvent?.Invoke(this, args.PointerPrevPosition);
}