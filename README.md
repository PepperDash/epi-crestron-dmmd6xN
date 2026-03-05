# Dm Md 6xN Plugin

A most basic plugin for Dm Md 6xN functionality.  Currently only the 6x4 is supported.  Attaching DM endpoints is fully supported.
<!-- START Minimum Essentials Framework Versions -->
### Minimum Essentials Framework Versions

- 1.14.3
<!-- END Minimum Essentials Framework Versions -->
<!-- START Config Example -->
### Config Example

```json
{
    "key": "GeneratedKey",
    "uid": 1,
    "name": "GeneratedName",
    "type": "DmMd6x4Device",
    "group": "Group",
    "properties": {
        "Control": "SampleValue"
    }
}
```
<!-- END Config Example -->
<!-- START Supported Types -->

<!-- END Supported Types -->
<!-- START Join Maps -->
### Join Maps

#### Digitals

| Join | Type (RW) | Description |
| --- | --- | --- |
| 1 | R | DM Chassis online |
| 1 | R | DM Chassis video enter |
| 2 | R | DM Chassis audio enter |
| 3 | R | DM Chassis enable audio breakaway routing |

#### Analogs

| Join | Type (RW) | Description |
| --- | --- | --- |
| 11 | R | DM Chassis video routes |
| 21 | R | DM Chassis video routes |

#### Serials

| Join | Type (RW) | Description |
| --- | --- | --- |
| 1 | R | DM Chassis input names |
| 11 | R | DM Chassis output names |
<!-- END Join Maps -->
<!-- START Interfaces Implemented -->

<!-- END Interfaces Implemented -->
<!-- START Base Classes -->
### Base Classes

- CrestronGenericBridgeableBaseDevice
- JoinMapBaseAdvanced
<!-- END Base Classes -->
<!-- START Public Methods -->

<!-- END Public Methods -->
<!-- START Bool Feedbacks -->
### Bool Feedbacks

- EnableAudioBreakawayFeedback
<!-- END Bool Feedbacks -->
<!-- START Int Feedbacks -->

<!-- END Int Feedbacks -->
<!-- START String Feedbacks -->

<!-- END String Feedbacks -->
