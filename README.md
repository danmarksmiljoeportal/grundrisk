
Grundrisk API integration guide to Preliminary screening endpoint.

# 1. The code

## .net core client with security
Dmp.Examples.GrundriskIntegration is the The .net core client is located in the folder of the same name.

It does a full codeflow login and then calls the grundrisk preliminary screening. Remember to look into "1. Security and access to the endpoint" as you would need the client id and secret.

It is located in the folder DMP.examples.GrundriskIntegration and it is also found here:

https://github.com/danmarksmiljoeportal/grundrisk/tree/master/Dmp.Examples.GrundriskIntegration

## How to generate C# API Client from swagger via NSwagStudio
- Download NSwagStudio and document at here: https://github.com/RicoSuter/NSwag/wiki/NSwagStudio
- After installing, using `GrundriskApiClient.nswag` file in `Dmp.Examples.GrundriskIntegration` folder and generate API Client
- There is issue with `Required = Newtonsoft.Json.Required.DisallowNull` when generate the code (https://github.com/RicoSuter/NSwag/issues/1991 ). So, we need to add custom json settings for API Client
```csharp
public partial class GrundriskClient
{
    partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
    {
        settings.ContractResolver = new SafeContractResolver();
    }
}

public class SafeContractResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var jsonProp = base.CreateProperty(member, memberSerialization);
        jsonProp.Required = Required.Default;
        return jsonProp;
    }
}
```
- Refer the example `GrundriskClient.cs` and `GrundriskClient.Setting.cs` in code

## Spa that show data
The code consist of a SPA using Angular which shows how to fetch data and a .net core client and has no security included and is is intended to show how data can be displayed.



# 2. Security and access to the endpoint

In order to communicate with the endpoint `/screenings/preliminary` the code has to use the OAUTH codeflow against the endpoint. That means you have to contact DMP for registration on the DMP useradm for both test and production.

You will need the role miljoe_grundrisk_foreloebigscreening in order to acces the endpoint.

Please contact Danmarks Mlijøportal's support at support@miljoeportal.dk to get a client id and client secret for authorization.

## API urls

| Environment | Url |Swagger | Swagger definintion|
| ----------- | ---------------- |----|---|
| TEST        |  https://grundrisk-api.test.miljoeportal.dk/preliminaryScreenings | https://grundrisk-api.test.miljoeportal.dk/swagger/ |https://grundrisk-api.test.miljoeportal.dk/swagger/v1/swagger.json |
| DEMO        | https://grundrisk-api.demo.miljoeportal.dk/preliminaryScreenings | https://grundrisk-api.demo.miljoeportal.dk/swagger/ |https://grundrisk-api.demo.miljoeportal.dk/swagger/v1/swagger.json |
| PROD        | https://grundrisk-api.miljoeportal.dk/preliminaryScreenings | https://grundrisk-api.miljoeportal.dk/swagger/ |https://grundrisk-api.miljoeportal.dk/swagger/v1/swagger.json |

## Web urls

| Environment | Url |
| ----------- | ---------------- |
| TEST        |  https://grundrisk.test.miljoeportal.dk/ |
| DEMO        | https://grundrisk.demo.miljoeportal.dk/ |
| PROD        | https://grundrisk.miljoeportal.dk/ |


The production is not ready before July 1. 2020.

# 3. Danmarks Miljøportal's Identity Provider
Danmarks Miljøportal's identity provider supports OpenID Connect, a simple identity layer on top of the OAuth 2.0 protocol, which allows computing clients to verify the identity of an end-user based on the authentication performed by an authorization server, as well as to obtain basic profile information about the end-user in an interoperable and REST-like manner. In technical terms, OpenID Connect specifies a RESTful HTTP API, using JSON as a data format.

OpenID Connect allows a range of clients, including Web-based, mobile, and JavaScript clients, to request and receive information about authenticated sessions and end-users. The specification suite is extensible, supporting optional features such as encryption of identity data, discovery of OpenID Providers, and session management.

OpenID Connect defines a discovery mechanism, called OpenID Connect Discovery, where an OpenID server publishes its metadata at a well-known URL. The discovery documents are available on the following URL's for the test and production environment respectively.

https://log-in.test.miljoeportal.dk/runtime/oauth2/.well-known/openid-configuration

https://log-in.miljoeportal.dk/runtime/oauth2/.well-known/openid-configuration


The identity provider supports the OAuth 2.0 / OpenID Connect flow ``Authorization code``.

The Authorization Code grant type is used by confidential and public clients to exchange an authorization code for an access token.

After the user returns to the client via the redirect URL, the application will get the authorization code from the URL and use it to request an access token.

# 4. The screening flags in the response from the API endpoint `/preliminaryScreenings`

When you receive a response from the preliminary screenings for each compound the following applies:

1. The value can be "flag":9 , which is 1001 in the binary representation.

2. The enums for calculating which flags that we can derive of the flag value, we use the bitwise left operation. The flag enums are like this:

```
    None = 0 << 0,
    Bly_kobber_eller_PAH = 1 << 0,
    Kompleks_geologi = 1 << 1,
    Manglende_modelstof = 1 << 2,
    Svag_geologi = 1 << 3,
    MTBE_fjernet = 1 << 4,
    Boring_uden_ler = 1 << 5,
    Risiko_pga_vandindvinding = 1 << 6,
```

In our example where the value is decimal 9 and 1001 as binary, can we now derive the following:
 1. Bly_Kobber_eller_PAH is lit on as this is the first 1 in the 1001
 2. Svag_geologi is also lit as we have 1 in the end 1001.


Next is to put at more wording on the flag enum - here do we use this:


- Bly_kobber_eller_PAH
  - Bly, kobber eller PAH fjernet i trin 1
- Kompleks_geologi
  - Område med kompleks geologi
- Manglende_modelstof
  - Et eller flere forureningsstoffer mangler et modelstof
- Svag_geologi
  - Svag datadækning for bestemmelse af dæklagstykkelser
- MTBE_fjernet
  - MTBE fjernet
- Boring_uden_ler
  - Lokalitet har boring uden ler i en radius af 100-300 m fra kant
- Risiko_pga_vandindvinding
  - Lokalitet udgør en risiko fordi der ligger en vandindvindingsboring indenfor 100 m

# 5. Standard parameters in the response

When you receive a response from the preliminary screenings for each compound there is also an section called standardParameters.

The values have this description:

\- Infiltration
* Infiltration

\- headGradient
* Hydraulisk gradient

\- aquiferDepth
* Dybde til grundvandsmagasin

\- horizontalHydraulicConductivity
* Hydraulisk konduktivitet

\- porosity
* Porøsitet

\- firstOrderDegradationRate
* Nedbrydningsrate

\- distNearestWaterWell
* Afstand til nærmeste indvinding

\- distNoClay
* Afstand til nærmeste boring uden ler i dæklag

# 6. The premilinary screening and riskassessment web links in the response

The response will show the links:
- `Preliminary screening link: https://grundrisk.demo.miljoeportal.dk/screening/preliminary/location-details/{response.LocationId}`
- `Preliminary riskassessment link: https://grundrisk.demo.miljoeportal.dk/riskassessment/preliminary/location-details/{response.LocationId}`

# 7. Landfills
The preliminary screening will now scan the input for the following activities or pollutioncause in order to determine if the input constitutes a landfill location.
1. Activitites is located here: https://dkjord-api.demo.miljoeportal.dk/api/locations/landfills/activities

and has currently the codes
```
	['063', '065', '066', '067', '068', '069']
```
2. Pollutioncauses is located here: https://dkjord-api.demo.miljoeportal.dk/api/locations/landfills/pollutioncauses

and has currently the codes
```
	['90.02.00', '90.02.10', '90.02.20']
```
3. If it does match one of the code it will constitute landfill it will get the following compounds added which then will be screened along with the other input.

The compounds that are added are obtained from: https://dkjord-api.demo.miljoeportal.dk/api/locations/landfills/pollutants

and has currently the values

| PollutantCode | PollutantName   | PollutantCompoundGroup      | Concentration | ModelCompoundCode | ModelCompoundName |
|---------------|-----------------|-----------------------------|---------------|-------------------|-------------------|
| 380           | Carbon,org,NVOC | Perkolatparametre           | 73000         | 551               | Kem.iltf. COD     |
| 662           | Benzen          | Benzen                      | 8,5           | 662               | Benzen            |
| 1014          | Ammonium-N      | Perkolatparametre           | 59000         | 551               | Kem.iltf.COD      |
| 1511          | Arsen           | Metaller                    | 12,5          | 1511              | Arsen             |
| 2041          | Jern            | Perkolatparametre           | 55000         | 551               | Kem.iltf.COD      |
| 2618          | Trichlorethylen | Chlorerede opløsningsmiddel | 1,1           | 2618              | Trichlorethylen    |
| 2676          | Phenol          | Phenoler                    | 3,2           | 2676              | Phenol            |
| 3105          | Chlorbenzen     | Chlorbenzen                 | 50            | 3105              | Chlorbenzen       |
| 4512          | Mechlorprop     | Pesticider-Phenoxysyrer     | 220           | 4512              | Mechlorprop       |
| 4515          | Atrazin         | Atrazin                     | 3,4           | 4515              | Atrazin           |

4. CompoundOrigin enum:

[Definition of CompoundOrigin enum](./Wiki/Domains/Enums/CompoundOrigin.md)

* If IsLandfill of preliminary screening in the response is true, the list of preliminary screening contains 10 results that have CompoundOrigin is Landfill

# 8. Removal reasons
The removal reasons are set if the Removed parameter is set to true.

At this case the removalreason will have an value like for instance 13.

To translate this into the removal reasons shown on the grundrisk web use this:

	NotRemoved = 0, // default value
        [Description('MTBE fjernet ifm. olie-miljø-pulje'],)]
        Removed_0_1 = 01,
        [Description(" 'MTBE fjernet, grundet aktiviteter vedr. værksted/olie']]
        Removed_0_2 = 02,
        [Description("'Fjernet grundet modelstof NVOC/COD/Ammonium/PAH/Bly/Kobber'])]
        Removed_1_1 = 11,
        [Description(Fjernet grundet stofgruppe PAH/Bly/Kobber']]
        Removed_1_2 = 12,
        [Description("'Forureningsfladen er 0')]
        Removed_1_3 = 13,
        [Description("'Grundet dæklagets tykkelse forventes det ikke, at forureningsstoffet når grundvandet'],]
        Removed_2_1 = 21,
        [Description('Fjernet grundet bestemte stoffer']
        Removed_2_2 = 22,
        [Description ('GVK er større end koncentration efter vertikal transport']
        Removed_2_3 = 23,
        [Description('MTBE fjernet grundet dæklagstykkelsen]
        Removed_2_4 = 24,
        // Step 3
        [Description('GVK er større end koncentration efter horisontalt transport)]
        Removed_3_1 = 31


# 9. CompoundTranslationType enum:
CompoundTranslationType is a enum with these values: Activity, PollutionCause, Pollutant
```
    Activity = 0,
    PollutionCause = 1,
    Pollutant = 2
```

If the result is V1 and from Activity, CompoundTranslationType is Activity

If the result is V1 and from PollutionCause, CompoundTranslationType is PollutionCause

If the result is V2, CompoundTranslationType is Pollutant


# 10. Test input  for preliminary screening

It produces produces 2 flags  - value 8 and 9 and standard parameters

* Request
```json
{
  "locationNumber": "000-00003",
  "pollutantComponentCodes": [
    "0703",
    "0490"
  ],
  "activities": [
    {
      "activityCode": "999",
      "pollutionCauseCode": "50.20.10"
    },
    {
      "activityCode": "006",
      "pollutionCauseCode": "50.50.00"
    },
    {
      "activityCode": "999",
      "pollutionCauseCode": "25.12.00"
    }
  ],
  "v1ShapeWkts": [
    "POLYGON ((554931.9389 6145817.3598, 554943.7005 6145814.4546, 554943.7377 6145814.4366, 554957.3742 6145803.8961, 554963.2915 6145805.0073, 554963.3599 6145804.9957, 554982.2349 6145794.1282, 554982.2398 6145794.1252, 554983.5714 6145793.2533, 554995.1822 6145842.5366, 554992.8408 6145844.0876, 554965.4258 6145862.191, 554960.6086 6145861.1682, 554948.7069 6145842.9933, 554957.3674 6145837.738, 554957.4005 6145837.5998, 554948.9235 6145823.9228, 554948.7887 6145823.8888, 554939.6435 6145829.1403, 554931.9389 6145817.3598))",
    "POLYGON ((554944.9397 6145837.2277, 554939.7532 6145829.3079, 554948.8044 6145824.1104, 554957.1773 6145837.6194, 554948.5973 6145842.8258, 554944.9397 6145837.2278, 554944.9397 6145837.2277))"
  ],
  "v2ShapeWkts": [
    "POLYGON ((554944.9397 6145837.2277, 554939.7532 6145829.3079, 554948.8044 6145824.1104, 554957.1773 6145837.6194, 554948.5973 6145842.8258, 554944.9397 6145837.2278, 554944.9397 6145837.2277))"
  ]
}

```

* Response (part of the response)

```json
Response (part of the response)
"polygonArea": 78.5398178100586,     "factor": 0,     "compoundName": "Bly",     "industryName": "Servicestationer",     "activityName": "Benzin og olie, salg af", "flag": 9,
```

```json
"standardParameters": {       "infiltration": 100,       "aquiferDepth": 14.34807491,       "headGradient": 0.007,       "lithoCode": 1,       "distNearestWaterWell": 675.1535179323225,       "distNoClay": 669.5346970240031,       "porosity": 0,       "horizontalHydraulicConductivity": 0,       "firstOrderDegradationRate": 0     },
```
For each compound do you get this result:
```json
{
  "id": "b2d971a3-7e50-4cd2-8433-ac870031ab66",
  "createdAt": "2020-12-04T03:00:58.5102264+00:00",
  "hasV1Polygon": true,
  "hasV2Polygon": true,
  "v1PolygonResult": {
    "type": 0,
    "hasData": true,
    "factor": 303.41983658441023,
    "polygonStatus": 1,
    "reassessment": null,
    "pollutant": "MTBE"
  },
  "v2PolygonResult": {
    "type": 1,
    "hasData": true,
    "factor": 438.2789431697559,
    "polygonStatus": 1,
    "reassessment": null,
    "pollutant": "MTBE"
  },
  "noModelCompounds": false,
  "noScreeningInputs": false,
  "isLandfill": false,
  "preliminaryScreeningResults": [
    {
      "locationId": "ec072d62-d5ad-4f07-ab24-ac870031ab79",
      "preliminaryScreeningId": "b2d971a3-7e50-4cd2-8433-ac870031ab66",
      "createdAt": "2020-12-04T03:01:02.2635272+00:00",
      "status": 2,
      "industry": null,
      "activity": null,
      "compoundName": "Benzin",
      "compoundCasNr": "",
      "qualityCriterion": 9,
      "worstCaseConcentration": 8000,
      "coverThickness": 14.05306862091965,
      "dataQuality": 2,
      "concentrationDownstream": 0.039270165749594695,
      "removed": false,
      "removalReason": 0,
      "logInfo": [
        2,
        5
      ],
      "concTopTables": 13.619023375377187,
      "conc100mGrundRisk": 0.039270165749594695,
      "factor": 0.004363351749954966,
      "flag": 96,
      "polygonType": 1,
      "polygonArea": 163.87679669065588,
      "standardParameters": {
        "infiltration": 100,
        "aquiferDepth": 14.34807491,
        "headGradient": 0.00402756,
        "lithoCode": 1,
        "distNearestWaterWell": 99.99,
        "distNoClay": 238.13659333991328,
        "porosity": 0.3,
        "horizontalHydraulicConductivity": 0.0001,
        "firstOrderDegradationRate": 0.003
      },
      "compoundOrigin": 1,
      "sourceSize": 163.87679669065588,
      "compoundTranslationType": 2,
      "id": "0b2b54c6-aff0-459d-b527-ac870031b937"
    },
    {
      "locationId": "ec072d62-d5ad-4f07-ab24-ac870031ab79",
      "preliminaryScreeningId": "b2d971a3-7e50-4cd2-8433-ac870031ab66",
      "createdAt": "2020-12-04T03:01:02.2635287+00:00",
      "status": 2,
      "industry": null,
      "activity": null,
      "compoundName": "MTBE",
      "compoundCasNr": "1634044",
      "qualityCriterion": 5,
      "worstCaseConcentration": 50000,
      "coverThickness": 14.05306862091965,
      "dataQuality": 2,
      "concentrationDownstream": 2191.3947158487795,
      "removed": false,
      "removalReason": 0,
      "logInfo": [
        2
      ],
      "concTopTables": 50000,
      "conc100mGrundRisk": 2191.3947158487795,
      "factor": 438.2789431697559,
      "flag": 96,
      "polygonType": 1,
      "polygonArea": 163.87679669065588,
      "standardParameters": {
        "infiltration": 100,
        "aquiferDepth": 14.34807491,
        "headGradient": 0.00402756,
        "lithoCode": 1,
        "distNearestWaterWell": 99.99,
        "distNoClay": 238.13659333991328,
        "porosity": 0.3,
        "horizontalHydraulicConductivity": 0.0001,
        "firstOrderDegradationRate": 0
      },
      "compoundOrigin": 1,
      "sourceSize": 163.87679669065588,
      "compoundTranslationType": 2,
      "id": "bf2f3e6c-47b6-414b-9a90-ac870031b937"
    },
	...
  ],
  "locationId": "ec072d62-d5ad-4f07-ab24-ac870031ab79",
  "locationNumber": "000-00003"
}
 ```

# 11. Overruling screening
Overruling screening rules are applied to determine whether or not screening data will be saved or discard by applying several rules.
There are 10 rules to be applied.
- Comparison rule 1: Save screening if exceedingfactor changes from <1 to >1
- Comparison rule 2: Save screening if exceedingfactor changes from >1 to <1
- Comparison rule 3: Save screening if exceeding factor changes more than 5%(configurable)
- Comparison rule 4: Save screening if any changes in screening flags
- Comparison rule 5: Save screening if there is changes in pollutants
- Comparison rule 6: Save screening if there is changes in pollutioncauses
- Comparison rule 7: Save screening if there is changes in activities
- Comparison rule 8: Save screening if sum of V1 polygon areas exceeds 5% (configurable)
- Comparison rule 9: Save screening if sum of V2 polygon areas exceeds 5% (configurable)
- Comparison rule 10: Save screening if there is changes in missing activities/pollutioncause/pollutants

[Definition of OverrulingScreeningType enum](./Wiki/Domains/Enums/OverrulingScreeningType.md)

# 12. Screening log
One of the output after doing a screening is the screening log.
Then this screening log data will be used in many places like the Jar endpoints and load screening details
* Schema:
 ```json
"screeningLog": {
    "newScreeningAt": "2021-01-15T04:34:05.440Z",
    "previousScreeningAt": "2021-01-15T04:34:05.440Z",
    "reasonToSave": 0,
    "overrulingScreeningRulesApplied": [
      0
    ],
    "addedFlags": 0,
    "removedFlags": 0,
    "addedPollutants": [
      "string"
    ],
    "removedPollutants": [
      "string"
    ],
    "addedPollutionCauses": [
      {
        "compoundName": "string",
        "industry": {
          "name": "string",
          "codeValue": "string"
        }
      }
    ],
    "removedPollutionCauses": [
      {
        "compoundName": "string",
        "industry": {
          "name": "string",
          "codeValue": "string"
        }
      }
    ],
    "addedActivities": [
      {
        "compoundName": "string",
        "activity": {
          "name": "string",
          "codeValue": "string",
          "translatedCompound": "string",
          "type": 0
        }
      }
    ],
    "removedActivities": [
      {
        "compoundName": "string",
        "activity": {
          "name": "string",
          "codeValue": "string",
          "translatedCompound": "string",
          "type": 0
        }
      }
    ],
    "addedMissingActivities": [
      {
        "code": "string",
        "description": "string",
        "translatedCompound": "string",
        "type": 1
      }
    ],
    "removedMissingActivities": [
      {
        "code": "string",
        "description": "string",
        "translatedCompound": "string",
        "type": 1
      }
    ],
    "addedMissingPollutionCauses": [
      {
        "code": "string",
        "description": "string",
        "translatedCompound": "string",
        "type": 1
      }
    ],
    "removedMissingPollutionCauses": [
      {
        "code": "string",
        "description": "string",
        "translatedCompound": "string",
        "type": 1
      }
    ],
    "addedMissingPollutantComponents": [
      {
        "code": "string",
        "description": "string",
        "translatedCompound": "string"
      }
    ],
    "removedMissingPollutantComponents": [
      {
        "code": "string",
        "description": "string",
        "translatedCompound": "string"
      }
    ],
    "addedV1PolygonAreaInTotal": 0,
    "removedV1PolygonAreaInTotal": 0,
    "addedV2PolygonAreaInTotal": 0,
    "removedV2PolygonAreaInTotal": 0,
    "addedExceedFactor": 0,
    "removedExceedFactor": 0,
    "reassessedToStatus": 0,
    "reassessmentComment": "string",
    "screeningTriggeredBy": 0
  }
```

[Definition of ReasonToSaveScreening enum](./Wiki/Domains/Enums/ReasonToSaveScreening.md)

[Definition of ScreeningTriggeredBy enum](./Wiki/Domains/Enums/ScreeningTriggeredBy.md)

# 13. Jar endpoints
There is a need to provide latest screening data together with screening log and risk calculations data to Jar consuming those data, thus some endpoints are created to adapt this need. Also, Jar's events are created to do notification jobs to Jar system side once there is a new screening or a new risk calculation being started or updated.

- Endpoint to fetch latest screening, screening results data together with screening log:
  - /jar/locations/{locationNumber}/screenings
* Response body:
``` json
{
  "locationNumber": "string",
  "totalFlags": 0,
  "v1ScreeningResults": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "createdAt": "2021-01-08T04:29:02.795Z",
      "status": 0,
      "industry": {
        "name": "string",
        "codeValue": "string"
      },
      "activity": {
        "name": "string",
        "codeValue": "string",
        "translatedCompound": "string",
        "type": 0
      },
      "compoundName": "string",
      "compoundCasNr": "string",
      "qualityCriterion": 0,
      "worstCaseConcentration": 0,
      "coverThickness": 0,
      "dataQuality": 0,
      "concentrationDownstream": 0,
      "removed": true,
      "removalReason": 0,
      "factor": 0,
      "flag": 0,
      "polygonType": 0,
      "polygonArea": 0,
      "standardParameters": {
        "infiltration": 0,
        "aquiferDepth": 0,
        "headGradient": 0,
        "lithoCode": 0,
        "distNearestWaterWell": 0,
        "distNoClay": 0,
        "porosity": 0,
        "horizontalHydraulicConductivity": 0,
        "firstOrderDegradationRate": 0
      },
      "compoundOrigin": 0,
      "sourceSize": 0,
      "compoundTranslationType": 0
    }
  ],
  "v2ScreeningResults": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "createdAt": "2021-01-08T04:29:02.795Z",
      "status": 0,
      "industry": {
        "name": "string",
        "codeValue": "string"
      },
      "activity": {
        "name": "string",
        "codeValue": "string",
        "translatedCompound": "string",
        "type": 0
      },
      "compoundName": "string",
      "compoundCasNr": "string",
      "qualityCriterion": 0,
      "worstCaseConcentration": 0,
      "coverThickness": 0,
      "dataQuality": 0,
      "concentrationDownstream": 0,
      "removed": true,
      "removalReason": 0,
      "factor": 0,
      "flag": 0,
      "polygonType": 0,
      "polygonArea": 0,
      "standardParameters": {
        "infiltration": 0,
        "aquiferDepth": 0,
        "headGradient": 0,
        "lithoCode": 0,
        "distNearestWaterWell": 0,
        "distNoClay": 0,
        "porosity": 0,
        "horizontalHydraulicConductivity": 0,
        "firstOrderDegradationRate": 0
      },
      "compoundOrigin": 0,
      "sourceSize": 0,
      "compoundTranslationType": 0
    }
  ],
  "screeningLog": {
    "newScreeningAt": "2021-01-08T04:29:02.795Z",
    "previousScreeningAt": "2021-01-08T04:29:02.795Z",
    "reasonToSave": 0,
    "overrulingScreeningRulesApplied": [
      0
    ],
    "addedFlags": 0,
    "removedFlags": 0,
    "addedPollutants": [
      "string"
    ],
    "removedPollutants": [
      "string"
    ],
    "addedPollutionCauses": [
      {
        "compoundName": "string",
        "industry": {
          "name": "string",
          "codeValue": "string"
        }
      }
    ],
    "removedPollutionCauses": [
      {
        "compoundName": "string",
        "industry": {
          "name": "string",
          "codeValue": "string"
        }
      }
    ],
    "addedActivities": [
      {
        "compoundName": "string",
        "activity": {
          "name": "string",
          "codeValue": "string",
          "translatedCompound": "string",
          "type": 0
        }
      }
    ],
    "removedActivities": [
      {
        "compoundName": "string",
        "activity": {
          "name": "string",
          "codeValue": "string",
          "translatedCompound": "string",
          "type": 0
        }
      }
    ],
    "addedMissingActivities": [
      {
        "code": "string",
        "description": "string",
        "translatedCompound": "string",
        "type": 1
      }
    ],
    "removedMissingActivities": [
      {
        "code": "string",
        "description": "string",
        "translatedCompound": "string",
        "type": 1
      }
    ],
    "addedMissingPollutionCauses": [
      {
        "code": "string",
        "description": "string",
        "translatedCompound": "string",
        "type": 1
      }
    ],
    "removedMissingPollutionCauses": [
      {
        "code": "string",
        "description": "string",
        "translatedCompound": "string",
        "type": 1
      }
    ],
    "addedMissingPollutantComponents": [
      {
        "code": "string",
        "description": "string",
        "translatedCompound": "string"
      }
    ],
    "removedMissingPollutantComponents": [
      {
        "code": "string",
        "description": "string",
        "translatedCompound": "string"
      }
    ],
    "addedV1PolygonAreaInTotal": 0,
    "removedV1PolygonAreaInTotal": 0,
    "addedV2PolygonAreaInTotal": 0,
    "removedV2PolygonAreaInTotal": 0,
    "addedExceedFactor": 0,
    "removedExceedFactor": 0,
    "reassessedToStatus": 0,
    "reassessmentComment": "string",
    "screeningTriggeredBy": 0
  }
}
```
- Endpoint to fetching location, risk calculations data:
  - /jar/locations/{locationNumber}/riskCalculations
* Response body:
``` json
{
  "locationNumber": "string",
  "locationStatus": "string",
  "approvedCalculations": [
    {
      "createdAt": "2021-01-08T04:30:52.567Z",
      "compoundName": "string",
      "compoundCasNr": "string",
      "exceedingFactor": 0,
      "calculationModelType": 0,
      "calculationFailed": true,
      "calculationFailedErrorMessage": "string"
    }
  ],
  "unapprovedCalculations": [
    {
      "createdAt": "2021-01-08T04:30:52.567Z",
      "compoundName": "string",
      "compoundCasNr": "string",
      "exceedingFactor": 0,
      "calculationModelType": 0,
      "calculationFailed": true,
      "calculationFailedErrorMessage": "string"
    }
  ]
}
```

# 14. Notification events sent to Jar
There are 3 events to be sent to Jar as following listed:
- JarScreeningNewEvent : when there is a new screening has been saved, system notifies Jar with this event.
  - Schema:
```json
{
   "id":"3e8ceb09-186c-452a-ac61-acb00039309f",
   "subject":"Jar.Screening.New",
   "data":{
      "NewScreeningAt":"2021-01-14T03:28:14.1558744+00:00",
      "PreviousScreeningAt":"2021-01-13T23:31:38.3342789+00:00",
      "ReasonToSave":1,
      "ScreeningLog":{
         "NewScreeningAt":"2021-01-14T03:28:14.1558744+00:00",
         "PreviousScreeningAt":"2021-01-13T23:31:38.3342789+00:00",
         "ReasonToSave":1,
         "OverrulingScreeningRulesApplied":[

         ],
         "AddedFlags":0,
         "RemovedFlags":0,
         "AddedPollutants":[

         ],
         "RemovedPollutants":[

         ],
         "AddedPollutionCauses":[

         ],
         "RemovedPollutionCauses":[

         ],
         "AddedActivities":[

         ],
         "RemovedActivities":[

         ],
         "AddedMissingActivities":[

         ],
         "RemovedMissingActivities":[

         ],
         "AddedMissingPollutionCauses":[

         ],
         "RemovedMissingPollutionCauses":[

         ],
         "AddedMissingPollutantComponents":[

         ],
         "RemovedMissingPollutantComponents":[

         ],
         "AddedV1PolygonAreaInTotal":0.0,
         "RemovedV1PolygonAreaInTotal":0.0,
         "AddedV2PolygonAreaInTotal":0.0,
         "RemovedV2PolygonAreaInTotal":0.0,
         "AddedExceedFactor":0.0,
         "RemovedExceedFactor":0.0,
         "ScreeningTriggeredBy":1
      },
      "ReassessedToStatus":0,
      "ReassementDate":"0001-01-01T00:00:00+00:00",
      "CreatedAt":"2021-01-14T03:28:14.1558744+00:00",
      "LocationId":"81fafe52-4742-4316-b2ca-dc3bbd3445fc",
      "LocationNumber":"230-05005",
      "Region":1084,
      "Id":"3e8ceb09-186c-452a-ac61-acb00039309f"
   },
   "eventType":"Jar.Screening.New",
   "eventTime":"2021-01-14T03:28:14.8873174Z",
   "metadataVersion":"1",
   "dataVersion":"1.0"
}
```
- JarRiskCalculationUnapprovedEvent: when a location start a new risk assessment process, system notifies Jar with this event.
  - Schema:
```json
{
   "id":"4d160bb9-99d4-4219-a0df-acb000ff80fe",
   "topic":null,
   "subject":"Jar.RiskCalculation.Unapproved",
   "data":{
      "Type":"Jar.RiskCalculation.Unapproved",
      "ContaminantCasNr":"120365",
      "ContaminantName":"Dichlorprop",
      "ModelType":1,
      "ExcessFactor":null,
      "CreatedAt":"2021-01-14T15:30:28.6231824+00:00",
      "LocationId":"f74f8f25-832b-42b0-81e3-000c834287fa",
      "LocationNumber":"233-00322",
      "Region":1084,
      "Id":"4d160bb9-99d4-4219-a0df-acb000ff80fe"
   },
   "eventType":"Jar.RiskCalculation.Unapproved",
   "eventTime":"2021-01-14T22:30:38.5944416+07:00",
   "metadataVersion":null,
   "dataVersion":"1.0"
}
```
- JarRiskCalculationApprovedEvent: when a risk assessment details is approved, system notifies Jar with this event.
  - Schema:
```json
{
   "id":"4d160bb9-99d4-4219-a0df-acb000ff80fe",
   "topic":null,
   "subject":"Jar.RiskCalculation.Approved",
   "data":{
      "Type":"Jar.RiskCalculation.Approved",
      "ContaminantCasNr":"120365",
      "ContaminantName":"Dichlorprop",
      "ModelType":1,
      "ExcessFactor":null,
      "CreatedAt":"2021-01-14T15:30:28.6231824+00:00",
      "LocationId":"f74f8f25-832b-42b0-81e3-000c834287fa",
      "LocationNumber":"233-00322",
      "Region":1084,
      "Id":"4d160bb9-99d4-4219-a0df-acb000ff80fe"
   },
   "eventType":"Jar.RiskCalculation.Approved",
   "eventTime":"2021-01-14T22:33:02.5312347+07:00",
   "metadataVersion":null,
   "dataVersion":"1.0"
}
```
All 3 events are sent to an appropriate region in one of 5 regions:
* RegionH
* RegionSj
* RegionSyd
* RegionMidt
* RegionNord