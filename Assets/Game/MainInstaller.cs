using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private CableBehaviour m_cableBehaviourPrefab;
    [SerializeField] private CableView m_cableViewPrefab;

    public override void InstallBindings()
    {
        //Stupid id providers
        Container.Bind<UniquePostProvider>().To<UniquePostProvider>().AsSingle();
        Container.Bind<UniqueIdProvider>().To<UniqueIdProvider>().AsSingle();

        //Electricity
        Container.Bind<ConnectionManager>().To<ConnectionManager>().FromComponentsInHierarchy().AsSingle();
        Container.BindFactory<CableBehaviour, CableBehaviour.Factory>().FromComponentInNewPrefab(m_cableBehaviourPrefab).AsSingle();
        Container.BindFactory<CableView, CableView.Factory>().FromComponentInNewPrefab(m_cableBehaviourPrefab).AsSingle();
    }
}