#!/bin/bash

# constants: DEBIAN/control
CONTROL_FILE=$(cat << EOF
Package: wikidb
Version: (cat ./VERSION)
Architecture: $2
Maintainer: PilotGuy772
Description: A simple utility to download and archive pages from the internet on local databases
EOF
)
PROJECT_DIRECTORY=$(pwd)


# script to build WikiDB applications
# run this script from the root directory with the solution file in it
# usage: ./script <runtime> <architecture> <version>
# this will always package as a .deb for that runtime
dotnet build
dotnet publish -c Release -r "$1"

# this script will always deposit in ~/build/<package-name>
mkdir "./build"
mkdir "./build/wikidb"
# WikiDB
mv "./WikiDB/bin/Release/net7.0/$1/publish/WikiDB" "./build/wikidb/wikidb"
# WikiGet
mv "./WikiGet/bin/Release/net7.0/$1/publish/WikiGet" "./build/wikidb/wikiget"
# WikiServe
mv "./WikiServe/bin/Release/net7.0/$1/publish/WikiServe" "./build/wikidb/wikiserve"

cd "./build/wikidb" || exit

mkdir DEBIAN
touch ./DEBIAN/control
echo "$CONTROL_FILE" > ./DEBIAN/control

mkdir ./usr
mkdir ./usr/bin
mv ./wikidb ./usr/bin/wikidb
mv ./wikiget ./usr/bin/wikiget
mv ./wikiserve ./usr/bin/wikiserve

# default config files are always in <solution-root>/default-config/config.xml
mkdir ./etc
mkdir ./etc/wikidb/
cp "$PROJECT_DIRECTORY/default-config/config.xml" ./etc/wikidb/config.xml
mkdir ./etc/wikidb/wikis
cp -R "$PROJECT_DIRECTORY/default-config/wikis" ./etc/wikidb/wikis

echo "Files copied. Attempting to start packaging...."

cd ..
dpkg-deb --build ./wikidb

echo "Packaging complete."