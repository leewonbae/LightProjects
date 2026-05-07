const controlTowerDB = require("../models/control-tower.db");
const systemDB = require("../models/system.db");

module.exports = {
    getServerList: async () => {
        return await controlTowerDB.sequelize.query(
            `SELECT * 
               FROM active_servers`,
            {
                type: controlTowerDB.sequelize.QueryTypes.SELECT,
            }
        );
    },

    getDbList: async () => {
        return await controlTowerDB.sequelize.query(
            `SELECT * 
               FROM control_tower_db`,
            {
                type: controlTowerDB.sequelize.QueryTypes.SELECT,
            }
        );
    },

    getActiveServerList: async () => {
        return await controlTowerDB.sequelize.query(
            `SELECT * 
               FROM active_servers
              WHERE status = 'RUNNING'`,
            {
                type: controlTowerDB.sequelize.QueryTypes.SELECT,
            }
        );
    },

    getActiveServerByName: async (params) => {
        return await controlTowerDB.sequelize.query(
            `SELECT * 
               FROM active_servers
              WHERE name=:name`,
            {
                replacements: params,
                type: controlTowerDB.sequelize.QueryTypes.SELECT,
            }
        );
    },

    getDbByName: async (params) => {
        return await controlTowerDB.sequelize.query(
            `SELECT * 
               FROM control_tower_db
              WHERE name=:name`,
            {
                replacements: params,
                type: controlTowerDB.sequelize.QueryTypes.SELECT,
            }
        );
    },

    addActiveServer: async (params) => {
        await controlTowerDB.sequelize.query(
            `INSERT INTO active_servers(name, branch, \`database\`, status, is_possible_change_server_dt) 
                  VALUES (:name, :branch, :database, :status, :isPossibleChangeServerDt)`,
            {
                replacements: params,
                type: controlTowerDB.sequelize.QueryTypes.INSERT,
            }
        );
    },

    register: async (params) => {
        await controlTowerDB.sequelize.query(
            `INSERT IGNORE INTO active_servers(name, ip, port,project_name, environment_value, status) 
                         VALUES (:name, :ip, :port, :project_name, :environment_value, :status)`,
            {
                replacements: params,
                type: controlTowerDB.sequelize.QueryTypes.INSERT,
            }
        );
    },

    addDb: async (params) => {
        await controlTowerDB.sequelize.query(
            `INSERT IGNORE INTO control_tower_db(name, branch) 
                         VALUES (:name, :branch)`,
            {
                replacements: params,
                type: controlTowerDB.sequelize.QueryTypes.INSERT,
            }
        );
    },

    deleteDb: async (dbName) => {
        await systemDB.sequelize.query(
            `DROP DATABASE IF EXISTS ${dbName}_account, ${dbName}_game, ${dbName}_data_table`,
            {
                type: systemDB.sequelize.QueryTypes.DELETE,
            }
        );

        await controlTowerDB.sequelize.query(
            `DELETE FROM control_tower_db WHERE name=:name`,
            {
                replacements: { name: dbName },
                type: controlTowerDB.sequelize.QueryTypes.DELETE,
            }
        );
    },

    deleteDataTable: async (dbName) => {
        await systemDB.sequelize.query(
            `DROP DATABASE IF EXISTS [${dbName}_data_table]`,
            {
                type: systemDB.sequelize.QueryTypes.DELETE,
            }
        );
    },

    deregister: async (params) => {
        await controlTowerDB.sequelize.query(
            `DELETE FROM active_servers WHERE id=:id`,
            {
                replacements: params,
                type: controlTowerDB.sequelize.QueryTypes.DELETE,
            }
        );
    },

    deleteServer: async (params) => {
        await controlTowerDB.sequelize.query(
            `DELETE FROM active_servers WHERE name=:name`,
            {
                replacements: params,
                type: controlTowerDB.sequelize.QueryTypes.DELETE,
            }
        );
    },

    updateStatus: async (params) => {
        await controlTowerDB.sequelize.query(
            `UPDATE active_servers
                SET status=:status
              WHERE id=:id`,
            {
                replacements: params,
                type: controlTowerDB.sequelize.QueryTypes.UPDATE,
            }
        );
    },

    updateDockerStatus: async (params) => {
        await controlTowerDB.sequelize.query(
            `UPDATE active_servers
                SET port=:port, revision=:revision, status=:status
              WHERE name=:name`,
            {
                replacements: params,
                type: controlTowerDB.sequelize.QueryTypes.UPDATE,
            }
        );
    },

    updateDockerImage: async (params) => {
        await controlTowerDB.sequelize.query(
            `UPDATE active_servers
                SET image_id=:image_id, status=:status
              WHERE name=:name`,
            {
                replacements: params,
                type: controlTowerDB.sequelize.QueryTypes.UPDATE,
            }
        );
    },

    updateContainerId: async (params) => {
        await controlTowerDB.sequelize.query(
            `UPDATE active_servers
                SET container_id=:container_id,
                    status = 'RUNNING'
              WHERE name=:name`,
            {
                replacements: params,
                type: controlTowerDB.sequelize.QueryTypes.UPDATE,
            }
        );
    },
};