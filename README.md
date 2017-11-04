# simple c# http reverse proxy

## usage

```
reverseproxy.exe [hostname]
```

by browsing to [dest].[hostname]:4114, [dest] is loaded via the proxy.

at the moment prox doesn't support rewriting urls internally to make sure your browser loads resources via the proxy as well which is (super) important (!)

## to-dos

 - implement rewrite support so that loads are proxied too (important!)
 - implement destination HTTPS support (important!!!)
 - implement gzip compression
 - implement HTTPS support

two of the four to-dos can be accomplished in the interim by using a local proxy through a webserver already on the server (eg. apache)