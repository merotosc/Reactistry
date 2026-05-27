# TODO

## General

### Prototype

- [ ] move to Azure Static Web App
- [ ] add try catch blocks to avoid game crash

### vNext

- [ ] reload game on save data clear
- [ ] keep tool when canceling operation with esc?
- [ ] rename merger and splitter. Distributor, Balancer, Divider, Combiner?
- [ ] use input map for key bindings
- [ ] option to pan and delete using toolbar and left-click
- [ ] rename buildingOptions to buildingData?
- [ ] save last camera position in save data
- [ ] save items in buildings in save data
- [ ] random seed every new game stored in save data
- [ ] save/load from/to json save data
- [ ] read atom colors from atoms.csv
- [ ] developer docs
- [ ] better csv reading avoiding duplicate code
- [ ] unlock rings based on levels
- [ ] remove GetItems in favor of GetInfo?
- [ ] helper method to determine output position and direction for item in building
- [ ] refactor building extension methods to a Dictionary<BuildingType, BuildingDefinition>
- [ ] decide if okay that ItemPath is set in producers when creating and in world when moving -> ev set or return path when consuming item
- [ ] mirror option for buildings
- [ ] use state machine for buildings update loop
- [ ] change simulation speed (0.5x, 2x, 4x)
- [ ] instantiate preview entity object to avoid messy entity options code -> not easy to implement since building instance is mostly immutable
- [ ] decide if use delta seconds or tick rate for entities speed
- [ ] upgrade system
- [ ] sandbox mode
- [ ] use sprite pooling or other batching improvements for items
- [ ] configurable user settings (autosave interval, zoom sensitivity)

### Done

- [x] pause/run simulation
- [x] check if buildings side by side without pipes do not create errors
- [x] button/shortcut to clear save data -> ctrl+shift+alt+x
- [x] use wasd for camera movement
- [x] avoid key spam on key hold (build and save controller)
- [x] write molecule formula as in csv file (CH4 instead of H4C)
- [x] merger improve round robin
- [x] splitter improve round robin
- [x] save tasks progress
- [x] save and load game data
- [x] rename project to Reactistry
- [x] read level tasks from csv file
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

### Prototype

- [ ] change icon of game
- [ ] display tutorial hints / printscreen

### vNext

- [ ] improve shortcuts and cheat sheet
    - left: build/use/select
    - right: delete
    - middle: pan
    - scroll: zoom
    - esc: cancel
    - 1-9: tool
    - r: rotate
    - t: variant (f instead?)
    - shift+r: rotate counterclockwise
    - shift+t(f?): variant counterclockwise
    - space: pause/run
    - ctrl+s: save
    - ctrl+shift+alt+x: clear
    - tab: cycle tools?
- [ ] auto tile 2d for lab
- [ ] display level/task number
- [ ] show selected tool/building in toolbar
- [ ] atom numbers in formula as subscript
- [ ] hide building tooltip if build tool active
- [ ] icon for invalid molecule
- [ ] reactions cheat sheet
- [ ] show debug infos (buildings & items count, item paths, building info)
- [ ] custom cursor (default, build, deletion)
- [ ] display more molecule information (formula, name)
- [ ] display building info such as state, progress
- [ ] display warning for non-wired inputs/outputs
- [ ] reaction, extraction and consumer animations
- [ ] pipes animations (e.g. moving bubbles)

### Done

- [x] blue arrow for extractor output
- [x] better consumer/destroyer tile
- [x] display save icon (saving... or saved)
- [x] start/end of 1xN tiles recognizable
- [x] hint tooltip on molecule resource
- [x] better molecules resource tile instead of same as molecule item
- [x] item creation and deletion animations
- [x] custom tile for different molecule resource
- [x] display chunks grid
- [x] molecules resource icon/tile
- [x] display invalid build positions
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

### vNext

- [ ] avoid switching item when dragging (messes up first building)
- [ ] display entity options better for each building (e.g. reactor inputs count)
- [ ] blueprints
- [ ] move building
- [ ] clone building
- [ ] copy-paste region selection
- [ ] region selection for deletion (with shift + right-click drag?)

### Done

- [x] set horizontal/vertical orthogonal mode priority based on direction from first to second tile
- [x] orthogonal mode for pipes
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
