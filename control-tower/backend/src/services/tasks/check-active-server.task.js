const { SimpleIntervalJob, Task } = require('toad-scheduler');
const axios = require('axios');
const activeServerService = require("../active-server.service");

const checkActiveServerTask = new Task('CheckActiveServerTask', async () => {
    try {
        const activeServerList = await activeServerService.getActiveServerList()
        if (activeServerList.length == 0) {
            return;
        }

        for (const activeServer of activeServerList) {
            if (activeServer.status === 'RESTARTING') {
                continue;
            }
            
            const requestUrl = `http://${activeServer.ip}:${activeServer.port}/system/get-server-date-time`;

            try {
                await axios.get(requestUrl, { timeout: 1000 });
            } catch (error) {
                console.error(`${error.message} Name [${activeServer.name}] Ip [${activeServer.ip}] Port [${activeServer.port}] [${new Date().toLocaleString()}]`);
                const params = {
                    id: activeServer.id,
                };

                if (error.message === 'timeout of 1000ms exceeded' ||
                    error.message.indexOf('connect ECONNREFUSED') >= 0) {
                    await activeServerService.deregister(params);
                }
            }
        }
    } catch (e) {
        console.log(e);
    }
});

console.log('[Task] Check active server');
module.exports = new SimpleIntervalJob({ seconds: 60 }, checkActiveServerTask);