# Unity Service Injection Framework
Flexible Dependency Injection designed specifically for the Unity Engine. 
This lightweight and intuitive framework is designed to seamlessly integrate with Unity's native workflow, 
providing a straightforward approach to managing dependencies in your game.

The framework is heavily inspired by [Zenject](https://github.com/modesttree/Zenject) but reduces its complexity.

## Table of Contents

- [📥 Installation](#-installation)
- [🚀 Getting Started](#-getting-started)
- [Bootstrapping](#bootstrapping)
    - [Registration](#registration)
    - [Resolve Types](#resolve-types)
    - [Lifetime Scope](#lifetime-scope)
- [Resolve Dependencies](#resolve-dependencies)
    - [By Constructor](#by-constructor)
    - [By Resolve Methods](#by-resolve-methods)
    - [By Attribute](#by-attribute)
    - [Utilities](#utilities)
      
## 📥 Installation

Open the Package Manager in Unity and choose Add package from git URL, then enter:

```
https://github.com/rehavvk/service-injection.git
```

from the `Add package from git URL` option.

## 🚀 Getting Started

To enable the service injection, you need to add a `ProjectContext` to your project.

1. The context menu entry `Create > Service Injection > Project Context` in the project window will create a new one for you.
Move it somewhere in a Resources folder. This one is used loaded once when a scene with a `SceneContext` in it is loaded.

2. Create a new **Boostrapper** class derived from `MonoBootstrapper` and add it to the created `ProjectContext` as component.
Drag your bootstrapper in the bootstrapper array on the `ProjectContext`.
3. Create a new `SceneContext` in your desired scene. The context menu entry `Create > Service Injection > Project Context` in the inspector will create a new one for you. 
A scene without a `SceneContext` will not start the bootstrapping if loaded as first scene. 
But such a scene would still have access to the registered services if the `ProjectContext` was initialized once.

You are ready to add bootstrapping to your newly created bootstrapper.

## Bootstrapping

The bootstrapping is separated into multiple phases.

1. **Register Phase** > Register how dependencies are resolved.
2. **Boot Completed Phase** > Do other bootstrapping steps which require dependencies to be registered. From now on it is possible to resolve dependencies from the `ServiceLocator`.
3. **Resolve Phase** > All registered and created instances receive their resolved dependencies.

> ℹ️ **The system sorts the registered types by their dependencies to ensure all dependencies are created before they are resolved.**

All Bootstrappers have to implement at least the `Boot()` to do registration. The `OnBootCompleted()` is optional.

```csharp
public class MyBootstrapper : MonoBootstrapper
{
    [SerializeField] private MyService myService;
        
    protected override void Boot()
    {
        Register<MyService>()
             .FromInstance(myService);
    }
}
```

### Registration

You always have to define the contract type of the registration. The concrete type is optional.
So you are able to define that other parts of your project have a inconcrete dependency like an 
interface or abstract class and define in some bootstrapper what kind of object is passed to them.

The framework provides different ways to register your resolvations and follows a builder pattern.

**Register**

If no dependency source is provided, the bootstrapping will create an instance of 
the concrete type as soon as the registration is completed.

```csharp
protected override void Boot()
{
    // Register concrete only. Concrete type is contract type too.
    Register<MyService>();
    
    // Register contract type and concrete type.
    Register<IMyService, MyService>();
}
```

**Register Instance**

Register a specific instance which is used to resolve the dependency.

```csharp
[SerializeField] private MyService myService;

protected override void Boot()
{  
    Register<IMyService>()
        .FromInstance(myService);
    
    OR
        
    RegisterInstance<IMyService>(myService);
}
```

**Register Factory**

Register a factory which is used to resolve the dependency.

```csharp
[SerializeField] private MyService myService;

protected override void Boot()
{  
    Register<IMyService>()
        .FromFactory(() => new MyService());
    
    OR
        
    RegisterFactory<IMyService>(() => new MyService());
}
```

### Resolve Types

By default, the dependency is resolved once and cached for later resolves.

**As Singleton**

You can clarify it by adding the builder method.

```csharp
[SerializeField] private MyService myService;

protected override void Boot()
{  
    Register<IMyService, MyService>()
        .AsSingleton();
}
```

**As Transient**

You can alter this behavior so that a new resolvation is done when resolving this dependency.

```csharp
[SerializeField] private MyService myService;

protected override void Boot()
{  
    Register<IMyService, MyService>()
        .AsTransient();
}
```

### Lifetime Scope

By default, all dependencies are available from their time of registration until the application is closed.

**Global Scoped**

You can clarify it by adding the builder method.

```csharp
[SerializeField] private MyService myService;

protected override void Boot()
{  
    Register<IMyService, MyService>()
        .GlobalScoped();
}
```

**Scene Scoped**

You can alter this behavior so that the registration only is valid as long the scene the bootstrapper was executed in is loaded.

```csharp
[SerializeField] private MyService myService;

protected override void Boot()
{  
    Register<IMyService, MyService>() 
        .SceneScoped();
}
```

## Resolve Dependencies

### By Constructor

When an instance is created via `ServiceLocator.CreateInstance<T>()` or `ResolveDependencies<T>(T instance)` is called on it, 
all types in their constructor are tried to get resolved from the injection registry.

```csharp
public class MyExample
{
    public MyExample(IMyService myService)
    {
        
    }
}
```

### By Resolve Methods

In `MonoBehaviours` or where ever you like you can resolve dependencies by the `Resolve` method.

```csharp
private MyService myService;
        
private void Awake()
{
    myService = this.Resolve<IMyService>();
}
```

### By Attribute

When an instance is created via `ServiceLocator.CreateInstance<T>()` or `ResolveDependencies<T>(T instance)` is called on it,
all fields and properties with the `[Inject]` attribute are tried to get resolved from the injection registry.

```csharp
[Inject]
private MyService myService;
```

### Utilities

**With Arguments**

You can pass more non-registry parameters for resolving dependencies of an injection registration.
They have priority over registry entries and get resolved by their type.

```csharp
protected override void Boot()
{
    Register<IMyService, MyService>()
        .WithArguments("Test", true);
}

public class MyService : IMyService 
{
    public MyService(string name, bool isActive) 
    {
        
    }
}
```

**With Callback**

You can add a callback to do stuff as soon as the dependency instance is created for the first time.

```csharp
[SerializeField] private MyService myService;
        
protected override void Boot()
{
    RegisterInstance<IMyService>(myService)
        .WithCallback((MyService myService) =>
        {
            myService.Init();
        });
}
```

---

***Happy resolving with the Unity Service Injection Framework!***
