const dayjs = require("dayjs");
const objectSupport = require("dayjs/plugin/objectSupport");
const utc = require("dayjs/plugin/utc");
const timezone = require("dayjs/plugin/timezone"); // dependent on utc plugin

dayjs.extend(objectSupport);
dayjs.extend(utc);
dayjs.extend(timezone);

const stringUtil = require("./string-util");

function getFormattedDateTime(dayjs, format) {
  return dayjs.format(format);
}

module.exports = {
  setTimeZero: dateTime => {
    return dayjs(module.exports.getISODate(dateTime));
  },
  getDiffDate: (date1, date2) => {
    const milliSeconds = module.exports.setTimeZero(date1).diff(module.exports.setTimeZero(date2));
    return milliSeconds / 1000 / 60 / 60 / 24;
  },
  getISODateTimeNow: () => {
    return getFormattedDateTime(dayjs(), "YYYY-MM-DD HH:mm:ss");
  },
  getISODateTime: date => {
    return getFormattedDateTime(dayjs(date), "YYYY-MM-DD HH:mm:ss");
  },
  getISODateNow: () => {
    return getFormattedDateTime(dayjs(), "YYYY-MM-DD");
  },
  getISODateTimeAddMinute: (isoDate, addMinute) => {
    return getFormattedDateTime(dayjs(isoDate).add(addMinute, "minute"), "YYYY-MM-DD HH:mm:00");
  },
  getISODateTimeAddHour: (isoDate, addHour) => {
    return getFormattedDateTime(dayjs(isoDate).add(addHour, "hour"), "YYYY-MM-DD HH:mm:00");
  },
  getISODateTimeAddDay: (isoDate, addDay) => {
    return getFormattedDateTime(dayjs(isoDate).add(addDay, "day"), "YYYY-MM-DD HH:mm:00");
  },
  applyUTC: isoDate => {
    if (!isoDate) {
      return "";
    }

    return getFormattedDateTime(dayjs(isoDate).utc(), "YYYY-MM-DD HH:mm:ss");
  },
  applyTimeZone: (isoDate, timeZone) => {
    if (!isoDate) {
      return "";
    }

    return getFormattedDateTime(dayjs(isoDate).tz(timeZone), "YYYY-MM-DD HH:mm:ss");
  },
  hasExpired: dateStr => {
    if (!dateStr) {
      return false;
    }

    return dayjs().isAfter(dayjs(dateStr));
  },
  getISOTimeFromSeconds: totalSeconds => {
    if (!totalSeconds) {
      totalSeconds = 0;
    }

    const hours = parseInt(totalSeconds / 60 / 60);
    const minutes = parseInt((totalSeconds - hours * 60 * 60) / 60);
    const seconds = totalSeconds - hours * 60 * 60 - minutes * 60;

    return (
      `${stringUtil.padZero(hours, 2)}:` +
      `${stringUtil.padZero(minutes, 2)}:` +
      stringUtil.padZero(seconds, 2)
    );
  },
};
