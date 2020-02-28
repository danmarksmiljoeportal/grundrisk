
#Grundrisk API integration guide to Preliminary screening endpoint.

Dato: 28-02-2020

Lavet af: Peter Prang Due, ppd@globeteam.com

Globeteam A/S
Virumgårdsvej 17A
2830 Virum

# Indholdsfortegnelse
[[_TOC_]]



# 1. Security and access to the endpoint

In order to communicate with the endpoint `/screenings/preliminary` the code has to use the OAUTH codeflow against the endpoint. That means you have to contact DMP for registration on the DMP useradm for both test and production.

You will need the role miljoe_grundrisk_foreloebigscreening in order to acces the endpoint.

# 2. The screening flags in the response from the API endpoint `/screenings/preliminary`

When you receive a response from the preliminary screenings for each compound the following applies:

1. The value can be "flag":9 , which is 1001 in the binary representation.

2. The enums for calculating which flags that we can derive of the flag value, we use the bitwise left operation. The flag enums are like this:

```
    None = 0 << 0,
    Bly_kobber_eller_PAH = 1 << 0,
    Kompleks_geologi = 1 << 1,
    Manglende_modelstof = 1 << 2,
    Svag_geologi = 1 << 3,
    MTBE_fjernet_i_Trin_2 = 1 << 4,
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
  -Svag datadækning for bestemmelse af dæklagstykkelser 
- MTBE_fjernet_i_Trin_2
  - MTBE fjernet i Trin 2 pga. beskyttende dæklag
- Boring_uden_ler
  - Lokalitet har boring uden ler i en radius af 100-300 m fra kant
- Risiko_pga_vandindvinding
 - Lokalitet udgør en risiko fordi der ligger en vandindvindingsboring indenfor 100 m



#2. Standard parameters in the response

When you receive a response from the preliminary screenings for each compound there is also an section called standardParameters.

The values have this description:
```
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
```

#3. Test input  for preliminary screening 

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
    "v2ShapeWkts": null
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
