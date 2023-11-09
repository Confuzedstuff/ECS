﻿using System;
using System.Collections.Generic;
using System.Linq;
using ECSDomain;
using ECSDomain.Messages;

namespace ECSDomain;

public class ECS
{
    public static readonly ECS Instance = new();
    private readonly Dictionary<Type, Archetype> archetypes = new();
    private readonly Dictionary<GlobalId, Archetype> archetypesById = new();

    private readonly List<ECSSystem> systems = new();

    private readonly MessageBuffers messageBuffers = new();

    public ECS()
    {
    }

    public void RegisterArch(Archetype arch)
    {
        archetypesById.Add(arch.GlobalId, arch);
        archetypes.Add(arch.GetType(), arch);
    }
    
    public void UpdateSystemArches(ECSSystem system)
    {
        var query = system.GetQuery();
        var validItems = GetValidArches(query);
        system.SetArches(validItems);
    }
    
    public Archetype GetArchetype(in GlobalId globalId)
    {
        if (archetypesById.TryGetValue(globalId, out var arch))
        {
            return arch;
        }

        throw new Exception($"Archetype not registered {globalId} ");
    }
    
    public ref T GetEntityComponentRef<T>(in GEntity entity, out bool entityAlive)
        where T : struct
    {
        var arch = GetArchetype(entity.EntityArchetype);
        return ref arch.GetEntityComponents<T>(entity.GIndex, out entityAlive);
    }

    public List<Archetype> GetValidArches(QueryElements queryElements)
    {
        var validItems = archetypes.Values.ToList();
        foreach (var element in queryElements.element)
        {
            element.Evaluate(validItems);
        }

        return validItems;
    }

    public void RegisterSystem(ECSSystem system) => systems.Add(system);

    public void UpdateSystems(float delta)
    {
        foreach (var system in systems)
        {
            try
            {
                system.Update(delta);
                system.Execute(delta);
                system.PostExecute(delta);
            }
            catch (Exception e)
            {
               Console.WriteLine(e); // TODO use logger
            }
        }
    }

    public void InitSystems()
    {
        foreach (var system in systems)
        {
            system.ecs = this;
            system._Init();
            UpdateSystemArches(system);
        }
    }

    public MessageReader<T> RegisterMessageReader<T>()
        where T : struct
    {
        return messageBuffers.RegisterReader<T>();
    }

    public MessageBuffer<T> GetMessageWriter<T>()
        where T : struct
    {
        return messageBuffers.GetBuffer<T>();
    }
    
}