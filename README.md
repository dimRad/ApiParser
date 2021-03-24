# ApiParser
A tool for getting data


# Usage

`ApiParser file.csv`

# General Settings

UrlFormat - The url to get data from the data in the 'UrlColumn' will replace the `{0}` token

UrlColumn - Zero based column identifier

ShowNoDataWarning - If true shows a warning if the data in 'UrlColumn' is null

HasHeaderRecord - If set to true skips the first row


# Authentication

When both parameters are set the values will be populated in the `Authorization` header

AuthenticationScheme - The authorization scheme for protected resources.

AuthenticationValue - The authentication parameter

See secret.json
