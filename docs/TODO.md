# TODO

## General

- [ ] read level tasks from csv file
- [ ] remove GetItems in favor of GetInfo?
- [ ] helper method to determine output position and direction for item in building
- [ ] add try catch blocks to avoid game crash
- [ ] refactor building extension methods to a Dictionary<BuildingType, BuildingDefinition>
- [ ] decide if okay that ItemPath is set in producers when creating and in world when moving -> ev set or return path when consuming item
- [ ] save and load game data
- [ ] mirror option for buildings
- [ ] use state machine for buildings update loop
- [ ] pause/run simulation
- [ ] change simulation speed (0.5x, 2x, 4x)
- [ ] instantiate preview entity object to avoid messy entity options code -> not easy to implement since building instance is mostly immutable
- [ ] check if buildings side by side without pipes do not create errors
- [ ] decide if use delta seconds or tick rate for entities speed
- [ ] merger round robin
- [ ] splitter improve round robin
- [ ] upgrade system
- [ ] sandbox mode
- [ ] use sprite pooling or other batching improvements for items
- [x] level tasks
- [x] extraction sites
- [x] extractor validation
- [x] rename belt to pipe
- [x] consumer/hub
- [x] use TryMoveItemToEntity for splitter merge to avoid new item creation
- [x] splitter
- [x] split TryMoveItemToEntity in CanMoveToEntity and MoveToEntity to avoid item allocation in CreateItem -> updated item creation logic
- [x] merger
- [x] combine molecule in reactor based on recipes
- [x] keep moving items inside buildings
- [x] rename entity to building
- [x] check properties that can become fields or private setter
- [x] reactor with multiple inputs
- [x] buildings bigger than one tile

## UI

- [ ] molecules resource icon/tile
- [ ] atom numbers in formula in subscript
- [ ] reactions cheat sheet
- [ ] show debug infos (buildings & items count, item paths, building info)
- [ ] custom cursor
- [ ] start/end of 1xN tiles recognizable
- [ ] display more molecule information (formula, name)
- [ ] display building info such as state, progress
- [ ] display invalid build positions
- [ ] display warning for non-wired inputs/outputs
- [ ] display tutorial hints
- [ ] reaction, extraction and consumer animations
- [x] display shortcut hints
- [x] better buttons ui for hover, pressed
- [x] add transparency to ui panels
- [x] display items in buildings
- [x] better deletion gizmo/outline
- [x] tile grid smaller if far zoomed out -> removed completely if far away
- [x] display tile grid
- [x] pan and zoom map
- [x] esc to quit building mode
- [x] display deletion outline on right-click hold
- [x] improve graphic (pipes instead of belts?)

## Editor

- [ ] avoid switching item when dragging (messes up first building)
- [ ] orthogonal mode for pipes
- [ ] entity options (reactor inputs count)
- [ ] blueprints
- [ ] move building
- [ ] clone building
- [ ] copy-paste region selection
- [ ] region selection for deletion
- [x] fix gaps when dragging pipes
- [x] remove multiple on drag
- [x] position multiple buildings on hold
- [x] position multiple pipes on hold
- [x] position pipes
- [x] auto-detect pipe directions
- [x] position buildings
- [x] rotate buildings
- [x] remove entities
- [x] shortcuts (entity type, rotation)
