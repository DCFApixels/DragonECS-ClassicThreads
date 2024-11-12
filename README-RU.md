<p align="center">
<img width="400" src="https://github.com/DCFApixels/DragonECS-ClassicThreads/assets/99481254/fe788eb4-dcb5-40a9-b25f-4094753bc021.png">
</p>

<p align="center">
<img alt="Version" src="https://img.shields.io/github/package-json/v/DCFApixels/DragonECS-ClassicThreads?color=%23ff4e85&style=for-the-badge">
<img alt="License" src="https://img.shields.io/github/license/DCFApixels/DragonECS-ClassicThreads?color=ff4e85&style=for-the-badge">
<a href="https://discord.gg/kqmJjExuCf"><img alt="Discord" src="https://img.shields.io/badge/Discord-JOIN-00b269?logo=discord&logoColor=%23ffffff&style=for-the-badge"></a>
<a href="http://qm.qq.com/cgi-bin/qm/qr?_wv=1027&k=IbDcH43vhfArb30luGMP1TMXB3GCHzxm&authKey=s%2FJfqvv46PswFq68irnGhkLrMR6y9tf%2FUn2mogYizSOGiS%2BmB%2B8Ar9I%2Fnr%2Bs4oS%2B&noverify=0&group_code=949562781"><img alt="QQ" src="https://img.shields.io/badge/QQ-JOIN-00b269?logo=tencentqq&logoColor=%23ffffff&style=for-the-badge"></a>
</p>

# Классические C# Threads для [DragonECS](https://github.com/DCFApixels/DragonECS)

<table>
  <tr></tr>
  <tr>
    <td colspan="3">Readme Languages:</td>
  </tr>
  <tr></tr>
  <tr>
    <td nowrap width="100">
      <a href="https://github.com/DCFApixels/DragonECS-ClassicThreads/blob/main/README-RU.md">
        <img src="https://github.com/user-attachments/assets/3c699094-f8e6-471d-a7c1-6d2e9530e721"></br>
        <span>Русский</span>
      </a>  
    </td>
    <td nowrap width="100">
      <a href="https://github.com/DCFApixels/DragonECS-ClassicThreads">
        <img src="https://github.com/user-attachments/assets/30528cb5-f38e-49f0-b23e-d001844ae930"></br>
        <span>English</span>
      </a>  
    </td>
  </tr>
</table>

</br>

Поддержка обработки сущностей в нескольких потоках, на основе классической реализации потоков в C#.

> [!WARNING]
> Проект в стадии разработки. API может меняться.  

# Оглавление
* [Установка](#Установка)
* [Параллельная итерация группы](#Параллельная-итерация-группы)

</br>

# Установка
Семантика версионирования - [Открыть](https://gist.github.com/DCFApixels/e53281d4628b19fe5278f3e77a7da9e8#file-dcfapixels_versioning_ru-md)
## Окружение
Обязательные требования:
+ Зависимость: [DragonECS](https://github.com/DCFApixels/DragonECS)
+ Минимальная версия C# 7.3;

Опционально:
+ Поддержка NativeAOT
+ Игровые движки с C#: Unity, Godot, MonoGame и т.д.

Протестировано:
+ **Unity:** Минимальная версия 2020.1.0;

## Установка для Unity
* ### Unity-модуль
Поддерживается установка в виде Unity-модуля в  при помощи добавления git-URL [в PackageManager](https://docs.unity3d.com/2023.2/Documentation/Manual/upm-ui-giturl.html) или ручного добавления в `Packages/manifest.json`: 
```
https://github.com/DCFApixels/DragonECS-ClassicThreads.git
```
* ### В виде исходников
Пакет так же может быть добавлен в проект в виде исходников.

</br>

# Параллельная итерация
``` csharp
EcsThreadHandler _handler;
public void Run(EcsPipeline pipeline)
{
    //Получение Аспекта и сущностей для итерации.
    var ee = _world.Where(out Aspect a);
    void Handler(ReadOnlySpan<int> entities)
    {
        foreach (var e in entities)
        {
            //Вычисления в отдельном потоке.
            a.poses.Get(e).position += a.velocities.Read(e).value * _time.DeltaTime;
        }
    }
    // Запускает параллельную итерацию по сущностям, 
    // сущности будут разбита на части с минимальным размером 1000.
    ee.IterateParallel(_handler ??= Handler, 1000);
}
```
> Чем меньше минимальный размер части группы при делении, тем больше потоков может быть задействовано, в некоторых ситуациях слишком много потоков может негативно повлиять на производительность.

> Внутри обработчика запрещено создавать/удалять сущности, запрещено добавлять/удалять компоненты на сущности. Допускается только модификация данных внутри компонентов.
