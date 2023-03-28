# Project Manager
The ProjectManager is a tool for managing multiple PocketBase instances.
It is an easy-to-use web interface for creating, editing and deleting PocketBase instances.

## Features
- [x] Automatic Docker configuration
- [x] Automatic DNS Mapping
- [x] Automatic SSL configuration using NginxProxyManager
- [x] Multiple languages
- [x] Own domains
- [ ] Export / import projects

## Installation
The installation is done using a Docker-Compose file. Two containers, one for the backend and one for the frontend, are started here.
The default configuration looks like this:

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
### Environment variables

| Variable          | Bedeutung                                                       |
|-------------------|-----------------------------------------------------------------|
| BACKEND           | The backend url                                                 |
| FRONTEND          | The frontend url                                                |
| GENERAL__DATABASE | The MySql connection string                                     |
| GENERAL__ROOT     | Folder on the host system for storing the PocketBase files      |
| PROXY__ENABLE     | Enables built-in SSL encryption with NginxProxyManager          |
| PROXY__URL        | The URL for the NginxProxyManager web interface                 |
| PROXY__EMAIL      | Email address for an admin user of NginxProxyManager            |
| PROXY__PASSWORD   | Password for an admin user of NginxProxyManager                 |
| PROXY__DOMAIN     | Standard domain for PocketBase instances ([id].api.example.com) |
| PROXY__HOST       | Server host address (for the reverse proxy)                     |

### Edit source code and create Docker images yourself
If you want to edit the source code yourself, it is of course available to you. To use the backend in debug mode you should use a
``appsettings.Development.json`` file in the main directory to create the appropriate configuration for the development environment.
For debugging I recommend ``npm run dev:ssr`` to start the frontend and ``dotnet run`` to start the backend. Unless you're done with the edits
there is a ``docker-compose.example.yml`` file in the repository that automatically rebuilds the source code and starts the containers.
Alternatively, you can build them yourself with ``docker build``.
