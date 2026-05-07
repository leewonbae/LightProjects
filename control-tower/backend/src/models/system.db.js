const config = global.config;

const Sequelize = require("sequelize");
const systemDBSequelize = new Sequelize(config.SYSTEM_DB);

async () => { 
  await systemDBSequelize
    .authenticate()
    .then(() => {
      console.log("Connection has been established successfully.");
    })
    .catch((err) => {
      console.log("Unable to connect to the database:", err);
    });
};

module.exports = {
  Sequelize: Sequelize,
  sequelize: systemDBSequelize,
};
