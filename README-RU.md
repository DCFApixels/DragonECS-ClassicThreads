<p align="center">
<img alt="Version" src="https://img.shields.io/github/package-json/v/DCFApixels/DragonECS-ClassicThreads?color=%23ff4e85&style=for-the-badge">
<img alt="License" src="https://img.shields.io/github/license/DCFApixels/DragonECS-ClassicThreads?color=ff4e85&style=for-the-badge">
<!--<img alt="Discord" src="https://img.shields.io/discord/1111696966208999525?color=%23ff4e85&label=Discord&logo=Discord&logoColor=%23ff4e85&style=for-the-badge">-->
</p>

# Классические C# Threads для [DragonECS](https://github.com/DCFApixels/DragonECS)

| Languages: | [Русский](https://github.com/DCFApixels/DragonECS-ClassicThreads/blob/main/README-RU.md) | [English(WIP)](https://github.com/DCFApixels/DragonECS-ClassicThreads) |
| :--- | :--- | :--- |

Поддержка обработки сущностей в нескольких потоках, на основе классической реализации потоков в C#.
> **ВАЖНО!** Проект в стадии разработки. API может меняться.
# Оглавление
* [Установка](#Установка)
   * [Зависимости](#Зависимости)
   * [Unity-модуль](#Unity-модуль)
   * [В виде исходников](#В-виде-иходников)
* [Параллельная итерация группы](#Параллельная-итерация-группы)

# Установка
### Зависимости
Убедитесь что в проекте установлен фреймворк [DragonECS](https://github.com/DCFApixels/DragonECS).
* ### Unity-модуль
Поддерживается установка в виде Unity-модуля в  при помощи добавления git-URL [в PackageManager](https://docs.unity3d.com/2023.2/Documentation/Manual/upm-ui-giturl.html) или ручного добавления в `Packages/manifest.json`: 
```
https://github.com/DCFApixels/DragonECS-AutoInjections.git
```
* ### В виде исходников
Фреймворк так же может быть добавлен в проект в виде исходников. 

### Версионирование
В DragonECS применяется следующая семантика версионирования: [Открыть](https://gist.github.com/DCFApixels/e53281d4628b19fe5278f3e77a7da9e8#%D0%B2%D0%B5%D1%80%D1%81%D0%B8%D0%BE%D0%BD%D0%B8%D1%80%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5)

# Параллельная итерация группы
``` csharp
EcsThreadHandler _handler;
public void Run(EcsPipeline pipeline)
{
    //Получение субъекта и группы для итерации.
    var group = _world.Where(out Subject s);
    void Handler(ReadOnlySpan<int> entities)
    {
        foreach (var e in entities)
        {
            s.poses.Get(e).position += s.velocities.Read(e).value * _time.DeltaTime;
        }
    }
    // Запускает параллельную итерацию по группе, 
    // группа будет рабита на части с минимальным размером 1000.
    group.IterateParallel(_handler ??= Handler, 1000);
}
```
> **NOTICE:** Чем меньше минимальный размер части группы при делении, тем больше потоков может быть задействовано, в некоторых ситуациях слишком много потоков может негативно повлиять на производительность.

> **NOTICE:** Внутри обработчика запрещено изменять состояние мира: нельзя создавать/удалять сущности, нельзя добавлять/удалять компоненты на сущности. Допускается только модификация данных внутри существующих компонентов.
