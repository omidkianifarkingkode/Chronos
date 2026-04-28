# Chronos

Chronos is a lightweight Unity time toolkit that combines a trusted game clock, an action scheduler, and a centralized tick provider behind one easy static access point (`Chronos`).

## Installation (GitHub path)

Install Chronos through Unity Package Manager using a Git URL that points to the package folder:

```text
https://github.com/omidkianifarkingkode/Chronos.git?path=/Assets/Chronos
```

### Steps

1. Open **Unity** → **Window** → **Package Manager**.
2. Click **+** → **Add package from git URL...**.
3. Press **Add**.

## Setup (Menu Item)

After installation, create the required bootstrapper hierarchy from:

- **Window → Chrono → Setup Chronos**

This menu item creates (or finds) the Chronos bootstrapper and wires available modules so `Chronos.Clock`, `Chronos.Scheduler`, and `Chronos.TickProvider` are initialized at runtime.

## Modules

### Clock

The **Clock** module provides trusted local/server-backed time with tamper detection helpers.

#### Sample usage

```csharp
using UnityEngine;
using Kingkode.Chronos;

public class ClockExample : MonoBehaviour
{
    private void Start()
    {
        var nowUtc = Chronos.Clock.UtcNow;
        Debug.Log($"UTC Now: {nowUtc}");

        // Optional: sync when your backend returns server unix milliseconds
        // Chronos.Clock.SyncWithServer(serverUnixMs);
    }
}
```

Access via static field in `Chronos.cs`:

- `Chronos.Clock`

### Scheduler (Scheduer)

The **Scheduler** module runs delayed and repeated actions in a simple API.

#### Sample usage

```csharp
using System;
using UnityEngine;
using Kingkode.Chronos;

public class SchedulerExample : MonoBehaviour
{
    private void Start()
    {
        Chronos.Scheduler.After(TimeSpan.FromSeconds(3), () =>
        {
            Debug.Log("Runs once after 3 seconds");
        }, tag: "demo_once");

        Chronos.Scheduler.EverySecond(() =>
        {
            Debug.Log("Runs every second");
        }, tag: "demo_loop");
    }

    private void OnDestroy()
    {
        Chronos.Scheduler.CancelAll("demo_loop");
    }
}
```

Access via static field in `Chronos.cs`:

- `Chronos.Scheduler`

### TickProvider (optional module)

The **TickProvider** module emits app/gameplay ticks at a configurable tick rate.

#### Sample usage

```csharp
using UnityEngine;
using Kingkode.Chronos;

public class TickProviderExample : MonoBehaviour
{
    private void OnEnable()
    {
        if (Chronos.TickProvider != null)
        {
            Chronos.TickProvider.OnGameplayTick += OnGameplayTick;
        }
    }

    private void OnDisable()
    {
        if (Chronos.TickProvider != null)
        {
            Chronos.TickProvider.OnGameplayTick -= OnGameplayTick;
        }
    }

    private void OnGameplayTick(long tick, float deltaTime)
    {
        // gameplay tick logic
    }
}
```

Access via static field in `Chronos.cs`:

- `Chronos.TickProvider`
