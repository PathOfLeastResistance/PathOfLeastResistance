using UnityEngine;
using Zenject;

public abstract class ScriptableObjectExtendedInstaller : ScriptableObjectInstaller
{
    protected void BindSingle<T>() =>
        Container.BindInterfacesAndSelfTo<T>().AsSingle();

    protected void BindFromHierarchy<T>() =>
        Container.BindInterfacesAndSelfTo<T>().FromComponentInHierarchy().AsSingle();

    protected void BindScriptableObject<T>(T instance) where T : ScriptableObject =>
        Container.BindInterfacesAndSelfTo<T>().FromScriptableObject(instance).AsSingle();

    protected void BindPrefab<T>(T prefab) where T : Object =>
        Container.BindInterfacesAndSelfTo<T>().FromComponentInNewPrefab(prefab).AsSingle();
}
