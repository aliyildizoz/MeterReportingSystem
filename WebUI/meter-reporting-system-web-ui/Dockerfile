FROM node:alpine

WORKDIR /usr/src/app

COPY ["./WebUI/meter-reporting-system-web-ui/", "."]


RUN npm install -g @angular/cli

RUN npm install

CMD ["ng", "serve", "--host", "0.0.0.0"]
