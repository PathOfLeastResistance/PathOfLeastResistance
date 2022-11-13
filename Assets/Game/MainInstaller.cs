using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<UniquePostProvider>().To<UniquePostProvider>().AsSingle();
        Container.Bind<UniqueIdProvider>().To<UniqueIdProvider>().AsSingle();
        
        Container.Bind<ConnectionManager>().To<ConnectionManager>().FromComponentsInHierarchy().AsSingle();
    }
}