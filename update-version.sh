#!/bin/bash

echo "Updating the version to $1 for files in $PWD"

newVersion=$1
for pluginXmlFile in $( find ../. -name "plugin.xml" ); do
  echo "Editing file: $pluginXmlFile $newVersion"
	cmd="ed -s $pluginXmlFile <<< $'H\n,s/<version>_PLACEHOLDER_<\/version>/<version>$newVersion<\/version>/g\nw'"
	echo $cmd
	eval $cmd
done

for pluginXmlFile in $( find ../. -name "gradle.properties" ); do
  echo "Editing file: $pluginXmlFile $newVersion"
	cmd="ed -s $pluginXmlFile <<< $'H\n,s/PluginVersion=_PLACEHOLDER_/PluginVersion=$newVersion/g\nw'"
	echo $cmd
	eval $cmd
done

for pluginXmlFile in $( find ../. -name "*.csproj" ); do
  echo "Editing file: $pluginXmlFile $newVersion"
	cmd="ed -s $pluginXmlFile <<< $'H\n,s/<PackageVersion>1.0.1</<PackageVersion>$newVersion</g\nw'"
	echo $cmd
	eval $cmd
done


for pluginXmlFile in $( find ../. -name "*.csproj" ); do
  echo "Editing file: $pluginXmlFile $newVersion"
	cmd="ed -s $pluginXmlFile <<< $'H\n,s/<AssemblyVersion>1.0.1</<AssemblyVersion>$newVersion</g\nw'"
	echo $cmd
	eval $cmd
done


for pluginXmlFile in $( find ../. -name "*.csproj" ); do
  echo "Editing file: $pluginXmlFile $newVersion"
	cmd="ed -s $pluginXmlFile <<< $'H\n,s/<FileVersion>1.0.1</<FileVersion>$newVersion</g\nw'"
	echo $cmd
	eval $cmd
done

exit 0

