using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Zenject;

[Serializable]
public class LevelProvider : ILevelsProvider
{
    [SerializeField] private List<LevelData> m_LevelDatas = new List<LevelData>();
    public Level GetLevelPrefab(int levelNumber) => m_LevelDatas[levelNumber].LevelPrefab;

    public int GetLevelNumber(Level level)
    {
        for (int i = 0; i < m_LevelDatas.Count; i++)
        {
            if (m_LevelDatas[i].LevelPrefab == level)
                return i;
        }

        return -1;
    }

    public int LevelsCount => m_LevelDatas.Count;
}

[Serializable]
public struct LevelData
{
    public Level LevelPrefab;
}

public class MainInstaller : MonoInstaller
{
    [SerializeField] private CableBehaviour m_cableBehaviourPrefab;
    [SerializeField] private LevelProvider m_levelProvider;
    [SerializeField] private RigidBodySettingsProvider m_pidSettingsProvider;

    public override void InstallBindings()
    {
        //Stupid id providers
        Container.Bind<UniquePostProvider>().To<UniquePostProvider>().AsSingle();
        Container.Bind<UniqueIdProvider>().To<UniqueIdProvider>().AsSingle();

        //Camera
        Container.Bind<CameraRaycaster>().To<CameraRaycaster>().FromComponentsInHierarchy().AsSingle();
        
        //Pid controllers for object translation and rotation
        Container.Bind<IPidSettingsProvider>().To<RigidBodySettingsProvider>().FromInstance(m_pidSettingsProvider).AsSingle();
        
        //Electricity
        Container.Bind<ConnectionManager>().To<ConnectionManager>().FromComponentsInHierarchy().AsSingle();
        Container.BindFactory<CableBehaviour, CableBehaviour.Factory>().FromComponentInNewPrefab(m_cableBehaviourPrefab).AsSingle();

        //Factory to load level prefab and init it
        Container.Bind<ILevelsProvider>().FromInstance(m_levelProvider);
        Container.BindFactory<int, Level, Level.Factory>().FromFactory<LevelFactory>();
    }
}