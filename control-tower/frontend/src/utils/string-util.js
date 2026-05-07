module.exports = {
  padZero: (num, length) => {
    return String(num).padStart(length, "0");
  },

  getJsonWithLocalize(str) {
    if (!str) {
      return { ko: "한글", en: "영문" };
    }

    try {
      return JSON.parse(str);
    } catch (e) {
      return { ko: str, en: str };
    }
  },
};
