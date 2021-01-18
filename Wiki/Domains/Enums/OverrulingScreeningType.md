# OverrulingScreeningType enum
Describing 10 overruling rules that are applying in screening process

Value:
```
        None = 0,
        ExceedingFactorChangedToLargerOrEqualOneRule = 10,
        ExceedingFactorChangedToSmallerThanOneRule = 20,
        ExceedingFactorExceedLimitationRule = 30,
        FlagsChangedRule = 40,
        PollutantsChangedRule = 50,
        PollutionCausesChangedRule = 60,
        ActivitiesChangedRule = 70,
        SumV1PolygonAreaExceedLimitationRule = 80,
        SumV2PolygonAreaExceedLimitationRule = 90,
        MissingModelCompoundsChangedRule = 100
```
- None: not available
- ExceedingFactorChangedToLargerOrEqualOneRule: mean the previous factor is below 1.0 or not exist, compare to current factor is 1.0 or higher
- ExceedingFactorChangedToSmallerThanOneRule: mean the previous factor is larger or equal 1.0, compare to current factor below 1.0  or not exist.
- ExceedingFactorExceedLimitationRule: mean the differentiate calculation between previous factor and current factor has rate of change larger than the configured limitation percentage.
- FlagsChangedRule: mean the previous screening flags is differ from current screening flags
- PollutantsChangedRule: mean the previous screening pollutant components are differ from current screening pollutant components
- PollutionCausesChangedRule: mean the previous screening pollution causes are differ from current screening pollution causes
- ActivitiesChangedRule: mean the previous screening activity compounds are differ from current screening activity compounds
- SumV1PolygonAreaExceedLimitationRule: mean the differentiate calculation between previous V1 polygon area and current V1 polygon area has rate of change larger than the configured limitation percentage.
- SumV2PolygonAreaExceedLimitationRule: mean the differentiate calculation between previous V2 polygon area and current V2 polygon area has rate of change larger than the configured limitation percentage.
- MissingModelCompoundsChangedRule: mean the previous screening having missing compounds (include missing pollutants, missing pollution causes, missing activities) are differ from current screening missing compounds