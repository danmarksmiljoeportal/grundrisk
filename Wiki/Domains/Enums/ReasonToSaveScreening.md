# ReasonToSaveScreening enum
Describing the reason why performs saving screening data into GrundRisk system
Value:
```
        None = 0,
        FirstTimeSaved = 1,
        AppliedOverrulingScreening = 10
```
- None: not available
- FirstTimeSaved: a location does not have any screening data before, as it is first time doing screening process and need to save.
- AppliedOverrulingScreening: while screening, system detected at least one rule of overruling screening rules is applied and need to save.
