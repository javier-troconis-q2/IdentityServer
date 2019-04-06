FROM gcr.io/google-appengine/aspnetcore:2.2
ADD ./bundle_output /app
ENV ASPNETCORE_URLS=http://*:${PORT}
WORKDIR /app
ENTRYPOINT [ "dotnet", "IdentityServer.GCPContainerized.dll"]