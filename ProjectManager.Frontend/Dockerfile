#stage 1
FROM node:18-slim as node
WORKDIR /app
COPY . .
RUN npm install
RUN npm run build:ssr --omit=dev
#stage 2
FROM node:18-slim
COPY --from=node /app/dist /app/dist
WORKDIR /app
CMD ["node", "dist/ProjectManager.Frontend/server/main.js"]
