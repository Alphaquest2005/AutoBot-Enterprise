UPDATE FileTypeMappings
SET         OriginalName = REPLACE(REPLACE(OriginalName, CHAR(13), ''), CHAR(10), ''), DestinationName = REPLACE(REPLACE(DestinationName, CHAR(13), ''), CHAR(10), '')