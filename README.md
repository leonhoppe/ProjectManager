# Project Manager
Der ProjectManager ist ein Tool zum Verwalten von mehreren PocketBase Instanzen.
Es handelt sich hierbei um ein einfach zu benutzendes WebInterface zum Erstellen, Bearbeiten und Löschen von PocketBase Instanzen.

## Features
- [x] Automatische Docker Konfiguration
- [x] Automatisches DNS Mapping
- [x] Automatische SSL-Konfiguration mithilfe von NginxProxyManager
- [x] Mehrere Sprachen
- [x] Eigene Domains
- [ ] Projekte exportieren / importieren

## Installation
Die Installation erfolgt durch eine Docker-Compose Datei. Hierbei werden zwei Container, einer für das Backend und einer für das Frontend, gestartet.
Die Standartkonfiguration sieht wie folgt aus:

```yml
version: '3.0'
services:
  frontend:
    image: git.leon-hoppe.de/leon.hoppe/projectmanager_frontend:latest
    restart: unless-stopped
    environment:
      - BACKEND=https://api.example.com
    ports:
      - '4220:4000'

  backend:
    image: git.leon-hoppe.de/leon.hoppe/projectmanager_backend:latest
    restart: unless-stopped
    environment:
      - FRONTEND=https://example.com
      - GENERAL__DATABASE=server=1.2.3.4;database=ProjectManager;user=ProjectManager;password=changeMe
      - GENERAL__ROOT=/projects/
      - PROXY__ENABLE=true
      - PROXY__URL=https://proxy.example.com
      - PROXY__EMAIL=admin@example.com
      - PROXY__PASSWORD=changeMe
      - PROXY__DOMAIN=api.example.com
      - PROXY__HOST=1.2.3.4
    ports:
      - '4221:80'
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock
```
### Environment Variablen

| Variable          | Bedeutung                                                            |
|-------------------|----------------------------------------------------------------------|
| BACKEND           | Die URL des Backends                                                 |
| FRONTEND          | Die URL des Frontends                                                |
| GENERAL__DATABASE | Der MySql Connection String                                          |
| GENERAL__ROOT     | Ordner auf dem Hostsystem zur Speicherung der PocketBase Dateien     |
| PROXY__ENABLE     | Aktiviert die eingebaute SSL Verschlüsselung durch NginxProxyManager |
| PROXY__URL        | Die URL für das WebInterface des NginxProxyManager                   |
| PROXY__EMAIL      | E-Mail-Adresse für einen Admin Benutzer von NginxProxyManager        |
| PROXY__PASSWORD   | Passwort für einen Admin Benutzer von NginxProxyManager              |
| PROXY__DOMAIN     | Standart Domain für PocketBase instanzen ([id].api.example.com)      |
| PROXY__HOST       | Hostadresse des Servers (für die reverse proxy)                      |

### SourceCode bearbeiten und Docker Images selber erstellen
Falls Sie den SourceCode selbst bearbeiten wollen, steht Ihnen dieser natürlich zur Verfügung. Um das Backend im Debug Modus zu verwenden sollten sie eine 
``appsettings.Development.json`` Datei im Hauptverzeichnis anlegen, um dort die entsprechende Konfiguration für die Entwicklungsumgebung erstellen.
Zum Debuggen empfehle ich ``npm run dev:ssr`` um das Frontend zu starten und ``dotnet run`` um das Backend zu starten. Sofern Sie mit den Bearbeitungen fertig
sind, gibt es eine ``docker-compose.example.yml`` Datei im dem Repository die automatisch den SoruceCode neu baut und die Container startet.
Wahlweise können Sie diese mit ``docker build`` auch selbst bauen.
