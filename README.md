# Linz Linien Alexa Skill
Alexa Skill to inquiry public transit departures of the city Linz, Austria. 

## Description
The ASP.NET Core project `src/LinzLinienAlexaSkill` provides a HTTP endpoint for the Alexa Skill under the path `/alexa`. This Skill only supports the German language. 

The slot type for the names of the stops was created using the utility `util/LinzLinienAlexaSkill.ModelUtil`. This utility just creates the slot type. The intents and utterances have to be modeled in the [Alexa Skills Console](https://developer.amazon.com/alexa/console/ask).

## Usage
### Invocation Name: Linz Linien
```
Alexa, frage Linz Linien ...
Alexa, öffne Linz Linien ... 
```

### Intent: NextTramDepartureFromStop
```
wann fährt die Straßenbahn von {originStopName} nach {finalDestinationStopName}
wann fährt die nächste Straßenbahn von {originStopName} nach {finalDestinationStopName}
nach der Straßenbahn von {originStopName} nach {finalDestinationStopName}
nach der nächsten Straßenbahn von {originStopName} nach {finalDestinationStopName}
```

### Intent: NextBusDepartureFromStop
```
wann fährt der Bus von {originStopName} nach {finalDestinationStopName}
wann fährt der nächste Bus von {originStopName} nach {finalDestinationStopName}
nach dem Bus von {originStopName} nach {finalDestinationStopName}
nach dem nächsten Bus von {originStopName} nach {finalDestinationStopName}
```

### Intent: NextLineDepartureFromStop
```
wann fährt die nächste Linie {lineNr} von {originStopName} nach {finalDestinationStopName}
wann fährt die Linie {lineNr} von {originStopName} nach {finalDestinationStopName}
```

### Intent: NextDeparturesFromStop
```
nach den Abfahrten von {originStopName}
was sind die Abfahrten von {originStopName}
nach den nächsten Abfahrten von {originStopName}
was sind die nächsten Abfahrten von {originStopName}
```

## Privacy Policy
See the [privacy-policy.md](privacy-policy.md) file for details.

## License
This project is licensed under the MIT license. See the [LICENSE](LICENSE) file for details.

## Acknowledgments
* The public transport [data](https://www.data.gv.at/katalog/dataset/9faa1734-607f-4bfd-b8c9-c5692bf37d55) by the city Linz is licensed under [CC BY 3.0 AT](https://creativecommons.org/licenses/by/3.0/at/deed.en).  
* The [Alexa Skills SDK for .NET](https://github.com/timheuer/alexa-skills-dotnet) by Tim Heuer is licensed under [MIT](https://github.com/timheuer/alexa-skills-dotnet/blob/master/LICENSE).
* The bus stop icon for this Skill by [Freepik](http://www.freepik.com) from [www.flaticon.com](https://www.flaticon.com/) is licensed under [CC 3.0 BY](http://creativecommons.org/licenses/by/3.0/).

## Build Status
develop | master
------------ | -------------
![develop branch build status](https://gurpreet-juttla.visualstudio.com/_apis/public/build/definitions/6edee6a0-af00-49f3-b7dc-aae3d17595f0/5/badge) | ![master branch build status](https://gurpreet-juttla.visualstudio.com/_apis/public/build/definitions/6edee6a0-af00-49f3-b7dc-aae3d17595f0/4/badge)

Changes to the master branch are automatically built and deployed to Azure.