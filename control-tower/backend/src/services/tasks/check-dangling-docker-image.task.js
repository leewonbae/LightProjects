const { SimpleIntervalJob, Task } = require('toad-scheduler');

const checkDanglingDockerImage = new Task('CheckDanglingDockerImage', async () => {
    try {
        const command = "sudo";
        const argument = ["docker", "rmi", '$(sudo docker images --filter "dangling=true" -q --no-trunc)'];

        const { spawn } = require('child_process');
        const child = spawn(command, argument, { shell: true });
    
        child.stdout.on('data', (data) => {
            console.log(`stdout ${data}`);
        });
    
        child.stderr.on('data', (data) => {
            console.error(`stderr ${data}`);
        });
    } catch (e) {
        console.log(e);
    }
});

console.log('[Task] Check Dangling Docker Image');
module.exports = new SimpleIntervalJob({ hours: 1 }, checkDanglingDockerImage);