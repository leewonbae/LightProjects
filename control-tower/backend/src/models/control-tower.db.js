const config = global.config;

const Sequelize = require("sequelize");
const controlTowerDBSequelize = new Sequelize(config.CONTROL_TOWER_DB);

async () => {
  await controlTowerDBSequelize
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
  sequelize: controlTowerDBSequelize,
};
