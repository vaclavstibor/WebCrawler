#stage 1
FROM node:latest as node
WORKDIR /app
COPY . .
RUN npm install
RUN npm run build --dev --watch --poll
#stage 2
FROM nginx:alpine
COPY --from=node /app/dist/web-crawler /usr/share/nginx/html
