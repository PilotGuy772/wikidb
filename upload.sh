#!/bin/bash

# upload the .deb package, pack the compiled binaries into a .tar.gz, and upload everything to the release
DEB_PATH="./build/*.deb"
DEB_FILES=($DEB_PATH)

for DEB in "${DEB_FILES[@]}"; do
  UPLOAD_URL=$(curl -s -H "Authorization: token $GITHUB_TOKEN" \
  "https://api.github.com/repos/PilotGuy772/wikidb/releases/$RELEASE_ID/assets?name=$(basename "$DEB")" | \
  jq -r '.upload_url' | sed -e 's/{?name,label}//')
  
  curl -H "Authorization: token $GITHUB_TOKEN" \
  -H "Content-type: $(file -b --mimie-type $DEB)" \
  --data-binary @"$DEB" "$UPLOAD_URL"
done