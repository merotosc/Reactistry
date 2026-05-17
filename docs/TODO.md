# TODO

## General

- [ ] refactor building extension methods to a Dictionary<BuildingType, BuildingDefinition>
- [ ] rename pipe to pipe
- [ ] decide if okay that ItemPath is set in producers when creating and in world when moving -> ev set or return path when consuming item
- [ ] save and load game data
- [ ] mirror option for buildings
- [ ] use state machine for buildings update loop
- [ ] pause/run simulation
- [ ] instantiate preview entity object to avoid messy entity options code
- [ ] check if buildings side by side without pipes do not create errors
- [ ] decide if use delta seconds or tick rate for entities speed
- [ ] extractor validation
- [ ] merger round robin
- [ ] use TryMoveItemToEntity for splitter merge to avoid new item creation
- [ ] level tasks and consumer
- [ ] level extractors
- [ ] use sprite pooling or other batching improvements for items
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

- [ ] start/end of 1xN tiles recognizable
- [ ] reaction, extraction and consumer animations
- [ ] display tile grid
- [ ] display building progress
- [ ] display invalid build positions
- [ ] display deletion outline on right-click hold
- [ ] display items inside buildings
- [ ] display molecule info on hover/click
- [ ] display warning for non-wired inputs/outputs
- [ ] scroll and zoom map
- [ ] display shortcut hints
- [ ] display tutorial hints
- [ ] esc to quit building mode
- [x] improve graphic (pipes instead of belts?)

## Editor

- [ ] orthogonal mode for pipes
- [ ] fix gaps when dragging pipes
- [ ] entity options (reactor inputs count)
- [x] remove multiple on drag
- [x] position multiple buildings on hold
- [x] position multiple pipes on hold
- [x] position pipes
- [x] auto-detect pipe directions
- [x] position buildings
- [x] rotate buildings
- [x] remove entities
- [x] shortcuts (entity type, rotation)
