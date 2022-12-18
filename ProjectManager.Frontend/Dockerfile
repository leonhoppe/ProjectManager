#stage 1
FROM node:18 as node
WORKDIR /app
COPY . .
RUN npm install
RUN npm run build:ssr --prod
#stage 2
FROM node:18
COPY --from=node /app/dist /app/dist
WORKDIR /app
CMD ["node", "dist/ProjectManager.Frontend/server/main.js"]
