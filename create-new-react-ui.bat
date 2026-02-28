@echo off

rmdir /s /q MyFirstAbpReactApp
echo Old MyFirstAbpReactApp deleted

echo Creating new React UI
dotnet run --project ui-scaffolding-generator-cli -- init MyFirstAbpReactApp --backend-path backend/MultiLayerAbp

pause