// .eslintrc.js
module.exports = {
  // 현재 eslintrc 파일을 기준으로 ESLint 규칙을 적용
  root: true,

  env: {
    browser: true,
    amd: true,
    node: true,
    commonjs: true,
  },

  // 추가적인 규칙들을 적용
  extends: [
    "@vue/prettier",
    "plugin:prettier/recommended",
    "plugin:vue/vue3-essential",
    "plugin:vue/vue3-recommended",
    "eslint:recommended",
    "prettier",
  ],

  // 코드 정리 플러그인 추가
  plugins: ["prettier"],

  // 사용자 편의 규칙 추가
  rules: {
    "vue/html-closing-bracket-newline": [
      "error",
      {
        singleline: "never",
        multiline: "never",
      },
    ],
    "vue/component-name-in-template-casing": [
      "error",
      "kebab-case",
      {
        registeredComponentsOnly: false,
      },
    ],
    "prettier/prettier": [
      "error",
      {
        singleQuote: false,
        semi: true,
        indent: [
          2,
          2,
          {
            SwitchCase: 1,
          },
        ],
        useTabs: false,
        tabWidth: 2,
        trailingComma: "all",
        printWidth: 100,
        bracketSpacing: true,
        arrowParens: "avoid",
        htmlWhitespaceSensitivity: "ignore",
        bracketSameLine: true,
        endOfLine: "auto",
      },
    ],
    "no-console": process.env.NODE_ENV === "production" ? "warn" : "off",
    "no-debugger": process.env.NODE_ENV === "production" ? "warn" : "off",
  },

  parserOptions: {
    ecmaFeatures: {
      jsx: true,
    },
    ecmaVersion: 2020,
    parser: "@babel/eslint-parser",
  },
};
