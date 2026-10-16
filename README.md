# Stage Amplifier and Whitelist

[![Latest Release](https://img.shields.io/github/release-date/Erallie/stage-amplifier-and-whitelist?display_date=published_at&style=for-the-badge&label=Latest%20Release)](https://github.com/Erallie/stage-amplifier-and-whitelist/releases/latest)
![Latest Downloads](https://img.shields.io/github/downloads/Erallie/stage-amplifier-and-whitelist/latest/total?sort=semver&style=for-the-badge&logo=Github&label=Latest%20Downloads)
![All-time Downloads](https://img.shields.io/github/downloads/Erallie/stage-amplifier-and-whitelist/total?style=for-the-badge&logo=GitHub&label=All-time%20Downloads)
<br>
[![Our Discord](https://img.shields.io/discord/1102582171207741480?style=for-the-badge&logo=discord&logoColor=ffffff&label=Our%20Discord&color=5865F2)](https://discord.gozarproductions.com)
[![Our Other Projects](https://img.shields.io/badge/Our%20Other%20Projects-%E2%9D%A4-563294?style=for-the-badge&logo=data%3Aimage%2Fwebp%3Bbase64%2CUklGRu4DAABXRUJQVlA4WAoAAAAQAAAAHwAAHwAAQUxQSGABAAABgFtbm5volyZTA%2BtibzK2H0w5sDkmhe3GmxrwxGg0839r%2FvkkOogIBW7bKB0c4%2BARYihzIqfd6dfO%2B%2B3XtHsq4jJhlIvcDRcgNB%2FeieQETorBHgghRtUYqwDs%2B4U4IpcvUB%2BVUPSK54uEnTwsUJoar2DeMpzLxQpeG5DH8lxyyfLivVYAwPBbkWdOBg3qFlqiLy679iHy9UDKMZRXmYxpCcusayTHG01K%2FEtatYWuj7oI9hL4BxsxVwhoP2mlAJJ%2BuuAflc6%2BEUCQTCX9EV87xBR2H75NxLZSpWiwzqdIm7ZO7uB3oEgZKbD9Nt3EmHweEPH1t1GNsZUbKeisiwjyTm5fA3SO1yCrADZXrV2PZQJPL1tjN4%2BxUL9ie1mJobzOnDwSx6ILiF%2FW%2BTUR4tcHx0UaV75JXC1a4g6Ky5dLcTSuy9q4HhTieF64Hy1A3GHB8gLLK2e92feuqnbfPK8IVlA4IGgCAACwDQCdASogACAAPk0cjEQioaEb%2BqwAKATEtgBOl7v9V3sHcA2wG4A3gD0APLP9jX9n%2F2jmqv5AZRh7J%2BN2fOx22iE%2F4TUsecFmY%2BSf1r%2BAP%2BTfzT%2FXdIB7KX7MtdIGr1A8H0jmrrfZvqButwOaYcLWYNRq5QgAAP7%2F%2FmIMpiVNn67QXpM1rrDmRS8Nr%2F6dhD%2Bq5e%2BM%2BAtUP1%2FxOj85Ol5y3ebjz%2BpHoOf%2FWW8a%2F2ojUaKVDkVqof%2Bv4f0f6ud8i58wusz%2Fyrj%2F%2BwnM3q0769dvK%2F%2BQe04xL49tkb9t6ylCqqezZtZGuGLJ%2F5iUrPqdYc%2F8VbYZfP%2FOpZP%2F4X4q%2BqS4gPOxzdINOe5PGv%2F0TS%2FJRf4LlFrFkrWtxlS8n40grV%2BKUu%2FiwzdQzImvwH81FxL1bZyTSsrYwMku1Pk9StTtWNjSR8ZWEYBH9eTn%2FvBERii5XaWOPJ%2FFVXtVQGbv%2BFRW5jbo9tfFDu%2BDHHf8LbgUd%2F8W8Id1AehBtRNsLQWbADmvF1QJU8x5tw%2FtTUwIoSaa%2F2jkcvyVHkAsb2qoIh1KF1pPdae%2BZaqjydy6nUa9agjrDk1G4pMhEUhH%2BV%2FIUe49MjhR%2FuxyFmwQ8dDogMyQ%2BdcSBa56Lwt1wyJ%2F22%2F5O98r6q6wiM63HyaYONd36W7br%2F0%2F6y2DZ3irAddj%2FRxntvr%2FbbChSYXAfEbO%2FD0G%2FFbMFqTHypodt9T6dAx%2BUjJYfHzFf%2FM3Ec%2FAtwbjc2gka6urN1MlSLb2VTS9Q5r8fkDzxZz6vu1OYUPUB1UFMIhYGvMATbxxoTmVhvpovzAc%2F8nbOjw3wAAA)](https://github.com/Erallie)
[![Donate](https://img.shields.io/badge/Donate-%24-563294?style=for-the-badge&logo=ko-fi&logoColor=FFFFFF&color=FF6433)](https://www.ko-fi.com/GozarProductions)

---

Voice amplification, stage access controls, and whitelist tools for
**VRChat worlds**, created by **Erallie**.

Use the standalone whitelist or optionally connect it to ArchiTech's
ProTV whitelist.

**Free for personal and commercial use with credit to Erallie.**
See [License and credits](#license-and-credits).

## Features

- Stage amplification with occupancy and whitelist controls.
- Microphone pickup amplification.
- Global amplification for a whitelisted user.
- Minimal trigger-based amplification without access restrictions.
- A standalone display-name whitelist.
- Optional authorization of the first instance master.
- Optional integration with ProTV authorization.
- Shared object toggles with whitelist-controlled interaction.
- Optional ownership transfer and object respawning.
- Local object and canvas visibility based on whitelist status.
- Optional control of ProTV's shared volume setting.
- Custom inspectors with descriptions, field help, and reference search buttons.

## Installation

Start with a VRChat Worlds project managed through the VRChat Creator
Companion, with the Worlds SDK and UdonSharp available.

### Through VCC

Add the the listing to VCC [here](https://erallie.github.io/stage-amplifier-and-whitelist/), then add
**Stage Amplifier and Whitelist** to your world project.

### Manual installation

Copy the package into:

`Packages/com.erallie.stage-amplifier`

A release package ZIP contains the package's contents. Extract those
contents into the directory above.

Preserve the included `.meta` files, and avoid installing duplicate
copies in both `Assets` and `Packages`.

Allow Unity to finish importing and compiling UdonSharp before
configuring the components.

The package manifest currently does not declare dependency versions.
Ensure the Worlds SDK, UdonSharp, and Unity UI are installed.

## Quick setup

These instructions will help you get a minimum viable product into your project. Further customization is required for building your world the way you want it:

1. Open **Example Scene Without ProTV**, or add the **Whitelist**
   and **Stage Amplifier** prefabs to your scene.
2. Add authorized players' exact VRChat display names to the whitelist.
3. Optionally enable **Include First Instance Master**.
4. Assign the same whitelist to the stage and other whitelist-controlled
   components. Use the **Find** buttons where available.
5. Assign the stage's **Barrier Collider** and
   **Teleport On Kick Transform**.
6. Configure the stage detection collider as a trigger. Use a separate,
   non-trigger collider as the physical barrier.
7. Assign **Update Listener** on the whitelist.
8. On the listener, click **Find and Add Whitelist Listeners**.
9. Add controls for whitelist-only access or global object toggling
   as needed.
10. Test in ClientSim and in a VRChat instance with multiple clients.

Copy example scenes into `Assets` before customizing them so package
updates do not overwrite your changes.

Use **Example Scene With ProTV** when the external ProTV assets are
installed.

## Stage access rules

The intended behavior is:

| Stage state | Whitelisted players | Unwhitelisted players |
| --- | --- | --- |
| Enabled, normal access, empty | May enter | May enter |
| Enabled, normal access, occupied | May enter | Blocked unless already the current performer |
| Enabled, whitelist-only access | May enter | Blocked |
| Globally off | Blocked | Blocked |

Whitelist-only access and global shutdown are separate controls:

- `StageWhitelistToggle` switches the stage's access mode.
- `WhitelistedToggle` enables or disables its assigned objects.

Global shutdown requires the scene to keep an appropriate physical
barrier active. Disabling an amplifier stops its trigger logic; it
does not create a barrier.

The current implementation tracks one `CurrentPerformer` name.
A whitelisted player may occupy that slot, while other whitelisted
players bypass the occupancy restriction. There is no separate queue
of unwhitelisted performers.

**Known issue:** occupied-stage access has been reported to block
whitelisted players. The rules above describe the intended behavior.
This documentation update does not resolve that runtime issue.

## Components

### Whitelist

Determines whether a player is authorized.

A player is authorized when any configured source permits them:

- The linked ProTV whitelist recognizes them as an authorized user
  or super user.
- Their display name exactly matches a local whitelist entry.
- **Include First Instance Master** is enabled and their name matches
  the initially recorded owner.

Local display-name matching is case-sensitive.

The initial owner name is recorded once through `OnPlayerJoined` by
the owner of the whitelist object. It is not replaced when the
instance master changes.

Recording the owner immediately calls the assigned update listener.

Keep the whitelist object enabled. Assign **Update Listener** so
dependent controls can refresh after initial owner recording.
Custom scripts that change authorization should also refresh the
listener.

### WhitelistUpdateListener

Sends the `WhitelistUpdated` custom event to its assigned listeners.

Refreshes happen at startup and when `UpdateUI()` is called.

With ProTV installed, this component derives from `TVPluginUI` and
registers with the assigned ProTV whitelist.

The **Find and Add Whitelist Listeners** button searches the current
scene, including inactive objects, for:

- `GlobalAmplifier`
- `StageWhitelistToggle`
- `ToggleWhenWhitelisted`
- `VolumeSyncToggle`
- `WhitelistedToggle`

It preserves existing assignments, fills empty slots, and avoids
adding duplicate references. It searches known component types
rather than examining arbitrary event names.

### StageAmplifier

Amplifies players inside its trigger and manages stage access.

Requires a `Collider`. Configure the detection collider as a trigger.

| Setting | Purpose |
| --- | --- |
| Whitelist | Determines authorization and occupancy bypass |
| Barrier Collider | Separate physical collider controlling local entry |
| Teleport On Kick Transform | Destination for rejected players |
| Mesh To Hide | Optional renderer hidden for the local performer |
| Canvas To Hide | Optional canvas hidden for the local performer |
| Voice Multiplier | Voice-distance multiplier; default `17` |
| Starting Volume | Minimum voice far distance; default `25` |

`WhitelistOnly` selects the access mode.

`CurrentPerformer` records the occupied slot.

Changing either property refreshes barrier state through
`WhitelistUpdated()`.

Exiting, respawning, or leaving resets the player's voice settings
and clears the slot if they were the recorded performer.

### StageWhitelistToggle

Lets whitelisted players switch a linked stage between normal
access and whitelist-only access.

Assign:

- The whitelist.
- The linked `StageAmplifier`.
- Optional mesh visibility and interaction text settings.

Use a collider for in-world interaction.

Interaction takes ownership of the control and linked stage, changes
their access state, and requests serialization.

This control does not globally disable the stage.

### Microphone

Amplifies the voice of the player holding the pickup.

Requires `VRCPickup` from `VRC.SDK3.Components`.

Configure the pickup's collider and Rigidbody as appropriate.
Dropping the microphone resets voice settings.

The microphone itself does not check whitelist authorization.
Control its availability through a separate whitelist component
when needed.

For synchronized movement and respawning through `WhitelistedToggle`,
place `VRCObjectSync` on the exact GameObject being toggled.

### GlobalAmplifier

Lets a whitelisted user interact to amplify their voice wherever
they move.

Requires a `Collider`.

The object's owner is the amplified player. The owner can switch
amplification off, and ownership changes also clear amplification.

**Hide Mesh If Not Whitelisted** optionally hides the control's
renderer from unauthorized players.

### MinimalAmplifier

Amplifies players inside a trigger without whitelist or occupancy
restrictions.

Requires a `Collider`, configured as a trigger.

Exiting, respawning, leaving, or disabling the component resets
voice settings.

### WhitelistedToggle

Lets whitelisted players toggle assigned objects using a shared
synced state.

Requires a `Collider`.

| Setting | Behavior |
| --- | --- |
| Whitelist | Authorization source; missing reference disables interaction |
| Hide Mesh If Not Whitelisted | Hides the control's renderer locally |
| Objects To Toggle | Objects enabled or disabled by shared state |
| Transfer Ownership | Takes ownership of explicitly listed targets |
| Child Objects | Additional ownership targets |
| Respawn On Disable | Respawns eligible targets when switching off |
| Menu Toggle | Optional Unity UI Toggle reflecting shared state |
| Interaction Text | Text displayed for the on and off states |

**Child Objects** appears only when **Transfer Ownership** is checked.

Ownership transfer does not automatically discover all descendants.
Add any additional ownership targets explicitly.

Respawning requires `VRCObjectSync` on each exact target in
**Objects To Toggle**. It runs for locally owned targets when the
toggle controller is locally owned.

Respawning may take ownership of target objects even when
**Transfer Ownership** is unchecked.

For an optional UI Toggle, connect its value-change event to the
Udon custom event `OnMenuToggleChanged`.

The displayed toggle state is updated with `SetIsOnWithoutNotify`.

Initial state comes from the first non-null target's `activeSelf`.

Keep the controller outside the hierarchy it disables so users can
turn the objects back on.

### ToggleWhenWhitelisted

Shows or hides assigned objects locally according to the local
player's authorization.

- **Toggle On** checked: objects are enabled for whitelisted players.
- **Toggle On** unchecked: objects are disabled for whitelisted players.

Assigned canvases are enabled for whitelisted players regardless
of **Toggle On**.

Keep this component on a separate, always-active controller rather
than on the whitelist object or an object it disables.

Refreshes use `WhitelistUpdated`.

### ToggleOthersOnActiveSelf

Enables assigned objects locally when this component is enabled,
and disables them when it is disabled.

It performs no whitelist checking or synchronization itself.

Avoid cyclic activation dependencies.

### VolumeSyncToggle

Lets authorized players change ProTV's shared volume-control setting.

Requires a `Collider`.

With ProTV installed, assign:

- The whitelist.
- The ProTV `TVManager`.

The component controls `TVManager.syncVolumeControl`.

Without ProTV, its TV manager field is excluded and interaction has
no effect on a TV system.

## Voice settings

The amplifiers calculate voice far distance using:

```text
max(Voice Multiplier × distance from listener to speaker, Starting Volume)
```

Defaults:

- **Voice Multiplier:** `17`
- **Starting Volume:** `25`

Despite its label, **Starting Volume** controls the minimum voice
far distance rather than voice gain.

Amplification disables voice low-pass filtering.

Resetting restores voice far distance to `25` and re-enables low-pass
filtering. It does not restore custom settings from another system.

Multiple voice systems affecting the same player can overwrite each
other's settings. Test combinations of microphones, stages, and
other voice scripts together.

## ProTV integration

ProTV-specific code is guarded by:

`ERALLIE_PROTV_PACKAGE`

Assembly version defines detect the VPM package:

`dev.architech.protv`

The integration uses:

- `TVManagedWhitelist` for authorization.
- `TVPluginUI` for whitelist update notifications.
- `TVManager` for shared volume control.

### Setup

1. Install ProTV and its dependencies separately.
2. Assign its whitelist to this package's `Whitelist`.
3. Assign the same ProTV whitelist to `WhitelistUpdateListener`.
4. Configure inherited TV plugin references where applicable.
5. Assign a TV manager to `VolumeSyncToggle`.
6. Populate the listener list and test authorization changes.

The standalone whitelist and first-master option are intended to work
without ProTV.

The package manifest does not declare ProTV as a required dependency.
However, the current runtime and editor assembly definitions contain
direct references to `ArchiTech.ProTV.Runtime` and `ArchiTech.SDK.Runtime`.

Conditional C# guards alone do not confirm a clean import without
those assemblies. Verify both installation states before publishing.

ProTV and ArchiTech dependencies remain under their own licenses.

## Inspector tools

Custom inspectors provide:

- A large component title.
- A component description.
- Help text beneath fields with tooltips.
- Find buttons for supported scene references.
- A listener discovery button.
- Conditional display of Child Objects.

Reference searches include inactive objects in the current scene.

If one match is found, it is assigned automatically. If multiple
matches are found, a selection menu appears.

Find buttons configure references in the editor; they do not
automatically repair missing references at runtime.

## Troubleshooting

| Problem | Check |
| --- | --- |
| Whitelisted player blocked by an occupied stage | Confirm the stage's own whitelist assignment and inspect other barrier controllers. The reported regression remains under investigation. |
| Control cannot be used | Check its whitelist reference, exact display name, collider, and listener refresh. |
| First master not recognized immediately | Check Include First Instance Master, Update Listener, listener assignments, and initialization order. |
| Player not teleported offstage | Assign Teleport On Kick Transform and confirm the stage trigger is active. |
| Microphone does not respawn | Enable Respawn On Disable and add VRCObjectSync to the exact toggled object. |
| ProTV fields missing | Confirm the package ID and assembly version define, then allow Unity to recompile. |
| Compile errors after removing ProTV | Inspect both assembly definitions for missing ArchiTech references as well as C# guards. |
| Different clients see different states | Check ownership, synchronization mode, late joining, and serialization. |
| Disabled objects cannot be re-enabled | Keep the toggle controller outside the hierarchy it disables. |

Missing-reference checks skip unavailable operations. They cannot
provide the feature associated with the missing reference.

ClientSim alone does not verify multi-client behavior. Test ownership,
synchronization, access rules, and late joining in VRChat.

This system controls world interactions; it is not a security boundary
against modified clients.

## Included assets

The package includes:

- Whitelist prefab.
- Stage Amplifier prefab.
- Microphone prefab.
- Stage and microphone toggle examples.
- Example scenes with and without ProTV.
- Stage and microphone models.

## License and credits

Copyright (c) 2026 **Erallie**.

Erallie's original materials are licensed under the
**Erallie Attribution License 1.0**. See [LICENSE.md](LICENSE.md).

Personal and commercial use, modification, redistribution, and sale
are permitted without royalties, subject to the license conditions.

When publishing a world or distributing a product using this package
or a modified version, credit:

> Stage Amplifier and Whitelist by Erallie.

For VRChat worlds, place credit in an accessible in-world credits
panel or the world's public description.

Preserve the license and copyright notices when redistributing
the package or source assets.

Unity, VRChat, UdonSharp, ProTV, ArchiTech SDK, the upstream VPM
template, and other third-party materials retain their respective
licenses. This license does not relicense those materials or imply
endorsement by their authors.
