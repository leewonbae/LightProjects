// vue.config.js
const path = require("path");

module.exports = {
  chainWebpack: config => {
    config.resolve.alias.set("vue", "@vue/compat");

    config.module
      .rule("vue")
      .use("vue-loader")
      .tap(options => {
        return {
          ...options,
        };
      });
  },
  configureWebpack: {
    resolve: {
      alias: {
        "@": path.join(__dirname, "src/"),
      },
    },
  },
  /////////////// css - basic & dark /////////////
  css: {
    loaderOptions: {
      sass: {
        additionalData: `@import "@/assets/scss/common.scss";`,
      },
    },
  },
  transpileDependencies: ["vuetify"],
  runtimeCompiler: true,
  outputDir: "../backend/public",
};
