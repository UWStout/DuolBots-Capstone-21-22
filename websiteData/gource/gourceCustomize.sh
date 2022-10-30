# Remove actions after a certain date (currently December SGX)
cat gourceLog.txt | awk -F\| '$1<=1639785600' > gourceLog.temp
sed -i.bak '/Packages/d' ./gourceLog.temp
sed -i.bak '/ProjectSettings/d' ./gourceLog.temp
sed -i.bak '/Plugins/d' ./gourceLog.temp
sed -i.bak '/Polybrush/d' ./gourceLog.temp
sed -i.bak '/TextMesh/d' ./gourceLog.temp
sed -i.bak '/Samples/d' ./gourceLog.temp
sed -i.bak '/\.meta/d' ./gourceLog.temp
sed -i.bak '/\.yamato/d' ./gourceLog.temp
mv gourceLog.temp gourceLog.txt
rm gourceLog.temp.bak

# Setup Project Name
projName="Duol Bots - Unity 3d Project"

function fix {
  sed -i -- "s/$1/$2/g" gourceLog.txt
}

# Replace non human readable names with proper ones
fix "|berriers|" "|Prof. B|"
fix "|woulfc8602|" "|Cole Woulf|"
fix "|vians3638|" "|Shelby Vian|"
fix "|vange7865|" "|Eslis Vang|"
fix "|senalikw0106|" "|Wyatt Senalik|"
fix "|lussmanb1376|" "|Ben Lussman|"
fix "|grossz9678|" "|Zach Gross|"
fix "|grabowskys8403|" "|Skyler Grabowsky|"
fix "|duffeya7898|" "|Aaron Duffey|"
fix "|plathm5053|" "|Morgen Plath|"
fix "|lacerdak2845|" "|Kasey Lacerda|"
fix "|nelsonc3552|" "|Carter Nelson|"
fix "|hilla7374|" "|Abigail Hill|"
