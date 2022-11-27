using UnityEngine;
using Zenject;

public abstract class MonoExtendedInstaller : MonoInstaller
{
    protected ConcreteIdArgConditionCopyNonLazyBinder BindSingle<T>() =>
        Container.BindInterfacesAndSelfTo<T>().AsSingle();

    protected ConcreteIdArgConditionCopyNonLazyBinder BindFromHierarchy<T>() =>
        Container.BindInterfacesAndSelfTo<T>().FromComponentInHierarchy().AsSingle();

    protected ConcreteIdArgConditionCopyNonLazyBinder BindScriptableObject<T>(T instance) where T : ScriptableObject =>
        Container.BindInterfacesAndSelfTo<T>().FromScriptableObject(instance).AsSingle();

    protected ConcreteIdArgConditionCopyNonLazyBinder BindPrefab<T>(T prefab) where T : Object =>
        Container.BindInterfacesAndSelfTo<T>().FromComponentInNewPrefab(prefab).AsSingle();

    protected ConcreteIdArgConditionCopyNonLazyBinder BindMonoBehaviour<T>() where T : Object =>
        Container.BindInterfacesAndSelfTo<T>().FromNewComponentOnNewGameObject().AsSingle();

    protected ConcreteIdArgConditionCopyNonLazyBinder BindInstance<T>(T instance) =>
        Container.BindInterfacesAndSelfTo<T>().FromInstance(instance).AsSingle();
}
