# Linz Linien Alexa Skill
Alexa Skill to inquiry public transit departures of the city Linz, Austria. Only the German language is supported.

## Utterances
```
Alexa, frage Linz Linien ...  / Alexa, öffne Linz Linien ... 

// NextTramDepartureFromStop
wann fährt die Straßenbahn von {originStopName} nach {finalDestinationStopName}
wann fährt die nächste Straßenbahn von {originStopName} nach {finalDestinationStopName}
nach der Straßenbahn von {originStopName} nach {finalDestinationStopName}
nach der nächsten Straßenbahn von {originStopName} nach {finalDestinationStopName}

// NextBusDepartureFromStop
wann fährt der Bus von {originStopName} nach {finalDestinationStopName}
wann fährt der nächste Bus von {originStopName} nach {finalDestinationStopName}
nach dem Bus von {originStopName} nach {finalDestinationStopName}
nach dem nächsten Bus von {originStopName} nach {finalDestinationStopName}

// NextLineDepartureFromStop
wann fährt die nächste Linie {lineNr} von {originStopName} nach {finalDestinationStopName}
wann fährt die Linie {lineNr} von {originStopName} nach {finalDestinationStopName}

// NextDeparturesFromStop
nach den Abfahrten von {originStopName}
was sind die Abfahrten von {originStopName}
nach den nächsten Abfahrten von {originStopName}
was sind die nächsten Abfahrten von {originStopName}
```


## License
This project is licensed under the MIT license. See the [LICENSE](LICENSE) file for details.

## Acknowledgments
The public transport [data](https://www.data.gv.at/katalog/dataset/9faa1734-607f-4bfd-b8c9-c5692bf37d55) by the city Linz is licensed under [CC BY 3.0 AT](https://creativecommons.org/licenses/by/3.0/at/deed.en).  
The [AlexaSkillsKit.NET](https://github.com/AreYouFreeBusy/AlexaSkillsKit.NET) is licensed under [MIT](https://github.com/AreYouFreeBusy/AlexaSkillsKit.NET/blob/master/LICENSE).