
Grundrisk API integration guide to Preliminary screening endpoint.

# 1. The code
The code consist of a SPA using Angular which shows how to fetch data and a .net core client.

The .net core client is located in the folder - Dmp.Examples.GrundriskIntegration  which does a full codeflow login and then calls the grundrisk preliminary screening. Remember to look into "1. Security and access to the endpoint" as you would need the client id and secret.

# 2. Security and access to the endpoint

In order to communicate with the endpoint `/screenings/preliminary` the code has to use the OAUTH codeflow against the endpoint. That means you have to contact DMP for registration on the DMP useradm for both test and production.

You will need the role miljoe_grundrisk_foreloebigscreening in order to acces the endpoint. 

Please contact Danmarks Mlijøportal's support at support@miljoeportal.dk to get a client id and client secret for authorization.

## Endpoint urls

| Environment | Url |Swagger | Swagger definintion|
| ----------- | ---------------- |----|---|
| TEST        |  https://grundrisk-api.test.miljoeportal.dk/preliminaryScreenings | https://grundrisk-api.test.miljoeportal.dk/swagger/ |https://grundrisk-api.test.miljoeportal.dk/swagger/v1/swagger.json |
| DEMO        | https://grundrisk-api.demo.miljoeportal.dk/preliminaryScreenings | https://grundrisk-api.demo.miljoeportal.dk/swagger/ |https://grundrisk-api.demo.miljoeportal.dk/swagger/v1/swagger.json |
| PROD        | https://grundrisk-api.miljoeportal.dk/preliminaryScreenings | https://grundrisk-api.miljoeportal.dk/swagger/ |https://grundrisk-api.miljoeportal.dk/swagger/v1/swagger.json |



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

# 5 Removal reasons
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



# 6  Standard parameters in the response

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


# 7 Test input  for preliminary screening 

It produces produces 2 flags  - value 8 and 9 and standard parameters

* Request
```json
{
    "pollutantComponentCodes": [
      "0703      ",
      "0490      "
    ],
    "activities": [
      {
        "activityCode": "999       ",
        "pollutionCauseCode": "50.20.10  "
      },
      {
        "activityCode": "006       ",
        "pollutionCauseCode": "50.50.00  "
      },
      {
        "activityCode": "999       ",
        "pollutionCauseCode": "25.12.00  "
      }
    ],
    "v1ShapeWkts": [
      "POLYGON ((554931.9389 6145817.3598, 554943.7005 6145814.4546, 554943.7377 6145814.4366, 554957.3742 6145803.8961, 554963.2915 6145805.0073, 554963.3599 6145804.9957, 554982.2349 6145794.1282, 554982.2398 6145794.1252, 554983.5714 6145793.2533, 554995.1822 6145842.5366, 554992.8408 6145844.0876, 554965.4258 6145862.191, 554960.6086 6145861.1682, 554948.7069 6145842.9933, 554957.3674 6145837.738, 554957.4005 6145837.5998, 554948.9235 6145823.9228, 554948.7887 6145823.8888, 554939.6435 6145829.1403, 554931.9389 6145817.3598))",
      "POLYGON ((554944.9397 6145837.2277, 554939.7532 6145829.3079, 554948.8044 6145824.1104, 554957.1773 6145837.6194, 554948.5973 6145842.8258, 554944.9397 6145837.2278, 554944.9397 6145837.2277))"
    ],
    "v2ShapeWkts": [
      "POLYGON ((554931.9389 6145817.3598, 554943.7005 6145814.4546, 554943.7377 6145814.4366, 554957.3742 6145803.8961, 554963.2915 6145805.0073, 554963.3599 6145804.9957, 554982.2349 6145794.1282, 554982.2398 6145794.1252, 554983.5714 6145793.2533, 554995.1822 6145842.5366, 554992.8408 6145844.0876, 554965.4258 6145862.191, 554960.6086 6145861.1682, 554948.7069 6145842.9933, 554957.3674 6145837.738, 554957.4005 6145837.5998, 554948.9235 6145823.9228, 554948.7887 6145823.8888, 554939.6435 6145829.1403, 554931.9389 6145817.3598))",
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
  "id": "58b68cfa-b8b5-462c-84cd-abb100ac7077",
  "createdAt": "2020-05-04T10:28:00.9028352+00:00",
  "hasV1Polygon": true,
  "hasV2Polygon": true,
  "v1PolygonResult": {
    "type": 0,
    "hasData": true,
    "factor": 920.4096994781613,
    "polygonStatus": 1,
    "reassessment": null,
    "pollutant": "MTBE"
  },
  "v2PolygonResult": {
    "type": 1,
    "hasData": true,
    "factor": 4850.969942641344,
    "polygonStatus": 1,
    "reassessment": null,
    "pollutant": "MTBE"
  },
  "noModelCompounds": false,
  "noScreeningInputs": false,
  "screeningResults": [
    {
      "preliminaryScreeningId": "58b68cfa-b8b5-462c-84cd-abb100ac7077",
      "createdAt": "2020-05-04T10:28:14.3428829+00:00",
      "status": 2,
      "industry": null,
      "activity": null,
      "compoundName": "Benzin",
      "compoundCasNr": null,
      "qualityCriterion": 0,
      "worstCaseConcentration": 8000,
      "coverThickness": 17.457580133628177,
      "dataQuality": 0,
      "concentrationDownstream": 212.76435208837154,
      "removed": false,
      "removalReason": 0,
      "logInfo": [
        2,
        6
      ],
      "concTopTables": 8000,
      "conc100mGrundRisk": 212.76435208837154,
      "factor": 212.76435208837154,
      "flag": 8,
      "polygonType": 1,
      "polygonArea": 163.87679669065588,
      "standardParameters": {
        "infiltration": 100,
        "aquiferDepth": 14.34807491,
        "headGradient": 0.007,
        "lithoCode": 1,
        "distNearestWaterWell": 675.1535179323225,
        "distNoClay": 669.5346970240031,
        "porosity": 0.3,
        "horizontalHydraulicConductivity": 0.0001,
        "firstOrderDegradationRate": 0
      },
      "id": "2fed46d8-e9ac-4ae1-97bc-abb100ac8d0e"
    },
    {
      "preliminaryScreeningId": "58b68cfa-b8b5-462c-84cd-abb100ac7077",
      "createdAt": "2020-05-04T10:28:14.342889+00:00",
      "status": 2,
      "industry": null,
      "activity": null,
      "compoundName": "MTBE",
      "compoundCasNr": null,
      "qualityCriterion": 0,
      "worstCaseConcentration": 50000,
      "coverThickness": 17.46450808448605,
      "dataQuality": 0,
      "concentrationDownstream": 4850.969942641344,
      "removed": false,
      "removalReason": 0,
      "logInfo": [
        2,
        6
      ],
      "concTopTables": 50000,
      "conc100mGrundRisk": 4850.969942641344,
      "factor": 4850.969942641344,
      "flag": 8,
      "polygonType": 1,
      "polygonArea": 2279.576082792796,
      "standardParameters": {
        "infiltration": 100,
        "aquiferDepth": 14.34807491,
        "headGradient": 0.007,
        "lithoCode": 1,
        "distNearestWaterWell": 694.6089725890401,
        "distNoClay": 668.5543884389465,
        "porosity": 0.3,
        "horizontalHydraulicConductivity": 0.0001,
        "firstOrderDegradationRate": 0
      },
      "id": "ad2b8503-7b97-4d15-81c7-abb100ac8d0e"
    },
 ```
