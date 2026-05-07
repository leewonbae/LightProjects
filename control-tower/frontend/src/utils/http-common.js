import axios from "axios";

axios.defaults.withCredentials = true;
axios.defaults.headers.post["Content-Type"] = "application/json;charset=utf-8";
axios.defaults.headers.post["Access-Control-Allow-Origin"] = "*";

export default axios.create({
  baseURL: `http://${process.env.VUE_APP_HTTP_SERVER}/api`,
  headers: {
    "Content-type": "application/json",
  },
  withCredentials: true,
});
