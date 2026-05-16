# TODO

## General

- [ ] save and load game data
- [ ] mirror option for buildings
- [ ] use state machine for buildings update loop
- [ ] check properties that can become fields or private setter
- [ ] pause/run simulation
- [ ] rename entity to building
- [ ] keep moving items inside buildings
- [ ] instantiate preview entity object to avoid messy entity options code
- [ ] fix reactor errors directly after producer
- [ ] combine molecule in reactor based on recipes
- [ ] decide if use delta seconds or tick rate for entities speed
- [ ] extractor validation
- [ ] splitter
- [ ] merger
- [ ] split TryMoveItemToEntity in CanMoveToEntity and MoveToEntity to avoid item allocation in CreateItem
- [ ] use TryMoveItemToEntity for splitter merge to avoid new item creation
- [ ] level tasks and consumer
- [ ] level extractors
- [ ] use sprite pooling or other batching improvements for items
- [x] reactor with multiple inputs
- [x] buildings bigger than one tile

## UI

- [ ] display invalid build positions
- [ ] display items inside buildings
- [ ] display molecule info on hover/click
- [ ] scroll and zoom map
- [ ] display shortcut hints
- [ ] display tutorial hints
- [ ] improve graphic (pipes instead of belts?)

## Editor

- [x] position belts
- [ ] auto-detect belt directions
- [x] position buildings
- [x] rotate buildings
- [x] remove entities
- [ ] entity options (reactor inputs count)
- [x] shortcuts (entity type, rotation)
