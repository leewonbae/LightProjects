const config = global.config;
const jenkins = config.JENKINS;

const express = require('express');
const axios = require('axios');
const router = express.Router();
const activeServerService = require("../services/active-server.service");
const dockerImageService = require("../services/docker-image.service");
const multer = require('multer');
const upload = multer({ dest: './uploadFiles'});

const toolsControllers = require("../controllers/tools.controllers.js");

// 클라이언트에서 요청하는 Active Server List 
router.get('/', async (req, res, next) => {
    return res.send(await activeServerService.getActiveServerList());
});

router.get('/server-dt/:serverName', async (req, res, next) =>{
    
    const params = {
        name: req.params.serverName,
    }

    const serverInfo = await activeServerService.getActiveServerByName(params);
    const requestUrl = `http://${serverInfo[0].ip}:${serverInfo[0].port}/system/get-server-date-time`;
    let serverDt = Date.now;
 
    await axios.get(requestUrl, { timeout: 1000 })
    .then(res => {
        serverDt = res.data;
    }).catch(error => {
        console.log(error);
    });
 
    return res.send(serverDt);
});

router.get('/all', async (req, res, next) => {
    const serverList = await activeServerService.getServerList();
    const dockerList = await dockerImageService.getDockerList();
    const dbList = await activeServerService.getDbList();

    return res.send({
        serverList: serverList,
        dbList: dbList,
        dockerList: dockerList
    });
});
router.post('/reset-server-dt', async (req, res, next) => {
    const params = {
        name: req.body.serverName,
    };

    let resultServerDt = Date.now;
    const serverInfo = await activeServerService.getActiveServerByName(params);
    if( !serverInfo || !serverInfo[0])
    {
        return res.send({result:resultServerDt});
    }

    const requestUrl = `http://${serverInfo[0].ip}:${serverInfo[0].port}/system/reset-server-date-time`;
    
    await axios.get(requestUrl, { timeout: 1000 })
    .then(res => {
        resultServerDt = res.data;
    }).catch(error => {
        console.log(error);
    });

    return res.send({result:resultServerDt});
});
router.post('/set-server-dt', async (req, res, next) => {
    const params = {
        name: req.body.serverName,
        serverDt: req.body.serverDt,
    };

    const serverInfo = await activeServerService.getActiveServerByName(params);
    const requestUrl = `http://${serverInfo[0].ip}:${serverInfo[0].port}/system/set-server-date-time?newDateTime=${params.serverDt}`;
    
    let resultServerDt = Date.now;
    await axios.post(requestUrl, { timeout: 1000 })
    .then(res => {
        resultServerDt = res.data;
    }).catch(error => {
        console.log(error);
    });

    return res.send({result:resultServerDt});
});
router.post('/add', async (req, res, next) => {
    const params = {
        name: req.body.name,
        branch: req.body.branch,
        database: req.body.database,
        status: 'INITIALIZING',
        isPossibleChangeServerDt: req.body.isPossibleChangeServerDt
    };
    await activeServerService.addActiveServer(params);

    const jsonBody = JSON.stringify(params);
    const command = "curl";
    const argument = [`-u${jenkins.id}:${jenkins.password}`, `-vs ${jenkins.docker_build.webhook}`,
        '-X POST', '-H "Content-Type: application/json"', `-d '${jsonBody}'`
    ];

    const { spawn } = require('child_process');
    const child = spawn(command, argument, { shell: true });

    child.stdout.on('data', (data) => {
        console.log(`stdout ${data}`);
    });

    child.stderr.on('data', (data) => {
        console.error(`stderr ${data}`);
    });

    const result = await activeServerService.getActiveServerByName(params);
    return res.send(result[0]);
});

router.post('/register', async (req, res, next) => {
    const params = {
        name: req.body.name,
        ip: req.body.ip,
        port: req.body.port,
        project_name: req.body.project_name,
        environment_value: req.body.environment_value,
        status: 'RUNNING'
    };

    //only our private network
    if (params.ip.indexOf("192.168.0") >= 0) {
        await activeServerService.register(params);
    }
    else
    {
        console.log(`not private network ip -- ${params.ip}`);
    }

    const result = await activeServerService.getActiveServerByName(params);
    return res.send(result[0]);
});

router.post('/deregister', async (req, res, next) => {
    const params = {
        id: req.body.id,
    };

    await activeServerService.deregister(params);
    return res.send(params);
});

router.post('/delete-server', async (req, res, next) => {
    const params = {
        name: req.body.name,
    };

    const result = await activeServerService.getActiveServerByName(params);
    if (result[0]) {
        await activeServerService.deleteServer(params);

        const command = "sudo";
        const argument = ['docker', 'kill', result[0].container_id];

        const { spawn } = require('child_process');
        const child = spawn(command, argument, { shell: true });

        child.stdout.on('data', (data) => {
            console.log(`stdout ${data}`);
        });
    }

    return res.send(params);
});

