# Configuration
Main configuration is done in *Custom Data* of programmable block that is running **ExcavOS**. If this data is empty then at first run it will be filled with default values:

```ini
[ExcavOS]
LiftThrustersGroupName=
StopThrustersGroupName=
CargoTrackGroupName=
AlignGyrosGroupName=
DumpSortersGroupName=
LiftThresholdWarning=0.9
```

## `LiftThrustersGroupName`
Requirement: **optional**

With this option you can specify block group with thrusters that should be used to calculate lifting thrust (ie. the required thrust to hover inside planet gravity). If left blank ExcavOS will guess grid orientation based on first found ship controller and select downward facing thrusters automatically.

## `StopThrustersGroupName`
Requirement: **optional**

With this option you can specify block group with thrusters that should be used to calculate stop distance (ie. how long and how far will grid travel until stopped by dampeners). If left blank ExcavOS will guess grid orientation based on first found ship controller and select forward facing thrusters automatically.

## `CargoTrackGroupName`
Requirement: **optional**

With this option you can specify block group with inventory that should be tracked in all cargo related information. Usually ship has more inventory space than it can fill with ores, ie. H2O2 generators, survival kits, ejectors, sorters, etc will artificially inflate available space. By default ExcavOS is tracking all inventories.

## `AlignGyrosGroupName`
Requirement: **optional**

With this option you can specify block group with gyroscopes used in gravity align feature. By default ExcavOS will claim all gyros on grid for this feature.

## `DumpSortersGroupName`
Requirement: **optional**

With this option you can specify block group with conveyor sorters that ExcavOS will use for setting up filters for dumping. By default ExcavOS uses all conveyor sorters found on grid.

## `LiftThresholdWarning`
Requirement: **optional**

With this option you can control threshold of warning that shows up on weight screen. By default it's `0.9` which means if you exceed 90% of available lift thrust you'll see blinking exclamation icon.