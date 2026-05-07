import { createApp, configureCompat } from "vue";

//CSS
import "bootstrap";
import "bootstrap/dist/css/bootstrap.min.css";

// fontawesome
import { library } from "@fortawesome/fontawesome-svg-core";
import { faToggleOn, faUserSecret } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import { fas } from "@fortawesome/free-solid-svg-icons";

import App from "./App.vue";
import router from "./router";

configureCompat({
  GLOBAL_EXTEND: "suppress-warning",
  WATCH_ARRAY: false,
  COMPONENT_V_MODEL: false,
  RENDER_FUNCTION: false,
  TRANSITION_GROUP_ROOT: false,
  MODE: 2,
});

let app = createApp({
  extends: App,
});

// fontawesome
library.add(fas);
library.add(faUserSecret);
library.add(faToggleOn);
app.component("FontAwesomeIcon", FontAwesomeIcon);

app.use(router).mount("#app");
