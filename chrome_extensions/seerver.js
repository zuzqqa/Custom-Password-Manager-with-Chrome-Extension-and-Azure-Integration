// node.js server to handle requests from chrome extension (POST method, just for testing)
const express = require("express");
const bodyParser = require("body-parser");

const app = express();
const port = 3000;

app.use((req, res, next) => {
  res.setHeader(
    "Access-Control-Allow-Origin",
    "chrome-extension://odmidfpignoialggnofjgeimfepkkmja"
  ); // Zastąp wartość 'chrome-extension://odmidfpignoialggnofjgeimfepkkmja' adresem URL Twojego rozszerzenia
  res.setHeader(
    "Access-Control-Allow-Methods",
    "GET, POST, OPTIONS, PUT, PATCH, DELETE"
  );
  res.setHeader(
    "Access-Control-Allow-Headers",
    "X-Requested-With,content-type"
  );
  res.setHeader("Access-Control-Allow-Credentials", true);
  next();
});

app.use(bodyParser.json());

app.post("/", (req, res) => {
  console.log("Odebrano dane:", req.body);
  res.send("Dane odebrane pomyślnie!");
});

app.listen(port, () => {
  console.log(`Serwer nasłuchuje na porcie ${port}`);
});
