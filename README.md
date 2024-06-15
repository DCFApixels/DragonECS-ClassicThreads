<p align="center">
<img width="400" src="https://github.com/DCFApixels/DragonECS-ClassicThreads/assets/99481254/fe788eb4-dcb5-40a9-b25f-4094753bc021.png">
</p>

<p align="center">
<img alt="Version" src="https://img.shields.io/github/package-json/v/DCFApixels/DragonECS-ClassicThreads?color=%23ff4e85&style=for-the-badge">
<img alt="License" src="https://img.shields.io/github/license/DCFApixels/DragonECS-ClassicThreads?color=ff4e85&style=for-the-badge">
<a href="https://discord.gg/kqmJjExuCf"><img alt="Discord" src="https://img.shields.io/badge/Discord-JOIN-00b269?logo=discord&logoColor=%23ffffff&style=for-the-badge"></a>
<a href="http://qm.qq.com/cgi-bin/qm/qr?_wv=1027&k=IbDcH43vhfArb30luGMP1TMXB3GCHzxm&authKey=s%2FJfqvv46PswFq68irnGhkLrMR6y9tf%2FUn2mogYizSOGiS%2BmB%2B8Ar9I%2Fnr%2Bs4oS%2B&noverify=0&group_code=949562781"><img alt="QQ" src="https://img.shields.io/badge/QQ-JOIN-00b269?logo=tencentqq&logoColor=%23ffffff&style=for-the-badge"></a>
</p>

# Classic C# Threads for [DragonECS](https://github.com/DCFApixels/DragonECS)

| Languages: | [Русский](https://github.com/DCFApixels/DragonECS-ClassicThreads/blob/main/README-RU.md) | [English(WIP)](https://github.com/DCFApixels/DragonECS-ClassicThreads) |
| :--- | :--- | :--- |

Support for processing entities in multiple threads, based on classic C# threads implementation.
> **NOTICE:** The project is a work in progress, API may change.  
> While the English version of the README is incomplete, you can view the [Russian version](https://github.com/DCFApixels/DragonECS-ClassicThreads/blob/main/README-RU.md).

</br>

# Installation
Versioning semantics - [Open](https://gist.github.com/DCFApixels/e53281d4628b19fe5278f3e77a7da9e8#file-dcfapixels_versioning_ru-md)
## Environment
Requirements:
+ Dependency: [DragonECS](https://github.com/DCFApixels/DragonECS)
+ Minimum version of C# 7.3;
  
Optional:
+ Support for NativeAOT
+ Game engines with C#: Unity, Godot, MonoGame, etc.
  
Tested with:
+ **Unity:** Minimum version 2020.1.0;

## Unity Installation
* ### Unity Package
The framework can be installed as a Unity package by adding the Git URL [in the PackageManager](https://docs.unity3d.com/2023.2/Documentation/Manual/upm-ui-giturl.html) or manually adding it to `Packages/manifest.json`: 
```
https://github.com/DCFApixels/DragonECS-ClassicThreads.git
```
* ### Source Code
The framework can also be added to the project as source code.

</br>

# Parallel iteration
``` csharp
EcsThreadHandler _handler;
public void Run(EcsPipeline pipeline)
{
    var group = _world.Where(out Aspect a);
    void Handler(ReadOnlySpan<int> entities)
    {
        foreach (var e in entities)
        {
            a.poses.Get(e).position += a.velocities.Read(e).value * _time.DeltaTime;
        }
    }
    group.IterateParallel(_handler ??= Handler, 1000);
}
```
> **NOTICE:** The smaller the minimum size of the group part when dividing, the more threads can be utilized. In some situations, too many threads can negatively impact performance.

> **NOTICE:** Inside the handler, creating/deleting entities, adding/removing components on entities is prohibited. Only modification of data within components is allowed.
