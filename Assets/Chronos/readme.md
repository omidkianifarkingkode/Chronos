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

## Configuration (ChronosSettings asset)

All Chronos data lives in a single `ChronosSettings` ScriptableObject owned by **your project**, never by the package. You configure Chronos without editing package source, scenes, or prefabs.

Create the asset from:

- **Window → Chrono → Create Settings Asset** (creates `Assets/Resources/ChronosSettings.asset`), or
- **Assets → Create → Chronos → Settings** (place it at the root of any `Resources` folder, keep the name `ChronosSettings`).

At runtime the bootstrapper resolves settings in this order:

1. The asset explicitly assigned to the `ChronosBootstrapper` component (optional override, useful for tests or per-build variants).
2. `Resources/ChronosSettings` anywhere in the project.
3. Built-in package defaults — Chronos works out of the box with no asset at all.

The asset contains:

| Section | Data |
| --- | --- |
| Logging | `LogEnabled`, `LogLevel` |
| Modules | `SchedulerEnable`, `TickingEnable` |
| Clock | `GameClockOptions` (overlay, skew/delta thresholds, cheat), `ServerTimeSyncOptions` (URL, sync triggers) |
| Scheduling | `ActionSchedulerOptions` (pool capacity, exception handling) |
| Ticking | `TickingOptions` (ticks per second, max ticks per frame) |

The resolved `ChronosSettings` instance is also registered in the DI container, so any service can take it as a dependency.

## Samples

Chronos package samples are now stored in `Samples~/Sample` to match Unity package sample conventions.

To import the sample into your project:

1. Open **Window → Package Manager**.
2. Select **Chronos** from the package list.
3. In the **Samples** tab, click **Import** for **Sample**.

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
