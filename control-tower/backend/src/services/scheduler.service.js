const { ToadScheduler } = require('toad-scheduler')

console.log('[ToadScheduler] Running');
const scheduler = new ToadScheduler();

if (global.configEnv === 'dev') {
    scheduler.addSimpleIntervalJob(require('./tasks/check-active-server.task'));

    if (process.platform === "linux") {
        scheduler.addSimpleIntervalJob(require('./tasks/check-dangling-docker-image.task'));
    }
}