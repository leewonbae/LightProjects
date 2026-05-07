const controlTowerDB = require("../models/control-tower.db");

module.exports = {
    getDockerList: async () => {
        return await controlTowerDB.sequelize.query(
            `SELECT * 
               FROM server_docker_images`,
            {
                type: controlTowerDB.sequelize.QueryTypes.SELECT,
            }
        );
    },

    addDocker: async (params) => {
        try {
            await controlTowerDB.sequelize.query(
                `INSERT INTO server_docker_images (name, image_id) 
                      VALUES (:name, :image_id)
            ON DUPLICATE KEY UPDATE name=:name, image_id=:image_id`,
                {
                    replacements: params,
                    type: controlTowerDB.sequelize.QueryTypes.INSERT,
                }
            );
        } catch (e) {
            console.error(e)
        }
    },

    getDockerByName: async (params) => {
        return await controlTowerDB.sequelize.query(
            `SELECT * 
               FROM server_docker_images
              WHERE name=:name`,
            {
                replacements: params,
                type: controlTowerDB.sequelize.QueryTypes.SELECT,
            }
        );
    },

    deleteDocker: async (params) => {
        await controlTowerDB.sequelize.query(
            `DELETE FROM server_docker_images WHERE name=:name`,
            {
                replacements: params,
                type: controlTowerDB.sequelize.QueryTypes.DELETE,
            }
        );
    },
};
