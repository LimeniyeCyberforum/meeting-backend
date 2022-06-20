This repository its backend of https://github.com/LimeniyeCyberforum/Meeting 

# How to run

### Docker
```
docker run --rm -d -it --name meeting-grpc \
    -p 50011:5011 -p 5010:5010 \
    -e ASPNETCORE_URLS="https://+;http://+" \
    -e ASPNETCORE_HTTPS_PORT=5010 \
    -e ASPNETCORE_Kestrel__Certificates__Default__Password="limeniye" \
    -e ASPNETCORE_Kestrel__Certificates__Default__Path=//meeting-grpc.pfx \
    -v /$PWD/meeting-grpc.pfx://meeting-grpc.pfx \
    limeniye/meeting-grpc
```

```
docker attach --sig-proxy=false meeting-grpc
```
