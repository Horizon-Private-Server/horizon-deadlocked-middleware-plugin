Remove-Item -Recurse -Force ./out
Remove-Item -Recurse -Force ./middleware
Remove-Item -Recurse -Force ./out/plugin
mkdir ./out/plugin
mkdir ./middleware

Copy-Item ../horizon-server-database-middleware -Destination ./middleware -Force -Recurse

docker build . -t deadlocked_middleware_plugin

docker run -v ${PWD}/out/:/mnt/out -it deadlocked_middleware_plugin

sudo chmod a+rw out -R

