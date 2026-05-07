import { createWebHistory, createRouter } from "vue-router";

const routes = [
  {
    path: "/one-page",
    component: () => import("@/views/OnePage.vue"),
    children: [
      {
        path: "home",
        alias: "/",
        name: "the-home",
        component: () => import("@/views/TheHome.vue"),
      },
      {
        path: "table",
        alias: "/table",
        name: "the-table",
        component: () => import("@/views/TheTable.vue"),
      },
      {
        path: "tools",
        alias: "/tools",
        name: "the-tools",
        component: () => import("@/views/TheTools.vue"),
      },
    ],
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