router.post('/update_docker_status', async (req, res, next) => {
    const params = {
        name: req.body.name,
        revision: req.body.revision,
        port: req.body.port,
        status: 'BUILDING_DOCKER'
    };

    await activeServerService.updateDockerStatus(params);
    return res.send(params);
});

router.post('/update_docker_image', async (req, res, next) => {
    const params = {
        name: req.body.name,
        image_id: req.body.image_id,
        status: 'BUILT_DOCKER'
    };

    await activeServerService.updateDockerImage(params);
    await dockerImageService.addDocker(params);

    return res.send(params);
});

router.post('/update_container_id', async (req, res, next) => {
    const params = {
        name: req.body.name,
        revision: req.body.revision,
        container_id: req.body.container_id,
        status: 'RUNNING'
    };

    await activeServerService.updateContainerId(params);
    return res.send(params);
});

router.post('/update-server', async (req, res, next) => {
    const params = {
        id: req.body.id,
        status: 'UPDATING'
    };

    await activeServerService.updateStatus(params);

    const command = "curl";
    const argument = [`-u${jenkins.id}:${jenkins.password}`, `-vs ${jenkins.dev_build.webhook}`];

    const { spawn } = require('child_process');
    const child = spawn(command, argument, { shell: true });

    child.stdout.on('data', (data) => {
        console.log(`stdout ${data}`);
    });

    return res.send(params);
});

router.post('/add-db', async (req, res, next) => {
    const params = {
        name: req.body.name,
        branch: req.body.branch,
    };

    await activeServerService.addDb(params);

    const jsonBody = JSON.stringify(params);
    const command = "curl";
    const argument = [`-u${jenkins.id}:${jenkins.password}`, `-vs ${jenkins.db_build.webhook}`,
        '-X POST', '-H "Content-Type: application/json"', `-d '${jsonBody}'`
    ];

    const { spawn } = require('child_process');
    const child = spawn(command, argument, { shell: true });

    child.stdout.on('data', (data) => {
        console.log(`stdout ${data}`);
    });

    child.stderr.on('data', (data) => {
        console.error(`stderr ${data}`);
    });

    const result = await activeServerService.getDbByName(params);
    return res.send(result[0]);
});

router.delete('/delete-db/:dbName', async (req, res, next) => {
    await activeServerService.deleteDb(req.params.dbName);
    return res.send({ name: req.params.dbName });
});

router.post('/update-data-table', async (req, res, next) => {
    const params = {
        name: req.body.name,
        branch: req.body.branch,
    };

    await activeServerService.deleteDataTable(params.name);

    const jsonBody = JSON.stringify(params);
    const command = "curl";
    const argument = [`-u${jenkins.id}:${jenkins.password}`, `-vs ${jenkins.data_table_deploy.webhook}`,
        '-X POST', '-H "Content-Type: application/json"', `-d '${jsonBody}'`
    ];

    const { spawn } = require('child_process');
    const child = spawn(command, argument, { shell: true });

    child.stdout.on('data', (data) => {
        console.log(`stdout ${data}`);
    });

    child.stderr.on('data', (data) => {
        console.error(`stderr ${data}`);
    });

    const result = await activeServerService.getDbByName(params);
    return res.send(result[0]);
});

router.post('/docker-image/restart', async (req, res, next) => {
    const containerId = req.body.containerId;

    const command = "sudo";
    const argument = ['docker', 'restart', containerId];

    const { spawn } = require('child_process');
    const child = spawn(command, argument, { shell: true });

    child.stdout.on('data', (data) => {
        console.log(`stdout ${data}`);
    });

    const params = {
        id: req.body.serverId,
        status: 'RESTARTING'
    };

    await activeServerService.updateStatus(params);
    return res.send({ containerId: containerId });
});

router.delete('/docker-image/:dockerImageName', async (req, res, next) => {
    const params = {
        name: req.params.dockerImageName
    };

    const activeServer = await activeServerService.getActiveServerByName(params);
    if (activeServer[0]) {
        return res.send({ error: "exist active server" });
    }

    const docker = await dockerImageService.getDockerByName(params);

    const command = "sudo";
    const argument = ['docker', 'rmi', docker[0].image_id];

    const { spawn } = require('child_process');
    const child = spawn(command, argument, { shell: true });

    child.stdout.on('data', (data) => {
        console.log(`stdout ${data}`);
    });

    dockerImageService.deleteDocker(params);
    return res.send(params);
});

//tools 
router.post('/compare-file-md5', upload.array('files'), toolsControllers.compareFileMd5);
    
   
module.exports = router;
